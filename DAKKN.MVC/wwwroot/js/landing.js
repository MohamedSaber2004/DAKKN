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
        if (typeof renderAllProducts === 'function') renderAllProducts();
    } else if (!Object.keys(currentTranslations).length) {
        const s = await fetchTranslations(lang || current);
        if (s) {
            currentTranslations = s;
            if (typeof renderAllProducts === 'function') renderAllProducts();
        }
    }
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
        safeInit('updateSlider');
        safeInit('renderAllProducts');

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
    window.addEventListener('scroll', () => {
        if (window.scrollY > 60) navbar.classList.add('navbar-scrolled');
        else navbar.classList.remove('navbar-scrolled');
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
window.addEventListener('resize', updateSlider);

/* ── Products Grid ─────────────────────────────────────── */
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
    const filtered = ALL_PRODUCTS.filter(p => (selectedCategory === 'all' || p.category === selectedCategory) && p.price <= maxPrice);
    const totalPages = Math.ceil(filtered.length / itemsPerPage) || 1;
    if (prodsPage > totalPages) prodsPage = totalPages;
    const paginated = filtered.slice((prodsPage - 1) * itemsPerPage, prodsPage * itemsPerPage);
    if (!paginated.length) { grid.innerHTML = `<div class="col-span-full py-12 text-center text-on-surface-variant/70">${currentTranslations.no_products_found || "No stickers match your filters."}</div>`; }
    else {
        grid.innerHTML = paginated.map(p => `
            <div class="product-card glass-panel p-5 rounded-2xl flex flex-col gap-4 scroll-animate visible" style="opacity:1;transform:none;">
                <div class="aspect-square rounded-xl overflow-hidden relative"><img class="w-full h-full object-cover" src="${p.img}" alt="${currentTranslations[p.nameKey] || p.nameKey}"/></div>
                <div><h3 class="text-lg font-bold text-on-surface">${currentTranslations[p.nameKey] || p.nameKey}</h3><p class="text-xs text-on-surface-variant">${currentTranslations[p.categoryKey] || p.category}</p></div>
                <div class="flex items-center justify-between mt-auto pt-2"><span class="text-xl font-bold text-primary font-sans">${document.documentElement.lang === 'ar' ? p.price + ' ج.م' : p.price + ' EGP'}</span><button onclick="alert('Added!')" class="px-4 py-2 rounded-lg bg-primary/10 text-primary hover:bg-primary hover:text-on-primary text-xs font-bold transition-all duration-200">${currentTranslations.add_cart || 'Add'}</button></div>
            </div>`).join('');
    }
    const priceDisplay = document.getElementById('price-val-display');
    if (priceDisplay) priceDisplay.textContent = document.documentElement.lang === 'ar' ? maxPrice + ' ج.م' : maxPrice + ' EGP';
    const catFilters = document.getElementById('prod-cat-filters');
    if (catFilters) catFilters.querySelectorAll('button').forEach(btn => {
        const isSelected = btn.id === `cat-filter-${selectedCategory.replace(' ', '-')}`;
        btn.className = isSelected ? 'cat-pill px-4 py-2 rounded-full border text-xs font-bold bg-primary border-primary text-white' : 'cat-pill px-4 py-2 rounded-full border border-primary/20 text-primary hover:border-primary text-xs font-bold bg-white/50';
    });
}
function filterCategory(cat) { selectedCategory = cat; prodsPage = 1; renderAllProducts(); }
function filterPrice(val) { maxPrice = parseInt(val); prodsPage = 1; renderAllProducts(); }
function prevProdsPage() { if (prodsPage > 1) { prodsPage--; renderAllProducts(); document.getElementById('all-products').scrollIntoView({ behavior: 'smooth' }); } }
function nextProdsPage() { prodsPage++; renderAllProducts(); document.getElementById('all-products').scrollIntoView({ behavior: 'smooth' }); }
function goToProdsPage(page) { prodsPage = page; renderAllProducts(); document.getElementById('all-products').scrollIntoView({ behavior: 'smooth' }); }

window.filterCategory = filterCategory;
window.filterPrice = filterPrice;
window.prevProdsPage = prevProdsPage;
window.nextProdsPage = nextProdsPage;
window.goToProdsPage = goToProdsPage;
window.renderAllProducts = renderAllProducts;
