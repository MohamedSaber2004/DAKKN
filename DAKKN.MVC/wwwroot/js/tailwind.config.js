tailwind.config = {
    darkMode: "class",
    theme: {
        extend: {
            colors: {
                /* ── Sky-blue / Cyan primary ────────────────────── */
                primary:                  "#0891B2",   /* cyan-600  */
                "primary-container":      "#CFFAFE",   /* cyan-100  */
                "primary-fixed":          "#E0F7FA",
                "primary-fixed-dim":      "#A5F3FC",
                "on-primary":             "#FFFFFF",
                "on-primary-container":   "#164E63",
                "inverse-primary":        "#67E8F9",

                /* ── Warm secondary ──────────────────────────────── */
                secondary:                "#0E7490",   /* cyan-700  */
                "secondary-container":    "#E0F2FE",
                "on-secondary":           "#FFFFFF",
                "on-secondary-container": "#0C4A6E",

                /* ── Warm amber tertiary ────────────────────────── */
                tertiary:                 "#B45309",   /* amber-700 */
                "tertiary-container":     "#FEF3C7",   /* amber-100 */
                "on-tertiary":            "#FFFFFF",
                "on-tertiary-container":  "#78350F",
                "tertiary-fixed":         "#FDE68A",

                /* ── Creamy / sky surfaces ───────────────────────── */
                surface:                  "#FFFFFF",
                "surface-dim":            "#DCF0F5",
                "surface-bright":         "#FEFCF8",   /* warm ivory */
                "surface-variant":        "#D8EFF4",
                "surface-container-lowest":"#FFFFFF",
                "surface-container-low":  "#FFF9F4",   /* warm cream */
                "surface-container":      "#EFF9FB",   /* sky tint   */
                "surface-container-high": "#DDF2F6",
                "surface-container-highest":"#CCE9EF",

                /* ── Text ────────────────────────────────────────── */
                "on-surface":             "#0A2030",
                "on-surface-variant":     "#3A5A62",
                background:               "#FFFCF8",   /* warm ivory background */
                "on-background":          "#0A2030",
                outline:                  "#5F8A96",
                "outline-variant":        "#A8CDD6",
                "inverse-surface":        "#1A3840",
                "inverse-on-surface":     "#E4F3F7",

                /* ── Error ───────────────────────────────────────── */
                error:                    "#DC2626",
                "error-container":        "#FEE2E2",
                "on-error":               "#FFFFFF",
                "on-error-container":     "#991B1B",
            },
            borderRadius: {
                DEFAULT: "0.25rem", lg: "0.5rem", xl: "0.75rem", "2xl": "1rem", full: "9999px"
            },
            fontFamily: {
                sans:    ["Inter",  "Cairo", "sans-serif"],
                arabic:  ["Cairo",  "sans-serif"],
            }
        }
    }
};
