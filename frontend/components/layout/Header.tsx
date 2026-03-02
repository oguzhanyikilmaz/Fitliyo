"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { getStoredUser, clearAuth, getDashboardPathForRole } from "@/lib/auth";

export default function Header() {
  const pathname = usePathname();
  const user = getStoredUser();

  const handleLogout = () => {
    clearAuth();
    window.location.href = "/";
  };

  return (
    <header className="sticky top-0 z-50 border-b border-apple-grayLighter/50 bg-white/80 backdrop-blur-xl">
      <div className="mx-auto flex h-12 max-w-6xl items-center justify-between px-4 sm:px-6 lg:px-8">
        <Link
          href="/"
          className="text-apple-body font-semibold text-apple-black transition-opacity hover:opacity-80"
        >
          Fitliyo
        </Link>
        <nav className="flex items-center gap-6">
          <Link
            href="/trainers"
            className={`text-apple-body font-normal ${pathname === "/trainers" ? "text-apple-black" : "text-apple-gray hover:text-apple-black"}`}
          >
            Eğitmenler
          </Link>
          {!user ? (
            <>
              <Link
                href="/login"
                className="text-apple-body font-normal text-primary hover:underline"
              >
                Giriş Yap
              </Link>
              <Link
                href="/register"
                className="btn-apple-primary"
              >
                Kayıt Ol
              </Link>
            </>
          ) : (
            <>
              <Link
                href={getDashboardPathForRole(user.roles)}
                className="text-apple-body font-normal text-apple-gray hover:text-apple-black"
              >
                Panel
              </Link>
              <span className="text-apple-body text-apple-gray">{user.userName}</span>
              <button
                type="button"
                onClick={handleLogout}
                className="text-apple-body font-normal text-apple-gray hover:text-apple-black"
              >
                Çıkış
              </button>
            </>
          )}
        </nav>
      </div>
    </header>
  );
}
