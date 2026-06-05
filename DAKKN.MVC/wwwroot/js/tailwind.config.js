tailwind.config = {
    darkMode: "class",
    theme: {
        extend: {
            colors: {
                /* ── Sky-blue / Cyan primary ────────────────────── */
                primary: "#006a68",
                "primary-container": "#69d2cf",
                "primary-fixed": "#8cf4f0",
                "primary-fixed-dim": "#6ed7d4",
                "primary-light": "#A6ECE9",
                "primary-dark": "#45B8B5",
                "on-primary": "#ffffff",
                "on-primary-container": "#005957",
                "on-primary-fixed": "#00201f",
                "on-primary-fixed-variant": "#00504e",
                "inverse-primary": "#6ed7d4",

                /* ── Warm secondary ──────────────────────────────── */
                secondary: "#725c00",
                "secondary-container": "#fed74c",
                "secondary-fixed": "#ffe07e",
                "secondary-fixed-dim": "#e9c339",
                "on-secondary": "#ffffff",
                "on-secondary-container": "#735d00",
                "on-secondary-fixed": "#231b00",
                "on-secondary-fixed-variant": "#564500",

                /* ── Warm amber tertiary ────────────────────────── */
                tertiary: "#8b501a",
                "tertiary-container": "#ffb072",
                "tertiary-fixed": "#ffdcc4",
                "tertiary-fixed-dim": "#ffb780",
                "on-tertiary": "#ffffff",
                "on-tertiary-container": "#79410b",
                "on-tertiary-fixed": "#2f1400",
                "on-tertiary-fixed-variant": "#6e3902",

                /* ── Surfaces ───────────────────────── */
                surface: "#FFFFFF",
                "surface-dim": "#d6dbda",
                "surface-bright": "#f6faf9",
                "surface-variant": "#dfe3e2",
                "surface-container-lowest": "#ffffff",
                "surface-container-low": "#f0f5f3",
                "surface-container": "#eaefee",
                "surface-container-high": "#e4e9e8",
                "surface-container-highest": "#dfe3e2",
                "surface-tint": "#006a68",

                /* ── Text ────────────────────────────────────────── */
                "on-surface": "#171d1c",
                "on-surface-variant": "#3d4948",
                background: "#F8FAFB",
                "on-background": "#171d1c",
                outline: "#6d7979",
                "outline-variant": "#bcc9c8",
                "inverse-surface": "#2c3131",
                "inverse-on-surface": "#edf2f1",
                "text-primary": "#1A1A1A",
                "text-secondary": "#6B7280",

                /* ── Error ───────────────────────────────────────── */
                error: "#ba1a1a",
                "error-container": "#ffdad6",
                "on-error": "#ffffff",
                "on-error-container": "#93000a",

                /* ── Accents ──────────────────────────────────────── */
                "accent-orange": "#FF9F45",
                "accent-pink": "#FF7EB6",
                "accent-purple": "#9C7DFF",

                /* ── Layout ───────────────────────────────────────── */
                "border": "#E8EDF0",
            },
            borderRadius: {
                DEFAULT: "0.25rem",
                lg: "0.5rem",
                xl: "0.75rem",
                "2xl": "1.5rem",
                full: "9999px"
            },
            spacing: {
                "xs": "4px",
                "sm": "8px",
                "md": "16px",
                "lg": "24px",
                "xl": "32px",
                "2xl": "48px",
                "3xl": "64px",
                "gutter": "16px",
                "margin-mobile": "20px",
            },
            fontFamily: {
                sans: ["Cairo", "Inter", "sans-serif"],
                display: ["Cairo", "sans-serif"],
                body: ["Cairo", "sans-serif"],
                caption: ["Cairo", "sans-serif"],
                h1: ["Cairo", "sans-serif"],
                h2: ["Cairo", "sans-serif"],
                h3: ["Cairo", "sans-serif"],
                h4: ["Cairo", "sans-serif"],
            },
            fontSize: {
                "caption": ["12px", { "lineHeight": "1.4", "fontWeight": "400" }],
                "body": ["14px", { "lineHeight": "1.5", "fontWeight": "400" }],
                "body-lg": ["16px", { "lineHeight": "1.5", "fontWeight": "500" }],
                "h4": ["20px", { "lineHeight": "1.4", "fontWeight": "600" }],
                "h3": ["24px", { "lineHeight": "1.3", "fontWeight": "600" }],
                "h2": ["30px", { "lineHeight": "1.3", "fontWeight": "600" }],
                "h2-mobile": ["26px", { "lineHeight": "1.3", "fontWeight": "600" }],
                "h1": ["36px", { "lineHeight": "1.2", "fontWeight": "700" }],
                "h1-mobile": ["32px", { "lineHeight": "1.2", "fontWeight": "700" }],
                "display": ["48px", { "lineHeight": "1.2", "fontWeight": "700" }],
            },
            boxShadow: {
                'small': '0 4px 12px rgba(0,0,0,0.05)',
                'medium': '0 8px 24px rgba(0,0,0,0.08)',
                'large': '0 20px 40px rgba(0,0,0,0.12)',
                'auth': '0 20px 40px rgba(0,0,0,0.12)',
            }
        }
    }
};

