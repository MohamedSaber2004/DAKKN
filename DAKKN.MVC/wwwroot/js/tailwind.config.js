tailwind.config = {
    darkMode: "class",
    theme: {
        extend: {
            colors: {
                /* ── Brand Primary (teal #0E908C) ────────── */
                primary:                  "var(--primary-color, #0E908C)",
                decorative:               "rgba(var(--primary-rgb, 14, 144, 140), 0.5)",
                "primary-container":      "rgba(var(--primary-rgb, 14, 144, 140), 0.15)",
                "primary-light":          "rgba(var(--primary-rgb, 14, 144, 140), 0.3)",
                "on-primary":             "#FFFFFF",
                "primary-hover":          "#0B726F",
                "primary-active":         "#085E5C",

                /* ── Warm secondary (amber/gold) ────────── */
                secondary: "#735D00",
                "secondary-container": "#FED74C",
                "on-secondary": "#FFFFFF",

                /* ── Tertiary (burnt orange - reduced usage) ─ */
                tertiary: "#8B501A",
                "tertiary-container": "#FFB072",
                "on-tertiary": "#FFFFFF",

                /* ── Surfaces ───────────────────────────── */
                surface: "#FFFFFF",
                "surface-dim": "#D6DBDA",
                "surface-bright": "#F6FAF9",
                "surface-variant": "#DFE3E2",
                "surface-container-lowest": "#FFFFFF",
                "surface-container-low": "#F0F5F3",
                "surface-container": "#EAEFEE",
                "surface-container-high": "#E4E9E8",
                "surface-container-highest": "#DFE3E2",

                /* ── Text (WCAG AA verified) ────────────── */
                "on-surface": "#171D1C",
                "on-surface-variant": "#3D4948",
                background: "#F8FAFB",
                "on-background": "#171D1C",
                "dark-page": "#0A2030",
                outline: "#6D7979",
                "outline-variant": "#BCC9C8",
                "inverse-surface": "#2C3131",
                "inverse-on-surface": "#EDF2F1",
                "text-primary": "#1A1A1A",
                "text-secondary": "#4B5563",

                /* ── Utility / Feedback ──────────────────── */
                error: "#BA1A1A",
                "error-container": "#FFDAD6",
                "on-error": "#FFFFFF",
                success: "#1E7B4C",
                "success-container": "#B8F0D0",
                warning: "#9C6E00",
                "warning-container": "#FFDF9E",
                info: "#006A68",
                "info-container": "#9AEFEC",

                /* ── Layout ──────────────────────────────── */
                border: "#E8EDF0",
            },
            spacing: {
                "xs": "0.25rem",
                "sm": "0.5rem",
                "md": "1rem",
                "lg": "1.5rem",
                "xl": "2.5rem",
                "2xl": "4rem",
                "3xl": "6rem",
                "margin-mobile": "1.25rem",
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
                'card': '0 4px 16px rgba(var(--primary-rgb, 14, 144, 140), 0.10)',
                'card-hover': '0 12px 32px rgba(var(--primary-rgb, 14, 144, 140), 0.20)',
                'button': '0 4px 14px rgba(var(--primary-rgb, 14, 144, 140), 0.30)',
            },
            fontSize: {
                'h1': ['2.5rem', { lineHeight: '1.15', fontWeight: '800' }],
                'h2': ['2rem', { lineHeight: '1.2', fontWeight: '800' }],
                'h3': ['1.5rem', { lineHeight: '1.3', fontWeight: '700' }],
                'h4': ['1.25rem', { lineHeight: '1.4', fontWeight: '700' }],
                'body': ['1rem', { lineHeight: '1.6' }],
                'body-sm': ['0.875rem', { lineHeight: '1.6' }],
                'caption': ['0.75rem', { lineHeight: '1.5' }],
            }
        }
    }
};
