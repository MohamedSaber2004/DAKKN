// ── Theme Management ────────────────────────────

function applyTheme(theme) {
    if (theme === 'dark') {
        document.documentElement.classList.add('dark');
    } else {
        document.documentElement.classList.remove('dark');
    }
}

function toggleDarkMode() {
    const isDark = document.documentElement.classList.toggle('dark');
    const theme = isDark ? 'dark' : 'light';
    localStorage.setItem('dakkn_theme', theme);
    // Dispatch a storage event manually for the current tab
    window.dispatchEvent(new StorageEvent('storage', {
        key: 'dakkn_theme',
        newValue: theme
    }));

    // Sync with database if CSRF token is present (user likely logged in)
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || window.csrfToken;
    if (token) {
        syncSettingsToDatabase({ isDarkMode: isDark, theme: theme }, token);
    }
}

async function syncSettingsToDatabase(settings, token) {
    try {
        const response = await fetch('/api/v1/settings', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(settings)
        });
        if (!response.ok) {
            console.warn('Failed to sync settings:', await response.text());
        }
    } catch (error) {
        console.error('Error syncing settings to database:', error);
    }
}

// Sync theme across tabs
window.addEventListener('storage', (e) => {
    if (e.key === 'dakkn_theme') {
        applyTheme(e.newValue);
    }
});

// ── Language State Management ────────────────────────────

// Utility to get current language from document
function getCurrentLang() {
    return document.documentElement.lang || 'ar';
}

// Stateless language toggle - always reads fresh from DOM
function toggleLang() {
    const current = getCurrentLang();
    const target = current.startsWith('en') ? 'ar' : 'en';
    
    // Persist language choice
    localStorage.setItem('dakkn_lang', target);
    setCookie(".AspNetCore.Culture", `c=${target}|uic=${target}`, 365);
    
    // Reload to let server-side @Localizer take over for all static text
    window.location.reload();
}

let currentTranslations = {};

async function fetchTranslations(lang) {
    try {
        const response = await fetch(`/api/v1/translations?lang=${lang}`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    } catch (error) {
        console.error('Error fetching translations:', error);
        return null;
    }
}

function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    const nameEQ = name + "=";
    const ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

// applyLang now handles explicit language changes and dynamic components
async function applyLang(lang, reload = false) {
    const current = getCurrentLang();
    
    // If explicit change requested
    if (lang && lang !== current) {
        // Persist language choice
        localStorage.setItem('dakkn_lang', lang);
        setCookie(".AspNetCore.Culture", `c=${lang}|uic=${lang}`, 365);
        
        if (reload) {
            window.location.reload();
            return;
        }
    }

    // Use pre-injected translations if available, otherwise fetch
    if (window.currentTranslations && Object.keys(window.currentTranslations).length) {
        currentTranslations = window.currentTranslations;
    } else if (!Object.keys(currentTranslations).length) {
        const s = await fetchTranslations(lang || current);
        if (s) currentTranslations = s;
    }
    // Re-render dynamic content with new language
    await renderFeaturedProducts();
    await renderCategoriesSlider();
    await renderCategoryFilters();
    await initPriceSlider();
    await renderAllProducts();
}

// Utility to hide preloader once everything is ready
function hidePreloader() {
    const preloader = document.getElementById('dakkn-preloader');
    if (preloader) {
        preloader.style.opacity = '0';
        preloader.style.transition = 'opacity 0.4s ease';
        setTimeout(() => {
            if (preloader.parentNode) preloader.remove();
            document.documentElement.removeAttribute('data-dakkn-loading');
        }, 400);
    } else {
        document.documentElement.removeAttribute('data-dakkn-loading');
    }
}

/* ── Boot ────────────────────────────────────────────────── */
document.addEventListener('DOMContentLoaded', async () => {
    try {
        // Determine target language: Cookie > LocalStorage > Page Lang
        const cookieValue = getCookie(".AspNetCore.Culture");
        let saved = null;
        
        if (cookieValue) {
            const match = cookieValue.match(/c=([a-z]{2})/);
            if (match) saved = match[1];
        }
        
        if (!saved) {
            saved = localStorage.getItem('dakkn_lang');
        }

        const current = getCurrentLang();
        
        // Loop prevention: check if we just reloaded for sync
        const isSyncing = sessionStorage.getItem('dakkn_lang_syncing');
        const isAdmin = window.location.pathname.toLowerCase().startsWith('/admin');
        
        // If the saved preference differs from the page language, reload once to sync
        // NOTE: Disable this auto-sync for Admin pages as they handle it via Settings
        if (saved && saved !== current && !isSyncing && !isAdmin) {
            console.log(`Syncing language: ${current} -> ${saved}`);
            sessionStorage.setItem('dakkn_lang_syncing', 'true');
            setCookie(".AspNetCore.Culture", `c=${saved}|uic=${saved}`, 365);
            window.location.reload();
            return;
        }

        // Clear sync flag once we match
        sessionStorage.removeItem('dakkn_lang_syncing');

        // Initialize frozen actions (guest action blocking)
        initFrozenActions();
        // Initialize delegated add-to-cart for dynamically rendered buttons
        initDelegatedAddToCart();
        // Initialize cart badge from server session (fire-and-forget, don't block rendering)
        initCartBadge().catch(() => {});

        // Initialize dynamic components with current language
        await applyLang(current);

        // Wait for fonts (Icons) to be ready to avoid icon text flickering
        if (document.fonts) {
            try {
                await Promise.race([
                    document.fonts.ready,
                    new Promise(resolve => setTimeout(resolve, 1000)) // Max 1s wait for fonts
                ]);
            } catch (e) {
                console.warn('Font loading failed or timed out.');
            }
        }

        // Initialize Components (wrapped to prevent one failure blocking everything)
        const safeInit = (fnName) => {
            try {
                if (typeof window[fnName] === 'function') window[fnName]();
            } catch (e) {
                console.error(`Error in ${fnName}:`, e);
            }
        };

        safeInit('initScrollAnimations');
        safeInit('initStaggeredGrids');
        safeInit('initNavbarScrollEffect');
        safeInit('initScrollSpy');

        // OTP & Password micro-interactions
        initOtpBoxes();
        initPasswordToggles();
        initOtpCountdown();

        // Attach click handlers for all in-page anchors
        document.querySelectorAll('a[href^="#"]').forEach(link => {
            const href = link.getAttribute('href');
            if (!href || href === '#') return;
            
            link.addEventListener('click', (e) => {
                const targetId = href.substring(1);
                if (!document.getElementById(targetId)) return;
                e.preventDefault();
                scrollToHash(href);
            });
        });

        if (window.location.hash) {
            setTimeout(() => scrollToHash(window.location.hash, false), 150);
        }

    } catch (error) {
        console.error('Boot sequence error:', error);
    } finally {
        // Everything is ready (or failed), hide preloader
        hidePreloader();
    }
});

/* ── Sub-Initializers ────────────────────────────────────── */

function initOtpBoxes() {
    const boxes = [...document.querySelectorAll('#otp-box-group input')];
    if (!boxes.length) return;
    boxes.forEach((box, idx) => {
        box.addEventListener('input', e => {
            const val = e.target.value.replace(/\D/g, '');
            e.target.value = val.slice(-1);
            if (val && idx < boxes.length - 1) boxes[idx + 1].focus();
        });
        box.addEventListener('keydown', e => {
            if (e.key === 'Backspace' && !box.value && idx > 0) {
                boxes[idx - 1].focus();
                boxes[idx - 1].value = '';
            }
        });
    });
    boxes[0]?.focus();
}

function initPasswordToggles() {
    document.querySelectorAll('[data-pw-toggle]').forEach(btn => {
        btn.addEventListener('click', () => {
            const wrapper = btn.closest('.relative');
            const input = wrapper?.querySelector('[data-pw-field]');
            const eyeOpen = btn.querySelector('[data-eye-open]');
            const eyeClosed = btn.querySelector('[data-eye-closed]');
            if (!input) return;
            const isHidden = input.type === 'password';
            input.type = isHidden ? 'text' : 'password';
            eyeOpen?.classList.toggle('hidden', isHidden);
            eyeClosed?.classList.toggle('hidden', !isHidden);
        });
    });
}

function initOtpCountdown() {
    const el = document.getElementById('otp-countdown');
    if (!el) return;
    let remaining = 10 * 60;
    const timer = setInterval(() => {
        const m = String(Math.floor(remaining / 60)).padStart(2, '0');
        const s = String(remaining % 60).padStart(2, '0');
        el.textContent = `${m}:${s}`;
        if (remaining-- <= 0) {
            clearInterval(timer);
            el.textContent = '00:00';
            el.classList.add('text-red-500');
        }
    }, 1000);
}

function getHeaderOffset() {
    const navbar = document.querySelector('.navbar-animate');
    return navbar ? Math.ceil(navbar.getBoundingClientRect().height + 8) : 80;
}

function scrollToHash(hash, updateHistory = true) {
    if (!hash || hash === '#') return;
    const targetId = hash.startsWith('#') ? hash.substring(1) : hash;
    const target = document.getElementById(targetId);
    if (!target) return;
    target.scrollIntoView({ behavior: 'smooth' });
    if (updateHistory) history.pushState(null, '', `#${targetId}`);
}

function initScrollAnimations() {
    const animated = document.querySelectorAll('.scroll-animate, .scroll-animate-left, .scroll-animate-right, .scroll-animate-scale');
    if (!animated.length) return;
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.12, rootMargin: '0px 0px -40px 0px' });
    animated.forEach(el => observer.observe(el));
}

function initStaggeredGrids() {
    document.querySelectorAll('[data-stagger]').forEach(grid => {
        grid.querySelectorAll(':scope > *').forEach((child, i) => {
            child.style.transitionDelay = `${i * 0.1}s`;
        });
    });
}

function initNavbarScrollEffect() {
    const navbar = document.querySelector('.navbar-animate');
    if (!navbar) return;
    let ticking = false;
    window.addEventListener('scroll', () => {
        if (!ticking) {
            requestAnimationFrame(() => {
                navbar.classList.toggle('navbar-scrolled', window.scrollY > 60);
                ticking = false;
            });
            ticking = true;
        }
    }, { passive: true });
}

function initScrollSpy() {
    const sections = Array.from(document.querySelectorAll('section[id]'));
    const navLinks = document.querySelectorAll('[data-section]');
    if (!sections.length || !navLinks.length) return;
    const sectionMapping = { 'hero': 'shop', 'shop': 'shop', 'about': 'shop', 'categories': 'categories', 'all-products': 'all-products', 'testimonials': 'testimonials', 'contact': 'contact' };
    const onScroll = () => {
        const headerHeight = getHeaderOffset();
        const scrollPos = window.scrollY + headerHeight + 100;
        const isBottom = (window.innerHeight + window.scrollY) >= document.body.offsetHeight - 50;
        if (window.scrollY > 100 && isBottom) {
            setActive(sections[sections.length - 1].id);
            return;
        }
        let currentSectionId = sections[0].id;
        for (const section of sections) {
            if (scrollPos >= section.offsetTop) currentSectionId = section.id;
            else break;
        }
        setActive(currentSectionId);
    };
    function setActive(id) {
        const activeLinkId = sectionMapping[id] || id;
        navLinks.forEach(link => {
            if (link.getAttribute('data-section') === activeLinkId) link.classList.add('active');
            else link.classList.remove('active');
        });
    }
    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();
}

/* ── Categories Slider ─────────────────────────────────── */
let currentSlideIndex = 0;
function getSlideLimit() {
    const track = document.getElementById('cat-slider-track');
    if (!track) return 0;
    const slides = track.children.length;
    let visible = (window.innerWidth >= 1024) ? 4 : (window.innerWidth >= 768) ? 3 : (window.innerWidth >= 640) ? 2 : 1;
    return Math.max(0, slides - visible);
}
function updateSlider() {
    const track = document.getElementById('cat-slider-track');
    if (!track) return;
    const limit = getSlideLimit();
    if (currentSlideIndex > limit) currentSlideIndex = limit;
    if (currentSlideIndex < 0) currentSlideIndex = 0;
    const firstSlide = track.firstElementChild;
    if (!firstSlide) return;
    const slideWidth = firstSlide.getBoundingClientRect().width;
    const gap = 24;
    const isRtl = document.documentElement.dir === 'rtl';
    const offset = currentSlideIndex * (slideWidth + gap) * (isRtl ? 1 : -1);
    track.style.transform = `translateX(${offset}px)`;
    for (let i = 0; i <= 2; i++) {
        const dot = document.getElementById(`cat-dot-${i}`);
        if (dot) {
            const targetDot = Math.min(2, Math.floor(currentSlideIndex * 3 / (track.children.length - 1)));
            dot.classList.toggle('bg-primary', i === targetDot);
            dot.classList.toggle('bg-primary/30', i !== targetDot);
        }
    }
}
function slideNext() { currentSlideIndex = (currentSlideIndex < getSlideLimit()) ? currentSlideIndex + 1 : 0; updateSlider(); }
function slidePrev() { currentSlideIndex = (currentSlideIndex > 0) ? currentSlideIndex - 1 : getSlideLimit(); updateSlider(); }
function slideGoTo(index) { currentSlideIndex = Math.round((index / 2) * getSlideLimit()); updateSlider(); }

window.slideNext = slideNext;
window.slidePrev = slidePrev;
window.slideGoTo = slideGoTo;
window.updateSlider = updateSlider;
window.toggleLang = toggleLang;
window.guestAddToCart = guestAddToCart;

let resizeTimer;
window.addEventListener('resize', () => {
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(updateSlider, 150);
});

/* ── Catalog API ────────────────────────────────────── */
async function apiFetch(url) {
    const res = await fetch(url);
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    return res.json();
}

async function fetchCategories() {
    const json = await apiFetch('/api/v1/catalog/categories');
    return json.data || [];
}

async function fetchFeaturedProducts() {
    const json = await apiFetch('/api/v1/catalog/products/featured');
    return json.data || [];
}

async function fetchProducts(page, pageSize, categoryId, maxPrice) {
    const params = new URLSearchParams({ pageNumber: page, pageSize });
    if (categoryId) params.set('categoryId', categoryId);
    if (maxPrice) params.set('maxPrice', maxPrice);
    const json = await apiFetch(`/api/v1/catalog/products?${params}`);
    return json.data || { items: [], totalPages: 1, pageNumber: 1 };
}

async function fetchPriceRange() {
    try {
        const json = await apiFetch('/api/v1/catalog/products/price-range');
        return json.data || { minPrice: 0, maxPrice: 200 };
    } catch {
        return { minPrice: 0, maxPrice: 200 };
    }
}

/* ── Placeholder ──────────────────── */
const PRODUCT_PLACEHOLDER = '/images/placeholders/product-placeholder.svg';

async function guestAddToCart(btnOrId, nameOrId, priceOrName, imageOrPrice, maybeImage) {
    let id, name, btn;
    if (typeof btnOrId === 'object' && btnOrId.tagName) {
        btn = btnOrId;
        id = nameOrId;
        name = priceOrName;
    } else {
        id = btnOrId;
        name = nameOrId;
    }
    try {
        const response = await fetch('/api/v1/cart/add', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productId: id, quantity: 1 })
        });
        const data = await response.json();
        if (data.success) {
            document.querySelectorAll('[id^="cart-badge"]').forEach(el => el.textContent = data.data || 0);
            if (btn) {
                const orig = btn.innerHTML;
                btn.innerHTML = '<span class="material-symbols-outlined text-sm">check</span>';
                btn.classList.add('bg-green-600', 'scale-95');
                setTimeout(() => {
                    btn.classList.remove('bg-green-600', 'scale-95');
                    btn.innerHTML = orig;
                }, 1000);
            }
            showCartToast(data.message || name || id);
            bounceCartBadge();
        }
    } catch (e) {
        console.error('Failed to add to cart:', e);
    }
}

function showCartToast(message) {
    const container = document.getElementById('cart-toast-container');
    if (!container) return;
    const toast = document.createElement('div');
    toast.className = 'cart-toast';
    toast.innerHTML = `<span class="material-symbols-outlined text-green-400 text-lg">check_circle</span><span>${message}</span>`;
    container.appendChild(toast);
    requestAnimationFrame(() => toast.classList.add('cart-toast-show'));
    setTimeout(() => {
        toast.classList.remove('cart-toast-show');
        toast.addEventListener('transitionend', () => toast.remove(), { once: true });
    }, 2500);
}

async function initCartBadge() {
    try {
        const response = await fetch('/api/v1/cart/count');
        const data = await response.json();
        const count = data.success ? (data.data || 0) : 0;
        document.querySelectorAll('[id^="cart-badge"]').forEach(el => el.textContent = count);
    } catch (e) {
        document.querySelectorAll('[id^="cart-badge"]').forEach(el => el.textContent = '0');
    }
}

function bounceCartBadge() {
    document.querySelectorAll('[id^="cart-badge"]').forEach(el => {
        el.classList.remove('cart-badge-bounce');
        void el.offsetWidth;
        el.classList.add('cart-badge-bounce');
        setTimeout(() => el.classList.remove('cart-badge-bounce'), 500);
    });
}

function resolveImage(url) {
    if (!url) return PRODUCT_PLACEHOLDER;
    if (url.startsWith('http') || url.startsWith('/')) return url;
    return '/files/' + url;
}

/* ── Featured Products (Shop section) ─────────────── */
async function renderFeaturedProducts() {
    const grid = document.getElementById('featured-products-grid');
    if (!grid) return;
    try {
        const items = await fetchFeaturedProducts();
        if (!items.length) {
            grid.innerHTML = `<div class="col-span-full text-center py-16">
                <span class="material-symbols-outlined text-6xl text-on-surface-variant/30 mb-4">inventory_2</span>
                <h3 class="text-xl font-bold text-on-surface dark:text-white mb-2">${currentTranslations.featured_empty_title || 'No Featured Products Available'}</h3>
                <p class="text-sm text-on-surface-variant dark:text-slate-400">${currentTranslations.featured_empty_msg || 'New products will appear here automatically.'}</p>
            </div>`;
            return;
        }
        const isAr = document.documentElement.lang.startsWith('ar');
        grid.innerHTML = items.map((p, i) => `
            <div class="product-card glass-panel p-5 rounded-2xl flex flex-col gap-4 scroll-animate delay-${(i + 1) * 100} dark:bg-slate-900/40 group hover:-translate-y-1 transition-all duration-300">
                <a href="/shop/product/${p.id}" class="no-underline">
                    <div class="aspect-square rounded-xl overflow-hidden relative">
                        <img class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" src="${resolveImage(p.imageUrl)}"
                             onerror="this.src='${PRODUCT_PLACEHOLDER}'" alt="${isAr ? p.arName : p.name}" loading="lazy" />
                    </div>
                    <div>
                        <h3 class="text-lg font-bold text-on-surface dark:text-white">${isAr ? p.arName : p.name}</h3>
                        <p class="text-xs text-on-surface-variant dark:text-slate-400">${isAr ? p.categoryArName : p.categoryName}</p>
                    </div>
                </a>
                <div class="flex items-center justify-between mt-auto pt-2">
                    <span class="text-xl font-bold text-primary">${p.price.toLocaleString()} ${isAr ? 'ج.م' : 'EGP'}</span>
                    <div class="flex gap-2">
                        <button data-product-add="${p.id}" data-product-name="${isAr ? (p.arName || p.name) : p.name}" data-product-price="${p.price}" data-product-image="${p.imageUrl || ''}" class="px-3 py-2 rounded-lg bg-primary text-on-primary text-[10px] font-bold hover:bg-primary-container hover:text-primary transition-all flex items-center gap-1">
                            <span class="material-symbols-outlined text-sm">add_shopping_cart</span>
                        </button>
                        <a href="/shop/product/${p.id}" class="px-3 py-2 rounded-lg bg-primary/10 text-primary text-[10px] font-bold hover:bg-primary hover:text-on-primary transition-all no-underline">View Details</a>
                    </div>
                </div>
            </div>`).join('');
    } catch (err) {
        console.error('Error rendering featured products:', err);
        grid.innerHTML = `<div class="col-span-full text-center py-12"><p class="text-on-surface-variant">Failed to load products.</p></div>`;
    }
}

/* ── Categories Slider ────────────────────────────── */
let categoriesData = [];

function catSkeleton() {
    return `<div class="flex gap-6 w-full">${Array.from({length:4},(_,i)=>`
        <div class="flex-shrink-0 w-[calc(25%-18px)] aspect-square rounded-2xl overflow-hidden glass-panel animate-pulse dark:bg-slate-900/60">
            <div class="w-full h-full bg-surface-container-high dark:bg-slate-800/60"></div>
        </div>`).join('')}</div>`;
}

async function renderCategoriesSlider() {
    const track = document.getElementById('cat-slider-track');
    if (!track) return;
    track.innerHTML = catSkeleton();
    try {
        categoriesData = await fetchCategories();
        if (!categoriesData.length) {
            track.innerHTML = `<div class="flex-shrink-0 w-full text-center py-16">
                <span class="material-symbols-outlined text-6xl text-on-surface-variant/30 mb-4">grid_view</span>
                <h3 class="text-xl font-bold text-on-surface dark:text-white mb-2">${currentTranslations.categories_empty_title || 'No Categories Yet'}</h3>
                <p class="text-sm text-on-surface-variant dark:text-slate-400">${currentTranslations.categories_empty_msg || 'Categories will appear once products are added.'}</p>
            </div>`;
            return;
        }
        const isAr = document.documentElement.lang.startsWith('ar');
        const CATEGORY_PLACEHOLDER = '/images/placeholders/category-placeholder.svg';
        track.innerHTML = categoriesData.map(cat => `
            <a href="/shop/products?categoryId=${cat.id}" data-frozen-action
               class="group rounded-2xl overflow-hidden relative flex-shrink-0 w-[calc(100%-0px)] sm:w-[calc(50%-12px)] md:w-[calc(33.333%-16px)] lg:w-[calc(25%-18px)] aspect-square glass-panel shadow-sm dark:bg-slate-900/40 no-underline block">
                <img src="${resolveImage(cat.imageUrl)}" alt="${isAr ? cat.arName : cat.categoryName}"
                     class="absolute inset-0 w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                     loading="lazy" onerror="this.src='${CATEGORY_PLACEHOLDER}'" />
                <div class="absolute inset-0 bg-black/20 group-hover:bg-black/10 transition-colors z-10"></div>
                <div class="absolute bottom-0 left-0 right-0 p-6 cat-overlay z-20">
                    <h3 class="text-xl font-bold text-white">${isAr ? cat.arName : cat.categoryName}</h3>
                    <p class="text-sm text-white/80">${cat.productsCount} Products</p>
                </div>
            </a>`).join('');
        updateSlider();
    } catch (err) {
        console.error('Error rendering categories:', err);
        track.innerHTML = `<div class="flex-shrink-0 w-full text-center py-12"><p class="text-on-surface-variant">Failed to load categories.</p></div>`;
    }
}

async function renderCategoryFilters() {
    const container = document.getElementById('prod-cat-filters');
    if (!container) return;
    if (!categoriesData.length) return;
    const isAr = document.documentElement.lang.startsWith('ar');
    container.innerHTML = `
        <button onclick="filterCategory(null)" id="cat-filter-all" class="cat-pill px-4 py-2 rounded-full border text-xs font-bold transition-all duration-200 bg-primary border-primary text-white">
            <span>All Categories</span>
        </button>
        ${categoriesData.map(cat => `
            <button onclick="filterCategory('${cat.id}')" id="cat-filter-${cat.id}" class="cat-pill px-4 py-2 rounded-full border border-primary/20 text-primary hover:border-primary text-xs font-bold transition-all duration-200 bg-white/50 dark:bg-slate-800/50">
                <span>${isAr ? cat.arName : cat.categoryName}</span>
            </button>
        `).join('')}`;
}

window.renderCategoryFilters = renderCategoryFilters;

let selectedCategoryId = null;
let maxPrice = 200;
let priceMin = 0;
let priceMax = 200;
let prodsPage = 1;
const itemsPerPage = 8;

async function renderAllProducts() {
    const grid = document.getElementById('all-prods-grid');
    if (!grid) return;
    try {
        const priceVal = maxPrice < priceMax ? maxPrice : null;
        const result = await fetchProducts(prodsPage, itemsPerPage, selectedCategoryId, priceVal);
        const items = result.items || [];
        const totalPages = result.totalPages || 1;
        const isAr = document.documentElement.lang.startsWith('ar');
        if (!items.length) {
            grid.innerHTML = `<div class="col-span-full py-16 text-center">
                <span class="material-symbols-outlined text-6xl text-on-surface-variant/30 mb-4">search_off</span>
                <h3 class="text-xl font-bold text-on-surface dark:text-white mb-2">${currentTranslations.no_products_title || 'No Products Found'}</h3>
                <p class="text-sm text-on-surface-variant dark:text-slate-400">${currentTranslations.no_products_found || 'No stickers match your filters.'}</p>
            </div>`;
        } else {
            grid.innerHTML = items.map(p => `
                <div class="product-card glass-panel p-5 rounded-2xl flex flex-col gap-4 scroll-animate visible" style="opacity:1;transform:none;">
                    <div class="aspect-square rounded-xl overflow-hidden relative"><img class="w-full h-full object-cover" src="${resolveImage(p.imageUrl)}" onerror="this.src='${PRODUCT_PLACEHOLDER}'" alt="${isAr ? p.arName : p.name}" loading="lazy"/></div>
                    <div><h3 class="text-lg font-bold text-on-surface dark:text-white">${isAr ? p.arName : p.name}</h3><p class="text-xs text-on-surface-variant dark:text-slate-400">${isAr ? p.categoryArName : p.categoryName}</p></div>
                    <div class="flex items-center justify-between mt-auto pt-2"><span class="text-xl font-bold text-primary font-sans">${p.price.toLocaleString()} ${isAr ? 'ج.م' : 'EGP'}</span><div class="flex gap-2"><button data-product-add="${p.id}" data-product-name="${isAr ? (p.arName || p.name) : p.name}" data-product-price="${p.price}" data-product-image="${p.imageUrl || ''}" class="px-3 py-2 rounded-lg bg-primary text-on-primary text-[10px] font-bold hover:bg-primary-container hover:text-primary transition-all flex items-center gap-1"><span class="material-symbols-outlined text-sm">add_shopping_cart</span></button><a href="/shop/product/${p.id}" class="px-3 py-2 rounded-lg bg-primary/10 text-primary hover:bg-primary hover:text-on-primary text-[10px] font-bold transition-all duration-200">${currentTranslations.shop_view_details || 'View Details'}</a></div></div>
                </div>`).join('');
        }
        const priceDisplay = document.getElementById('price-val-display');
        if (priceDisplay) {
            const val = maxPrice >= priceMax ? (isAr ? 'الكل' : 'All') : (isAr ? maxPrice + ' ج.م' : maxPrice + ' EGP');
            priceDisplay.textContent = val;
        }
        updatePaginationIndicators(totalPages);
        updateActiveFilterPill();
    } catch (err) {
        console.error('Error rendering all products:', err);
        grid.innerHTML = `<div class="col-span-full text-center py-12"><p class="text-on-surface-variant">Failed to load products.</p></div>`;
    }
}

function updatePaginationIndicators(totalPages) {
    const indicators = document.getElementById('prods-page-indicators');
    if (!indicators) return;
    let html = '';
    const maxVisible = 5;
    let start = Math.max(1, prodsPage - Math.floor(maxVisible / 2));
    let end = Math.min(totalPages, start + maxVisible - 1);
    if (end - start + 1 < maxVisible) start = Math.max(1, end - maxVisible + 1);
    for (let i = start; i <= end; i++) {
        html += `<button onclick="goToProdsPage(${i})" class="w-9 h-9 rounded-full text-xs font-bold transition-all duration-200 ${i === prodsPage ? 'bg-primary text-white shadow-md' : 'bg-white dark:bg-slate-800 text-primary border border-primary/20 hover:bg-primary/10'}">${i}</button>`;
    }
    indicators.innerHTML = html;
    const prevBtn = document.getElementById('btn-prod-prev');
    const nextBtn = document.getElementById('btn-prod-next');
    if (prevBtn) prevBtn.disabled = prodsPage <= 1;
    if (nextBtn) nextBtn.disabled = prodsPage >= totalPages;
}

function updateActiveFilterPill() {
    const catFilters = document.getElementById('prod-cat-filters');
    if (!catFilters) return;
    const key = selectedCategoryId || 'all';
    catFilters.querySelectorAll('button').forEach(btn => {
        const isSelected = btn.id === `cat-filter-${key}`;
        btn.className = isSelected
            ? 'cat-pill px-4 py-2 rounded-full border text-xs font-bold bg-primary border-primary text-white'
            : 'cat-pill px-4 py-2 rounded-full border border-primary/20 text-primary hover:border-primary text-xs font-bold bg-white/50 dark:bg-slate-800/50';
    });
}

function filterCategory(catId) {
    selectedCategoryId = catId;
    prodsPage = 1;
    renderAllProducts();
}

function filterPrice(val) {
    maxPrice = parseInt(val);
    prodsPage = 1;
    renderAllProducts();
}

function prevProdsPage() {
    if (prodsPage > 1) {
        prodsPage--;
        renderAllProducts();
        document.getElementById('all-products').scrollIntoView({ behavior: 'smooth' });
    }
}

function nextProdsPage() {
    prodsPage++;
    renderAllProducts();
    document.getElementById('all-products').scrollIntoView({ behavior: 'smooth' });
}

function goToProdsPage(page) {
    prodsPage = page;
    renderAllProducts();
    document.getElementById('all-products').scrollIntoView({ behavior: 'smooth' });
}

async function initPriceSlider() {
    const range = await fetchPriceRange();
    priceMin = range.minPrice;
    priceMax = range.maxPrice || 200;
    maxPrice = priceMax;
    const slider = document.getElementById('price-slider');
    const display = document.getElementById('price-val-display');
    if (slider) {
        slider.min = priceMin;
        slider.max = priceMax;
        slider.value = priceMax;
    }
    if (display) {
        const isAr = document.documentElement.lang.startsWith('ar');
        display.textContent = isAr ? 'الكل' : 'All';
    }
    document.querySelectorAll('#price-slider + div span').forEach((el, i) => {
        if (i === 0) el.textContent = priceMin + ' EGP';
        if (i === 1) el.textContent = priceMax + ' EGP';
    });
}

window.filterCategory = filterCategory;
window.filterPrice = filterPrice;
window.prevProdsPage = prevProdsPage;
window.nextProdsPage = nextProdsPage;
window.goToProdsPage = goToProdsPage;
window.renderAllProducts = renderAllProducts;

/* ── Sign-in Modal ─────────────────────────────── */
function showSignInModal() {
    const modal = document.getElementById('signin-modal');
    if (modal) {
        modal.classList.remove('hidden');
        document.body.style.overflow = 'hidden';
    }
}

function hideSignInModal() {
    const modal = document.getElementById('signin-modal');
    if (modal) {
        modal.classList.add('hidden');
        document.body.style.overflow = '';
    }
}

function initFrozenActions() {
    // Delegated click listener for frozen <a> tags (both static & dynamic)
    document.addEventListener('click', function(e) {
        const frozen = e.target.closest('a[data-frozen-action]');
        if (frozen) {
            e.preventDefault();
            showSignInModal();
        }
    });

    // Override category filter (All Products)
    filterCategory = function() { showSignInModal(); };

    // Override price filter
    filterPrice = function() { showSignInModal(); };

    // Override pagination
    prevProdsPage = function() { showSignInModal(); };
    nextProdsPage = function() { showSignInModal(); };
    goToProdsPage = function() { showSignInModal(); };

    // Intercept contact form submit
    const contactForm = document.querySelector('#contact form');
    if (contactForm) {
        contactForm.addEventListener('submit', function(e) {
            e.preventDefault();
            showSignInModal();
        });
    }

    // Modal close on backdrop click
    document.getElementById('signin-modal')?.addEventListener('click', function(e) {
        if (e.target === this) hideSignInModal();
    });

    // Modal close on Escape key
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape') hideSignInModal();
    });
}

function initDelegatedAddToCart() {
    document.addEventListener('click', function(e) {
        const btn = e.target.closest('[data-product-add]');
        if (!btn) return;
        // Skip if button has inline onclick (already handled during target phase)
        if (btn.hasAttribute('onclick')) return;
        const id = btn.getAttribute('data-product-add');
        const name = btn.getAttribute('data-product-name');
        const price = btn.getAttribute('data-product-price');
        const image = btn.getAttribute('data-product-image') || '';
        guestAddToCart(btn, id, name, price, image);
    });
}

window.showSignInModal = showSignInModal;
window.hideSignInModal = hideSignInModal;
