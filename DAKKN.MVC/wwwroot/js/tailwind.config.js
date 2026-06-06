tailwind.config = {
    darkMode: "class",
    theme: {
        extend: {
            colors: {
                /* ── Sky-blue / Cyan primary (Original) ─────────── */
                "primary-old":            "#0891B2",   

                /* ── New Brand Palette ──────────────────────────── */
                primary:                  "#0E908C",   /* New primary teal */
                decorative:               "#69D2CF",   /* Soft decorative teal */
                "background-light":       "#FFFFFF",
                "background-dark":        "#121212",

                "primary-container":      "#CFFAFE",
                "primary-fixed":          "#E0F7FA",
                "primary-fixed-dim":      "#A5F3FC",
                "on-primary":             "#FFFFFF",
                "on-primary-container":   "#164E63",
                "inverse-primary":        "#67E8F9",

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
                DEFAULT: "0.75rem", lg: "0.5rem", xl: "0.75rem", "2xl": "1rem", "3xl": "1.5rem", full: "9999px"
            },
            fontFamily: {
                sans:    ["Inter", "Plus Jakarta Sans", "Cairo", "sans-serif"],
                arabic:  ["Cairo", "sans-serif"],
                display: ["'IBM Plex Sans Arabic'", "sans-serif"],
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

