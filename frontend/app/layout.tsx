import type { Metadata } from "next";
import "./globals.css";
import Header from "@/components/layout/Header";

export const metadata: Metadata = {
  title: "Fitliyo — Spor & Sağlık Koçluğu",
  description: "Eğitmenlerle buluş, hedeflerine ulaş.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="tr" className="scroll-smooth">
      <body className="min-h-screen font-sans antialiased text-apple-black bg-apple-bg">
        <Header />
        <main className="mx-auto min-h-[80vh] max-w-6xl px-4 py-8 sm:px-6 lg:px-8">{children}</main>
        <footer className="border-t border-apple-grayLighter/50 bg-white py-8 text-center text-apple-body text-apple-gray">
          Fitliyo © {new Date().getFullYear()}
        </footer>
      </body>
    </html>
  );
}
