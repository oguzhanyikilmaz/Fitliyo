import type { Config } from "tailwindcss";

/**
 * Apple-inspired design system.
 * Renkler, tipografi ve gölgeler Apple web sitesi diline uygun.
 * Detay: docs/frontend/APPLE_DESIGN_SYSTEM.md
 */
const config: Config = {
  content: [
    "./pages/**/*.{js,ts,jsx,tsx,mdx}",
    "./components/**/*.{js,ts,jsx,tsx,mdx}",
    "./app/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: [
          "-apple-system",
          "BlinkMacSystemFont",
          "SF Pro Display",
          "SF Pro Text",
          "Segoe UI",
          "Helvetica Neue",
          "Helvetica",
          "Arial",
          "sans-serif",
        ],
        display: [
          "-apple-system",
          "BlinkMacSystemFont",
          "SF Pro Display",
          "Segoe UI",
          "Helvetica Neue",
          "sans-serif",
        ],
      },
      colors: {
        // Apple web primary (mavi link / CTA)
        primary: {
          DEFAULT: "#0071e3",
          hover: "#0077ed",
          light: "#e8f4fc",
        },
        // Apple metin ve arka plan skalası
        apple: {
          black: "#1d1d1f",
          gray: "#6e6e73",
          grayLight: "#86868b",
          grayLighter: "#d2d2d7",
          bg: "#fbfbfd",
          white: "#ffffff",
        },
        // Geriye uyumluluk: eski fitliyo-green artık primary ile aynı anlamda kullanılabilir
        fitliyo: {
          green: "#0071e3",
          dark: "#0077ed",
        },
      },
      fontSize: {
        "apple-hero": ["3rem", { lineHeight: "1.05", letterSpacing: "-0.02em" }],
        "apple-title": ["2.5rem", { lineHeight: "1.1", letterSpacing: "-0.02em" }],
        "apple-subhead": ["1.5rem", { lineHeight: "1.3" }],
        "apple-body": ["1.0625rem", { lineHeight: "1.47" }],
      },
      borderRadius: {
        apple: "1.125rem",
        "apple-lg": "1.375rem",
        "apple-pill": "980px",
      },
      boxShadow: {
        apple: "0 2px 8px rgba(0,0,0,0.04)",
        "apple-md": "0 4px 16px rgba(0,0,0,0.06)",
        "apple-lg": "0 8px 32px rgba(0,0,0,0.08)",
      },
    },
  },
  plugins: [],
};
export default config;
