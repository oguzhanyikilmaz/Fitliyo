"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useEffect } from "react";
import { getStoredUser } from "@/lib/auth";

const nav = [
  { href: "/student", label: "Dashboard" },
  { href: "/student/profile", label: "Profilim" },
  { href: "/student/trainers", label: "Eğitmenler" },
  { href: "/student/packages", label: "Paketler" },
  { href: "/student/orders", label: "Siparişlerim" },
  { href: "/student/sessions", label: "Seanslarım" },
  { href: "/student/notifications", label: "Bildirimler" },
  { href: "/student/support", label: "Destek" },
];

export default function StudentLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname();
  const router = useRouter();
  const user = getStoredUser();

  useEffect(() => {
    if (!user) {
      router.replace("/login?redirect=/student");
      return;
    }
    const isStudent = user.roles.includes("Student");
    if (!isStudent && !user.roles.includes("Admin") && !user.roles.includes("SuperAdmin")) {
      router.replace("/");
    }
  }, [user, router]);

  if (!user) return null;

  return (
    <div className="flex gap-6">
      <aside className="w-52 shrink-0 rounded-xl border bg-white p-4 shadow-sm">
        <p className="mb-4 text-xs font-medium uppercase text-slate-400">Öğrenci Paneli</p>
        <nav className="space-y-1">
          {nav.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              className={`block rounded-lg px-3 py-2 text-sm font-medium ${
                pathname === item.href
                  ? "bg-primary/10 text-primary"
                  : "text-apple-gray hover:bg-apple-grayLighter/50"
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
