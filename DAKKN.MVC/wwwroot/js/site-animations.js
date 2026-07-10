/* ==========================================================================
   DAKKN — Universal Site Animations (site-animations.js)
   Lightweight, dependency-free animation engine for every page.
   ========================================================================== */

(function () {
  'use strict';

  /* ── 1. Config ─────────────────────────────────────────────────────── */
  const CONFIG = {
    revealThreshold: 0.12,
    staggerDefault: 80,
    countDuration: 1500,
    scrollDebounce: 100,
  };

  /* ── 2. Page Entrance & Exit Transitions ───────────────────────────── */
  function initPageEntrance() {
    const target = document.querySelector(
      '#admin-canvas, main, [data-page-entrance]'
    );
    if (!target) return;
    if (window.dakknBfCached) return;

    // Apply high-end modern transition properties (smooth scale, translate, and fade)
    target.style.opacity = '0';
    target.style.transform = 'scale(0.985) translateY(16px)';
    target.style.transition =
      'opacity 0.6s cubic-bezier(0.16, 1, 0.3, 1), transform 0.6s cubic-bezier(0.16, 1, 0.3, 1)';
    
    // Trigger transition in the next frame
    requestAnimationFrame(() => {
      target.style.opacity = '1';
      target.style.transform = 'scale(1) translateY(0)';
      
      // Cleanup inline styles after transition to prevent layout/interaction bugs
      setTimeout(() => {
        if (target.style.opacity === '1') {
          target.style.removeProperty('opacity');
          target.style.removeProperty('transform');
          target.style.removeProperty('transition');
        }
      }, 600);
    });
  }

  function initPageExit() {
    const target = document.querySelector(
      '#admin-canvas, main, [data-page-entrance]'
    );
    if (!target) return;

    document.addEventListener('click', (e) => {
      // Skip if another handler already prevented navigation
      if (e.defaultPrevented) return;

      // Find the closest anchor tag
      const link = e.target.closest('a');
      if (!link) return;

      const href = link.getAttribute('href');
      
      // Skip exit transition if:
      // - No href, empty or anchor links
      // - External links
      // - JS void/empty links
      // - File downloads or target="_blank"
      // - Custom data-no-transition attribute
      if (!href || 
          href.startsWith('#') || 
          href.startsWith('javascript:') || 
          link.getAttribute('target') === '_blank' || 
          link.hasAttribute('download') ||
          link.hasAttribute('data-no-transition') ||
          (!href.startsWith('/') && !href.startsWith(window.location.origin))) {
        return;
      }

      // Skip if it's the exact same URL but just with hash
      try {
        const targetUrl = new URL(link.href);
        if (targetUrl.pathname === window.location.pathname && 
            targetUrl.search === window.location.search && 
            targetUrl.hash) {
          return;
        }
      } catch (err) {}

      e.preventDefault();

      // Trigger exit transition (fade out and translate up slightly)
      target.style.transition = 'opacity 0.35s cubic-bezier(0.16, 1, 0.3, 1), transform 0.35s cubic-bezier(0.16, 1, 0.3, 1)';
      target.style.opacity = '0';
      target.style.transform = 'scale(0.985) translateY(-16px)';

      // Navigate after transition completes
      setTimeout(() => {
        window.location.href = href;
      }, 300);
    });
  }

  /* ── 3. Scroll Reveal (IntersectionObserver) ───────────────────────── */
  function initScrollReveal() {
    const els = document.querySelectorAll(
      '.animate-reveal, [data-reveal-up], [data-reveal-left], [data-reveal-right], [data-reveal-scale]'
    );
    if (!els.length) return;

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            const el = entry.target;
            const dir = el.dataset.revealUp
              ? 'up'
              : el.dataset.revealLeft
                ? 'left'
                : el.dataset.revealRight
                  ? 'right'
                  : el.dataset.revealScale
                    ? 'scale'
                    : 'up';
            const dist = el.dataset.revealDist || '30px';
            const delay = parseInt(el.dataset.revealDelay) || 0;

            let transform = '';
            if (dir === 'up') transform = `translateY(${dist})`;
            else if (dir === 'left') transform = `translateX(-${dist})`;
            else if (dir === 'right') transform = `translateX(${dist})`;
            else if (dir === 'scale') transform = 'scale(0.92)';

            el.style.opacity = '0';
            el.style.transform = transform;
            el.style.transition = `opacity 0.6s cubic-bezier(0.22, 1, 0.36, 1), transform 0.6s cubic-bezier(0.22, 1, 0.36, 1)`;
            el.style.transitionDelay = `${delay}ms`;

            requestAnimationFrame(() => {
              el.style.opacity = '1';
              el.style.transform = 'translate(0, 0) scale(1)';
            });

            observer.unobserve(el);
          }
        });
      },
      { threshold: CONFIG.revealThreshold }
    );

    els.forEach((el) => observer.observe(el));
  }

  /* ── 4. Staggered Children ─────────────────────────────────────────── */
  function initStagger() {
    const containers = document.querySelectorAll('[data-stagger]');
    containers.forEach((container) => {
      const children = container.children;
      if (!children.length) return;
      const baseDelay = parseInt(container.dataset.stagger) || CONFIG.staggerDefault;
      Array.from(children).forEach((child, i) => {
        child.style.opacity = '0';
        child.style.transform = 'translateY(16px)';
        child.style.transition =
          `opacity 0.5s ease, transform 0.5s cubic-bezier(0.22, 1, 0.36, 1)`;
        child.style.transitionDelay = `${i * baseDelay}ms`;
      });
      requestAnimationFrame(() => {
        Array.from(children).forEach((child) => {
          child.style.opacity = '1';
          child.style.transform = 'translateY(0)';
        });
      });
    });
  }

  /* ── 5. Count-Up Animation ─────────────────────────────────────────── */
  function initCounters() {
    const counters = document.querySelectorAll('[data-count-to]');
    if (!counters.length) return;

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            const el = entry.target;
            const target = parseFloat(el.dataset.countTo) || 0;
            const suffix = el.dataset.countSuffix || '';
            const prefix = el.dataset.countPrefix || '';
            const duration = parseInt(el.dataset.countDuration) || CONFIG.countDuration;
            const start = performance.now();

            function update(now) {
              const elapsed = now - start;
              const progress = Math.min(elapsed / duration, 1);
              const eased = 1 - Math.pow(1 - progress, 3);
              const current = eased * target;
              el.textContent =
                prefix +
                (Number.isInteger(target)
                  ? Math.round(current).toLocaleString()
                  : current.toFixed(1)) +
                suffix;
              if (progress < 1) requestAnimationFrame(update);
            }
            requestAnimationFrame(update);
            observer.unobserve(el);
          }
        });
      },
      { threshold: 0.3 }
    );

    counters.forEach((el) => observer.observe(el));
  }

  /* ── 6. Smooth Scroll for Anchors ──────────────────────────────────── */
  function initSmoothScroll() {
    document.querySelectorAll('a[href^="#"]').forEach((link) => {
      const href = link.getAttribute('href');
      if (!href || href === '#') return;
      link.addEventListener('click', (e) => {
        const target = document.getElementById(href.substring(1));
        if (!target) return;
        e.preventDefault();
        target.scrollIntoView({ behavior: 'smooth', block: 'start' });
      });
    });
  }

  /* ── 7. Hover Lift (3D card tilt) ─────────────────────────────────── */
  function initHoverLift() {
    document.querySelectorAll('[data-hover-lift], .product-card, .stats-card').forEach((card) => {
      if (card.dataset.hoverLiftInitialized) return;
      card.dataset.hoverLiftInitialized = 'true';

      card.addEventListener('mousemove', (e) => {
        const rect = card.getBoundingClientRect();
        const x = (e.clientX - rect.left) / rect.width - 0.5;
        const y = (e.clientY - rect.top) / rect.height - 0.5;
        // Premium 3D rotation and lift
        card.style.transform = `perspective(800px) rotateY(${x * 8}deg) rotateX(${-y * 8}deg) translateY(-6px) scale(1.01)`;
        card.style.transition = 'transform 0.1s cubic-bezier(0.25, 0.8, 0.25, 1)';
      });
      card.addEventListener('mouseleave', () => {
        card.style.transform =
          'perspective(800px) rotateY(0deg) rotateX(0deg) translateY(0) scale(1)';
        card.style.transition = 'transform 0.4s cubic-bezier(0.25, 0.8, 0.25, 1)';
      });
    });
  }

  /* ── 7b. Spotlight Glow Effect ────────────────────────────────────── */
  function initSpotlightGlow() {
    const cards = document.querySelectorAll('.product-card, .stats-card, [data-hover-glow], .bg-surface');
    cards.forEach((card) => {
      if (card.dataset.spotlightInitialized) return;
      card.dataset.spotlightInitialized = 'true';

      card.addEventListener('mousemove', (e) => {
        const rect = card.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        card.style.setProperty('--mouse-x', `${x}px`);
        card.style.setProperty('--mouse-y', `${y}px`);
      });
    });
  }

  /* ── 8. Image Fade-In on Load ──────────────────────────────────────── */
  function initImageFadeIn() {
    document.querySelectorAll('[data-fade-img]').forEach((img) => {
      img.style.opacity = '0';
      img.style.transition = 'opacity 0.5s ease';
      img.addEventListener('load', () => {
        img.style.opacity = '1';
      });
      if (img.complete) img.style.opacity = '1';
    });
  }

  /* ── 9. Back to Top Button ─────────────────────────────────────────── */
  function initBackToTop() {
    const btn = document.getElementById('back-to-top');
    if (!btn) return;
    const scrollable = document.querySelector(
      '#admin-canvas, main, [data-scroll-container]'
    ) || window;

    const toggle = () => {
      const scrollY =
        scrollable === window
          ? window.scrollY
          : scrollable.scrollTop;
      btn.classList.toggle('opacity-0', scrollY < 300);
      btn.classList.toggle('pointer-events-none', scrollY < 300);
    };

    scrollable.addEventListener('scroll', toggle, { passive: true });
    toggle();

    btn.addEventListener('click', () => {
      if (scrollable === window) {
        window.scrollTo({ top: 0, behavior: 'smooth' });
      } else {
        scrollable.scrollTo({ top: 0, behavior: 'smooth' });
      }
    });
  }

  /* ── 10. Button Ripple Effect ──────────────────────────────────────── */
  function initRipple() {
    document.querySelectorAll('[data-ripple]').forEach((btn) => {
      btn.style.position = 'relative';
      btn.style.overflow = 'hidden';
      btn.addEventListener('click', function (e) {
        const rect = this.getBoundingClientRect();
        const ripple = document.createElement('span');
        const size = Math.max(rect.width, rect.height);
        const x = e.clientX - rect.left - size / 2;
        const y = e.clientY - rect.top - size / 2;
        ripple.style.cssText = `
          position: absolute; width: ${size}px; height: ${size}px;
          left: ${x}px; top: ${y}px; border-radius: 50%;
          background: rgba(255,255,255,0.35);
          transform: scale(0); animation: dakknRipple 0.5s ease-out;
          pointer-events: none;
        `;
        this.appendChild(ripple);
        ripple.addEventListener('animationend', () => ripple.remove());
      });
    });
  }

  /* ── Inject Ripple Keyframe ────────────────────────────────────────── */
  (function injectRippleKeyframe() {
    if (document.getElementById('dakkn-ripple-style')) return;
    const style = document.createElement('style');
    style.id = 'dakkn-ripple-style';
    style.textContent = `
      @@keyframes dakknRipple {
        from { transform: scale(0); opacity: 1; }
        to { transform: scale(2.5); opacity: 0; }
      }
    `;
    document.head.appendChild(style);
  })();

  /* ── 11. Skeleton Loader Pulse ─────────────────────────────────────── */
  function initSkeletons() {
    document.querySelectorAll('[data-skeleton]').forEach((el) => {
      if (!el.classList.contains('skeleton')) {
        el.classList.add('skeleton');
      }
      // Remove skeleton when data loads (listen for custom event)
      el.addEventListener('data:loaded', () => {
        el.classList.remove('skeleton');
      });
    });
  }

  /* ── 12. Floating Element Gentle Animation ─────────────────────────── */
  function initFloatingElements() {
    document.querySelectorAll('[data-float]').forEach((el) => {
      const duration = el.dataset.floatDuration || '4s';
      const distance = el.dataset.floatDistance || '8px';
      el.style.animation = `dakknFloat ${duration} ease-in-out infinite alternate`;
      if (!document.getElementById('dakkn-float-style')) {
        const style = document.createElement('style');
        style.id = 'dakkn-float-style';
        style.textContent = `
          @@keyframes dakknFloat {
            from { transform: translateY(0); }
            to { transform: translateY(-${distance}); }
          }
        `;
        document.head.appendChild(style);
      }
    });
  }

  /* ── 13. Navbar Scroll Effect (universal) ──────────────────────────── */
  function initNavbarScroll() {
    const navbar = document.querySelector('[data-navbar]');
    if (!navbar) return;
    let ticking = false;
    const scrollable = document.querySelector(
      '#admin-canvas, main, [data-scroll-container]'
    ) || window;

    const update = () => {
      const scrollY =
        scrollable === window
          ? window.scrollY
          : scrollable.scrollTop;
      if (scrollY > 20) {
        navbar.classList.add('navbar-scrolled');
      } else {
        navbar.classList.remove('navbar-scrolled');
      }
      ticking = false;
    };

    scrollable.addEventListener(
      'scroll',
      () => {
        if (!ticking) {
          requestAnimationFrame(update);
          ticking = true;
        }
      },
      { passive: true }
    );
    update();
  }

  /* ── 15. Custom Animated Cursor ────────────────────────────────────── */
  function initCustomCursor() {
    if ('ontouchstart' in window || navigator.maxTouchPoints > 0) return;

    const cursorDot = document.createElement('div');
    cursorDot.className = 'custom-cursor-dot';
    cursorDot.style.cssText = 'position:fixed;top:0;left:0;z-index:99999;pointer-events:none;width:8px;height:8px;background:var(--primary-color,#0E908C);border-radius:50%;will-change:transform;transition:opacity 0.15s ease;visibility:visible;';
    
    const cursorRing = document.createElement('div');
    cursorRing.className = 'custom-cursor-ring';
    cursorRing.style.cssText = 'position:fixed;top:0;left:0;z-index:99998;pointer-events:none;width:32px;height:32px;border:2px solid var(--primary-color,#0E908C);border-radius:50%;will-change:transform;transition:opacity 0.15s ease,transform 0.15s ease;visibility:visible;';
    
    document.body.appendChild(cursorDot);
    document.body.appendChild(cursorRing);

    let mouseX = window.innerWidth / 2;
    let mouseY = window.innerHeight / 2;
    let ringX = mouseX;
    let ringY = mouseY;
    let isHovering = false;
    let isVisible = false;

    document.addEventListener('mousemove', (e) => {
      if (!isVisible) {
        document.body.classList.add('custom-cursor-active');
        cursorDot.style.opacity = isHovering ? 0 : 1;
        cursorRing.style.opacity = 1;
        isVisible = true;
      }
      mouseX = e.clientX;
      mouseY = e.clientY;
      cursorDot.style.transform = `translate3d(${mouseX - 4}px, ${mouseY - 4}px, 0)`;
    });

    document.addEventListener('mouseleave', () => {
      cursorDot.style.opacity = 0;
      cursorRing.style.opacity = 0;
      isVisible = false;
    });
    
    document.addEventListener('mouseenter', () => {
      cursorDot.style.opacity = isHovering ? 0 : 1;
      cursorRing.style.opacity = 1;
      isVisible = true;
    });

    function renderCursor() {
      ringX += (mouseX - ringX) * 0.2;
      ringY += (mouseY - ringY) * 0.2;
      
      const scale = isHovering ? 'scale(1.5)' : 'scale(1)';
      cursorRing.style.transform = `translate3d(${ringX - 16}px, ${ringY - 16}px, 0) ${scale}`;
      
      requestAnimationFrame(renderCursor);
    }
    requestAnimationFrame(renderCursor);

    const bindHover = (el) => {
      el.addEventListener('mouseenter', () => {
        isHovering = true;
        if (isVisible) cursorDot.style.opacity = 0;
        cursorRing.classList.add('hovering');
      });
      el.addEventListener('mouseleave', () => {
        isHovering = false;
        if (isVisible) cursorDot.style.opacity = 1;
        cursorRing.classList.remove('hovering');
      });
    };

    const interactives = document.querySelectorAll('a, button, input, select, textarea, [role="button"], .interactive, .product-card, .stats-card, .btn');
    interactives.forEach(bindHover);
    
    const observer = new MutationObserver((mutations) => {
      mutations.forEach((mutation) => {
        if (mutation.addedNodes.length) {
          mutation.addedNodes.forEach((node) => {
            if (node.nodeType === 1) {
              const newInteractives = node.querySelectorAll ? node.querySelectorAll('a, button, input, select, textarea, [role="button"], .interactive, .product-card, .stats-card, .btn') : [];
              const elementsToCheck = node.matches && node.matches('a, button, input, select, textarea, [role="button"], .interactive, .product-card, .stats-card, .btn') ? [node, ...newInteractives] : newInteractives;
              elementsToCheck.forEach(bindHover);
            }
          });
        }
      });
    });
    observer.observe(document.body, { childList: true, subtree: true });
  }

  /* ── Boot ──────────────────────────────────────────────────────────── */
  function boot() {
    const safe = (fn) => {
      try {
        fn();
      } catch (e) {
        console.error('site-animations: ' + fn.name, e);
      }
    };

    // Detect bfcache restore (back/forward navigation) — skip entrance animations
    window.addEventListener('pageshow', (event) => {
      if (event.persisted) {
        window.dakknBfCached = true;
        const target = document.querySelector(
          '#admin-canvas, main, [data-page-entrance]'
        );
        if (target) {
          target.style.removeProperty('opacity');
          target.style.removeProperty('transform');
          target.style.removeProperty('transition');
        }
      }
    });

    const runInit = () => {
      safe(initPageEntrance);
      safe(initPageExit);
      safe(initScrollReveal);
      safe(initStagger);
      safe(initCounters);
      safe(initSmoothScroll);
      safe(initHoverLift);
      safe(initSpotlightGlow);
      safe(initImageFadeIn);
      safe(initBackToTop);
      safe(initRipple);
      safe(initSkeletons);
      safe(initFloatingElements);
      safe(initNavbarScroll);
      safe(initCustomCursor);
    };

    if (document.readyState === 'loading') {
      document.addEventListener('DOMContentLoaded', () => requestAnimationFrame(runInit));
    } else {
      requestAnimationFrame(runInit);
    }
  }

  boot();
})();
