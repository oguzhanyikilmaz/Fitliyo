"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useEffect } from "react";
import { getStoredUser } from "@/lib/auth";

const nav = [
  { href: "/admin", label: "Dashboard" },
  { href: "/admin/support", label: "Destek Talepleri" },
  { href: "/admin/disputes", label: "Uyuşmazlıklar" },
  { href: "/admin/featured", label: "Öne Çıkanlar" },
  { href: "/admin/blog", label: "Blog" },
  { href: "/admin/withdrawals", label: "Para Çekme Talepleri" },
];

export default function AdminLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname();
  const router = useRouter();
  const user = getStoredUser();

  useEffect(() => {
    if (!user) {
      router.replace("/login?redirect=/admin");
      return;
    }
    const isAdmin = user.roles.includes("Admin") || user.roles.includes("SuperAdmin");
    if (!isAdmin) {
      router.replace("/");
    }
  }, [user, router]);

  if (!user) return null;

  return (
    <div className="flex gap-6">
      <aside className="w-52 shrink-0 rounded-xl border bg-slate-800 p-4 text-white shadow-sm">
        <p className="mb-4 text-xs font-medium uppercase text-slate-400">Admin Paneli</p>
        <nav className="space-y-1">
          {nav.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              className={`block rounded-lg px-3 py-2 text-sm font-medium ${
                pathname === item.href
                  ? "bg-white/20 text-white"
                  : "text-slate-300 hover:bg-white/10"
              }`}
            >
              {item.label}
            </Link>
          ))}
        </nav>
      </aside>
      <div className="min-w-0 flex-1">{children}</div>
    </div>
  );
}
