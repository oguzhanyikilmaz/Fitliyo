"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { clearAuth, getStoredUser, isAccessTokenValid, isAuthenticated } from "@/lib/auth";

const nav = [
  { href: "/student", label: "Dashboard" },
  { href: "/student/profile", label: "Profilim" },
  { href: "/student/trainers", label: "Eğitmenler" },
  { href: "/student/packages", label: "Paketler" },
  { href: "/student/orders", label: "Siparişlerim" },
  { href: "/student/sessions", label: "Seanslarım" },
  { href: "/student/messages", label: "Mesajlar" },
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
  const [ready, setReady] = useState(false);
  const [user, setUser] = useState<ReturnType<typeof getStoredUser>>(null);

  useEffect(() => {
    if (!isAuthenticated() || !isAccessTokenValid()) {
      clearAuth();
      router.replace("/login?redirect=/student");
      return;
    }
    const u = getStoredUser();
    if (!u) {
      router.replace("/login?redirect=/student");
      return;
    }
    const isStudent = u.roles.includes("Student");
    if (!isStudent && !u.roles.includes("Admin") && !u.roles.includes("SuperAdmin")) {
      router.replace("/");
      return;
    }
    setUser(u);
    setReady(true);
  }, [router]);

  if (!ready || !user) {
    return (
      <div className="flex min-h-[50vh] items-center justify-center">
        <p className="text-apple-body text-apple-gray">Yükleniyor...</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-8 md:flex-row md:gap-10">
      <aside className="shrink-0 md:w-56">
        <div className="rounded-apple-lg border border-apple-grayLighter/80 bg-white p-5 shadow-apple">
          <p className="mb-4 text-xs font-semibold uppercase tracking-wide text-apple-grayLight">
            Öğrenci paneli
          </p>
          <nav className="flex flex-col gap-1.5">
            {nav.map((item) => (
              <Link
                key={item.href}
                href={item.href}
                className={`rounded-apple border border-transparent px-4 py-2.5 text-sm font-medium transition-colors ${
                  pathname === item.href
                    ? "border-primary/20 bg-primary/10 text-primary"
                    : "text-apple-gray hover:border-apple-grayLighter hover:bg-apple-bg"
                }`}
              >
                {item.label}
              </Link>
            ))}
          </nav>
        </div>
      </aside>
      <div className="min-w-0 flex-1 pb-8">{children}</div>
    </div>
  );
}
