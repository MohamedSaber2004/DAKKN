let currentLang = 'ar';
let currentTranslations = {};

async function fetchTranslations(lang) {
    try {
        const response = await fetch(`/api/v1/translations?lang=${lang}`);
        if (!response.ok) {
            throw new Error(`Failed to load translation file: ${response.statusText}`);
        }
        return await response.json();
    } catch (error) {
        console.error('Error fetching translation:', error);
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

async function applyLang(lang, force = false) {
    const root = document.getElementById('html-root');
    const isAlreadyCorrect = root && root.getAttribute('lang') === lang;

    currentLang = lang;
    const s = await fetchTranslations(lang);
    if (!s) return;

    if (root) {
        root.setAttribute('lang', s.lang);
        root.setAttribute('dir', s.dir);
    }

    // Only walk the DOM if the language changed or if forced
    if (force || !isAlreadyCorrect) {
        document.querySelectorAll('[data-i18n]').forEach(el => {
            const key = el.getAttribute('data-i18n');
            const translation = s[key] ?? key;
            const target = el.getAttribute('data-i18n-target');

            if (target === 'title') {
                document.title = translation;
            } else if (target === 'description') {
                const meta = document.querySelector('meta[name="description"]');
                if (meta) meta.setAttribute('content', translation);
            }

            if (el.tagName === 'INPUT' || el.tagName === 'TEXTAREA') {
                el.placeholder = translation;
            } else {
                el.textContent = translation;
            }
        });

        document.querySelectorAll('[data-i18n-html]').forEach(el => {
            const key = el.getAttribute('data-i18n-html');
            el.innerHTML = s[key] ?? key;
        });

        const langToggleBtn = document.getElementById('lang-toggle-btn');
        if (langToggleBtn) {
            langToggleBtn.textContent = s.lang_toggle;
        }
    }

    localStorage.setItem('dakkn_lang', lang);
    // Set ASP.NET Core Culture Cookie: c=ar|uic=ar
    setCookie(".AspNetCore.Culture", `c=${lang}|uic=${lang}`, 365);

    currentTranslations = s;
    if (typeof renderAllProducts === 'function') {
        renderAllProducts();
    }

    if (typeof updateSlider === 'function') {
        setTimeout(updateSlider, 50);
    }
}

function toggleLang() {
    applyLang(currentLang === 'en' ? 'ar' : 'en', true);
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

    // Use native scrollIntoView with smooth behavior. 
    // It respects the scroll-margin-top defined in landing.css for sections.
    target.scrollIntoView({ behavior: 'smooth' });

    if (updateHistory) {
        history.pushState(null, '', `#${targetId}`);
    }
}

/* ── Scroll Animation Engine ─────────────────────────────── */
function initScrollAnimations() {
    const animated = document.querySelectorAll(
        '.scroll-animate, .scroll-animate-left, .scroll-animate-right, .scroll-animate-scale'
    );

    if (!animated.length) return;

    const observer = new IntersectionObserver(
        (entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('visible');
                    observer.unobserve(entry.target);
                }
            });
        },
        {
            threshold: 0.12,
            rootMargin: '0px 0px -40px 0px'
        }
    );

    animated.forEach(el => observer.observe(el));
}

/* ── Stagger grid children ───────────────────────────────── */
function initStaggeredGrids() {
    document.querySelectorAll('[data-stagger]').forEach(grid => {
        grid.querySelectorAll(':scope > *').forEach((child, i) => {
            child.style.transitionDelay = `${i * 0.1}s`;
        });
    });
}

/* ── Navbar scroll-shrink effect ─────────────────────────── */
function initNavbarScrollEffect() {
    const navbar = document.querySelector('.navbar-animate');
    if (!navbar) return;

    window.addEventListener('scroll', () => {
        if (window.scrollY > 60) {
            navbar.classList.add('navbar-scrolled');
        } else {
            navbar.classList.remove('navbar-scrolled');
        }
    }, { passive: true });
}

/* ── Scroll Spy (active nav on scroll) ───────────────────── */
function initScrollSpy() {
    const sections = Array.from(document.querySelectorAll('section[id]'));
    const navLinks = document.querySelectorAll('[data-section]');

    if (!sections.length || !navLinks.length) return;

    const sectionMapping = {
        'hero': 'shop',
        'shop': 'shop',
        'about': 'shop',
        'categories': 'categories',
        'all-products': 'all-products',
        'testimonials': 'testimonials',
        'contact': 'contact'
    };

    const onScroll = () => {
        const headerHeight = getHeaderOffset();
        const scrollPos = window.scrollY + headerHeight + 100;

        // 1. Bottom of page check (only if not at the very top)
        const isBottom = (window.innerHeight + window.scrollY) >= document.body.offsetHeight - 50;
        if (window.scrollY > 100 && isBottom) {
            setActive(sections[sections.length - 1].id);
            return;
        }

        // 2. Find current section
        let currentSectionId = sections[0].id;
        for (const section of sections) {
            if (scrollPos >= section.offsetTop) {
                currentSectionId = section.id;
            } else {
                break;
            }
        }

        setActive(currentSectionId);
    };

    function setActive(id) {
        const activeLinkId = sectionMapping[id] || id;

        navLinks.forEach(link => {
            if (link.getAttribute('data-section') === activeLinkId) {
                link.classList.add('active');
            } else {
                link.classList.remove('active');
            }
        });
    }

    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll(); // Initial check
}
/* ── Boot ────────────────────────────────────────────────── */
document.addEventListener('DOMContentLoaded', () => {
    // Priority: Cookie -> LocalStorage -> Default 'ar'
    const cookieValue = getCookie(".AspNetCore.Culture");
    let saved = 'ar';

    if (cookieValue) {
        // Extract 'ar' or 'en' from 'c=ar|uic=ar'
        const match = cookieValue.match(/c=([a-z]{2})/);
        if (match) {
            saved = match[1];
        } else {
            saved = localStorage.getItem('dakkn_lang') ?? 'ar';
        }
    } else {
        saved = localStorage.getItem('dakkn_lang') ?? 'ar';
    }

    applyLang(saved);

    initScrollAnimations();
    initStaggeredGrids();
    initNavbarScrollEffect();
    initScrollSpy();

    // Attach click handlers for all in-page anchors
    document.querySelectorAll('a[href^="#"]').forEach(link => {
        const href = link.getAttribute('href');
        if (!href || href === '#') return;
        
        link.addEventListener('click', (e) => {
            const targetId = href.substring(1);
            if (!document.getElementById(targetId)) return;
            
            e.preventDefault();
            
            // Reverting to standard navigation: scroll directly to the relevant section
            scrollToHash(href);
        });
    });

    if (window.location.hash) {
        setTimeout(() => scrollToHash(window.location.hash, false), 150);
    }

    // Initialize Slider
    if (typeof updateSlider === 'function') {
        updateSlider();
    }

    // Initialize All Products Grid
    if (typeof renderAllProducts === 'function') {
        renderAllProducts();
    }
});

/* ── Categories Slider Engine ───────────────────────────── */
let currentSlideIndex = 0;

function getSlideLimit() {
    const track = document.getElementById('cat-slider-track');
    if (!track) return 0;
    const slides = track.children.length;
    let visible = 1;
    if (window.innerWidth >= 1024) visible = 4;
    else if (window.innerWidth >= 768) visible = 3;
    else if (window.innerWidth >= 640) visible = 2;
    
    return Math.max(0, slides - visible);
}

function updateSlider() {
    const track = document.getElementById('cat-slider-track');
    if (!track) return;
    
    const limit = getSlideLimit();
    if (currentSlideIndex > limit) {
        currentSlideIndex = limit;
    }
    if (currentSlideIndex < 0) {
        currentSlideIndex = 0;
    }
    
    const firstSlide = track.firstElementChild;
    if (!firstSlide) return;
    
    const slideWidth = firstSlide.getBoundingClientRect().width;
    const gap = 24; // gap-6
    
    const isRtl = document.getElementById('html-root')?.getAttribute('dir') === 'rtl';
    const multiplier = isRtl ? 1 : -1;
    
    const offset = currentSlideIndex * (slideWidth + gap) * multiplier;
    track.style.transform = `translateX(${offset}px)`;
    
    // Update pagination dots
    for (let i = 0; i <= 2; i++) {
        const dot = document.getElementById(`cat-dot-${i}`);
        if (dot) {
            const trackItems = track.children.length;
            const targetDot = Math.min(2, Math.floor(currentSlideIndex * 3 / (trackItems - 1)));
            if (i === targetDot) {
                dot.classList.remove('bg-primary/30');
                dot.classList.add('bg-primary');
            } else {
                dot.classList.remove('bg-primary');
                dot.classList.add('bg-primary/30');
            }
        }
    }
}

function slideNext() {
    const limit = getSlideLimit();
    if (currentSlideIndex < limit) {
        currentSlideIndex++;
    } else {
        currentSlideIndex = 0; // wrap around
    }
    updateSlider();
}

// Ensure global accessibility for inline HTML onclick attributes
function slidePrev() {
    if (currentSlideIndex > 0) {
        currentSlideIndex--;
    } else {
        currentSlideIndex = getSlideLimit(); // wrap around
    }
    updateSlider();
}

function slideGoTo(index) {
    const limit = getSlideLimit();
    currentSlideIndex = Math.round((index / 2) * limit);
    updateSlider();
}

window.slideNext = slideNext;
window.slidePrev = slidePrev;
window.slideGoTo = slideGoTo;
window.updateSlider = updateSlider;

window.addEventListener('resize', updateSlider);

/* ── Browse All Products Filtering & Pagination ────────── */
const ALL_PRODUCTS = [
    { id: 1, nameKey: "prod1_name", category: "Die-Cut", categoryKey: "cat1", price: 70, img: "https://lh3.googleusercontent.com/aida-public/AB6AXuBzltvWG8WURo7Y4suxD28ZKqrZqGo_jT9MfzPzO7YJy159K35OA3WcmauA2spMG7cSXjHDhIBERG4el4QITFH87cqD8aMULaFRF6R8fJR1-3Ca-RKAss7q3wjlw1wBwI3rFiciRCrPiyATA55LfV9EKWbH2U3sOeu7UJAl8IDPaLRei3JCXrYPz3r5ZhxVc0UtwbuT7RVAGaG2RC-eLlRnM81bQT6Eh1bbP7SQsT1UJJQ0cAhzAaTxkwWgpmYmBUSrtGFIZzFwZGoi" },
    { id: 2, nameKey: "prod2_name", category: "Holographic", categoryKey: "cat2", price: 80, img: "https://lh3.googleusercontent.com/aida-public/AB6AXuDNlKihBsHggV1O8npzxCBXgegvVuI9C4X9A-R-pMkpZxfd9ETgPzw3yV8Y7K3xqS2YMYggvApzgWiC98n2TQ2pkEDVuIHFZk8cdkGoU2Dzg0p-YDxsbRUkr7iogDacxpKtjBwNN8TgcAAQjXmr_2WPuLmhYP5h7kUYXO2hciCSdgSRCl8IObo8nbGIoQBgQFJobpxX69IGcm6UVeiu02pY0d5nSJZtH1JrJd_y0GescOC89BTPWnxp-4oBlohshrnihq2NDtrjYqPR" },
    { id: 3, nameKey: "prod3_name", category: "Clear", categoryKey: "cat3", price: 60, img: "https://lh3.googleusercontent.com/aida-public/AB6AXuAXD4f3g-Xcj3q_tUbHslHRvBGQQPqNJQo25aoOQpYJoAkNvsN195Xr69-P45FG6MuYFa9KKf_wUbKQqLgDlsy1sU972rS_ZK7HtRKvkl8I9A5ISg24pUCXPNvW0tl93POppw80pkH_AeNnI-tIE5PvfS67qcqQroSEGYsxRDcktuS0aiGkoOOxaPLQ5tXSwNn_JqXw3Ken-xEZM-KtUYI6B_jVITKI3yQKJM8Yk-CNFye4fDZcWCf1VN6AeXyte_x6yygWA3xobXz5" },
    { id: 4, nameKey: "prod4_name", category: "Die-Cut", categoryKey: "cat1", price: 70, img: "https://lh3.googleusercontent.com/aida-public/AB6AXuBNEuLIza1T-3Iuc-68bHHLc2pPetxBeuxUnxKpQ6IRTTI82ybqG-q5I9BAWIeOsJ9YBhCLD2DNvoBL2TX8rup2TWluPf4rAjhxdSShOt8fSWMzrUleduQUNPviusk5FHo6g-N4FpR_58KIT9_qchh-raYGS_H-GhrFQ-sgF5lc_PrfRgsuyGdd9q8_CATQniccrhh87XNV1MTwun7KOCcEmJboJnnpEmtKm8asHBRe94TFFUESjJL-f0S50d37pS09FasbtTA3VAPe" },
    { id: 5, nameKey: "prod5_name", category: "Custom Labels", categoryKey: "cat5", price: 120, img: "https://lh3.googleusercontent.com/aida-public/AB6AXuBzltvWG8WURo7Y4suxD28ZKqrZqGo_jT9MfzPzO7YJy159K35OA3WcmauA2spMG7cSXjHDhIBERG4el4QITFH87cqD8aMULaFRF6R8fJR1-3Ca-RKAss7q3wjlw1wBwI3rFiciRCrPiyATA55LfV9EKWbH2U3sOeu7UJAl8IDPaLRei3JCXrYPz3r5ZhxVc0UtwbuT7RVAGaG2RC-eLlRnM81bQT6Eh1bbP7SQsT1UJJQ0cAhzAaTxkwWgpmYmBUSrtGFIZzFwZGoi" },
    { id: 6, nameKey: "prod6_name", category: "Holographic", categoryKey: "cat2", price: 95, img: "https://lh3.googleusercontent.com/aida-public/AB6AXuDNlKihBsHggV1O8npzxCBXgegvVuI9C4X9A-R-pMkpZxfd9ETgPzw3yV8Y7K3xqS2YMYggvApzgWiC98n2TQ2pkEDVuIHFZk8cdkGoU2Dzg0p-YDxsbRUkr7iogDacxpKtjBwNN8TgcAAQjXmr_2WPuLmhYP5h7kUYXO2hciCSdgSRCl8IObo8nbGIoQBgQFJobpxX69IGcm6UVeiu02pY0d5nSJZtH1JrJd_y0GescOC89BTPWnxp-4oBlohshrnihq2NDtrjYqPR" },
    { id: 7, nameKey: "prod7_name", category: "Clear", categoryKey: "cat3", price: 50, img: "https://lh3.googleusercontent.com/aida-public/AB6AXuAXD4f3g-Xcj3q_tUbHslHRvBGQQPqNJQo25aoOQpYJoAkNvsN195Xr69-P45FG6MuYFa9KKf_wUbKQqLgDlsy1sU972rS_ZK7HtRKvkl8I9A5ISg24pUCXPNvW0tl93POppw80pkH_AeNnI-tIE5PvfS67qcqQroSEGYsxRDcktuS0aiGkoOOxaPLQ5tXSwNn_JqXw3Ken-xEZM-KtUYI6B_jVITKI3yQKJM8Yk-CNFye4fDZcWCf1VN6AeXyte_x6yygWA3xobXz5" },
    { id: 8, nameKey: "prod8_name", category: "Sticker Sheets", categoryKey: "cat4", price: 110, img: "https://lh3.googleusercontent.com/aida-public/AB6AXuBJDMScKPq8MJUOklieS9wyBeBolpgZgCezPLy5vxVSfLlZwafOkDMZcTXqanKwSbFFMgETfofN0f0Z3PPVvyA1qXwoqVbzrm3cUyBoWSfHJNsnxf83OEz_N6XWfdgx1j_CKPnEvaDHmQ3-UEQrqJqmPhtF6p4hySOo3H8IrSxJpVe0omQj6ZEYGEcptT3bs84jGWIWvNYTod4_9LUv4Br9s3Mo0kZYUle3La_hd77AzNcmDbQZXxtuVqUGni9yAaljCyssQ3T884AU" },
    { id: 9, nameKey: "prod9_name", category: "Matte Vinyl", categoryKey: "cat6", price: 85, img: "https://lh3.googleusercontent.com/aida-public/AB6AXuBNEuLIza1T-3Iuc-68bHHLc2pPetxBeuxUnxKpQ6IRTTI82ybqG-q5I9BAWIeOsJ9YBhCLD2DNvoBL2TX8rup2TWluPf4rAjhxdSShOt8fSWMzrUleduQUNPviusk5FHo6g-N4FpR_58KIT9_qchh-raYGS_H-GhrFQ-sgF5lc_PrfRgsuyGdd9q8_CATQniccrhh87XNV1MTwun7KOCcEmJboJnnpEmtKm8asHBRe94TFFUESjJL-f0S50d37pS09FasbtTA3VAPe" },
    { id: 10, nameKey: "prod10_name", category: "Die-Cut", categoryKey: "cat1", price: 75, img: "https://lh3.googleusercontent.com/aida-public/AB6AXuAk_qLkXMyRhZHsFCiX42KXytRcQjX2nab3tojgzqftF18C49FJpCS5XBGoaBsnX75L1gyYlftOBl7WKTmtACsTCrr806OId-nyvJx3XDZ1w-v8x5Qm_GT3GqS4OKn2MFeB83w4XvOMsFFzZxFXuQ8oFPLgpySI6MxTeuJpXw6YP_zc4_64RRwlk0pmeKbcMA2D6nh67sbI8UVcw84LEEpeaE0L3-YyDqG3MvrVsCw0ZjDKcxm043x28Rzxo5kX03VP42LKDk4lLeA2" },
    { id: 11, nameKey: "prod11_name", category: "Sticker Sheets", categoryKey: "cat4", price: 105, img: "https://lh3.googleusercontent.com/aida-public/AB6AXuBJDMScKPq8MJUOklieS9wyBeBolpgZgCezPLy5vxVSfLlZwafOkDMZcTXqanKwSbFFMgETfofN0f0Z3PPVvyA1qXwoqVbzrm3cUyBoWSfHJNsnxf83OEz_N6XWfdgx1j_CKPnEvaDHmQ3-UEQrqJqmPhtF6p4hySOo3H8IrSxJpVe0omQj6ZEYGEcptT3bs84jGWIWvNYTod4_9LUv4Br9s3Mo0kZYUle3La_hd77AzNcmDbQZXxtuVqUGni9yAaljCyssQ3T884AU" },
    { id: 12, nameKey: "prod12_name", category: "Holographic", categoryKey: "cat2", price: 90, img: "https://lh3.googleusercontent.com/aida-public/AB6AXuC7R0HC8qN2XvoVeNE2ALpluQWlRv1gqsRoZU694CuR3U-dd_9BcWYyazCSb8t36Ueu4AMVfnYtCF7ibe4X76KrHDlZT3reA_YXSTkFfjqGL8GFRd-0Lhg6lf7026uYYjkidJ1ee2eCGL1ntOmk7LT1iwgtgGxO2q0EIj0lskO7TRjr-x7JOKyDhU-8hBAWjDPQSOpFV90-R6H7F9udXZw6fG5DQQxbsnf9Xau_C9lXJlhk3LZcnb7kakWDPtTqFCx2BYRNZhvn4WxE" }
];

let selectedCategory = 'all';
let maxPrice = 150;
let prodsPage = 1;
const itemsPerPage = 8;

function renderAllProducts() {
    const grid = document.getElementById('all-prods-grid');
    if (!grid) return;

    // Filter products
    const filtered = ALL_PRODUCTS.filter(p => {
        const matchesCat = (selectedCategory === 'all' || p.category === selectedCategory);
        const matchesPrice = (p.price <= maxPrice);
        return matchesCat && matchesPrice;
    });

    // Pagination calculations
    const totalItems = filtered.length;
    const totalPages = Math.ceil(totalItems / itemsPerPage) || 1;
    if (prodsPage > totalPages) prodsPage = totalPages;
    if (prodsPage < 1) prodsPage = 1;

    const startIdx = (prodsPage - 1) * itemsPerPage;
    const endIdx = startIdx + itemsPerPage;
    const paginated = filtered.slice(startIdx, endIdx);

    // Build HTML
    if (paginated.length === 0) {
        grid.innerHTML = `
            <div class="col-span-full flex flex-col items-center justify-center py-12 text-on-surface-variant/70 gap-3">
                <span class="material-symbols-outlined text-4xl text-primary/40">sentiment_dissatisfied</span>
                <p class="font-bold text-sm text-center">${currentTranslations.no_products_found || "No stickers match your filters."}</p>
            </div>
        `;
    } else {
        grid.innerHTML = paginated.map(p => {
            const localizedName = currentTranslations[p.nameKey] || p.nameKey;
            const localizedCategory = currentTranslations[p.categoryKey] || p.category;
            const localizedPrice = currentLang === 'ar' 
                ? `${p.price} ج.م` 
                : `${p.price} EGP`;
            const localizedAddCart = currentTranslations.add_cart || "Add to cart";

            return `
                <div class="product-card glass-panel p-5 rounded-2xl flex flex-col gap-4 scroll-animate visible" style="opacity: 1; transform: none;">
                    <div class="aspect-square rounded-xl overflow-hidden relative bg-surface-container-high/40">
                        <img class="w-full h-full object-cover" src="${p.img}" alt="${localizedName}"/>
                    </div>
                    <div>
                        <h3 class="text-lg font-bold text-on-surface">${localizedName}</h3>
                        <p class="text-xs text-on-surface-variant">${localizedCategory}</p>
                    </div>
                    <div class="flex items-center justify-between mt-auto pt-2">
                        <span class="text-xl font-bold text-primary font-sans">${localizedPrice}</span>
                        <button onclick="alert('Added to cart / تم الإضافة للسلة')" class="px-4 py-2 rounded-lg bg-primary/10 text-primary hover:bg-primary hover:text-on-primary text-xs font-bold transition-all duration-200">
                            ${localizedAddCart}
                        </button>
                    </div>
                </div>
            `;
        }).join('');
    }

    // Update Price slider label
    const priceDisplay = document.getElementById('price-val-display');
    if (priceDisplay) {
        priceDisplay.textContent = currentLang === 'ar' 
            ? `${maxPrice} ج.م` 
            : `${maxPrice} EGP`;
    }

    // Update Category Pill styles
    const catFilters = document.getElementById('prod-cat-filters');
    if (catFilters) {
        catFilters.querySelectorAll('button').forEach(btn => {
            const isSelected = btn.id === `cat-filter-${selectedCategory.replace(' ', '-')}`;
            if (isSelected) {
                btn.className = 'cat-pill px-4 py-2 rounded-full border text-xs font-bold transition-all duration-200 bg-primary border-primary text-white';
            } else {
                btn.className = 'cat-pill px-4 py-2 rounded-full border border-primary/20 text-primary hover:border-primary text-xs font-bold transition-all duration-200 bg-white/50';
            }
        });
    }

    // Render Pagination Indicators
    const pageIndicators = document.getElementById('prods-page-indicators');
    if (pageIndicators) {
        let indicatorsHtml = '';
        for (let i = 1; i <= totalPages; i++) {
            const isActive = i === prodsPage;
            indicatorsHtml += `
                <button onclick="goToProdsPage(${i})" class="w-8 h-8 rounded-full border text-xs font-bold transition-all duration-200 flex items-center justify-center font-sans ${
                    isActive 
                        ? 'bg-primary border-primary text-white' 
                        : 'border-primary/20 text-primary hover:border-primary bg-white'
                }">
                    ${i}
                </button>
            `;
        }
        pageIndicators.innerHTML = indicatorsHtml;
    }

    // Disable/Enable Nav buttons
    const btnPrev = document.getElementById('btn-prod-prev');
    const btnNext = document.getElementById('btn-prod-next');
    if (btnPrev) btnPrev.disabled = (prodsPage === 1);
    if (btnNext) btnNext.disabled = (prodsPage === totalPages);
}

function filterCategory(cat) {
    selectedCategory = cat;
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

// Exports
window.filterCategory = filterCategory;
window.filterPrice = filterPrice;
window.prevProdsPage = prevProdsPage;
window.nextProdsPage = nextProdsPage;
window.goToProdsPage = goToProdsPage;
window.renderAllProducts = renderAllProducts;
