"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useEffect } from "react";
import { getStoredUser } from "@/lib/auth";

const nav = [
  { href: "/trainer", label: "Dashboard" },
  { href: "/trainer/profile", label: "Sağlık profilim" },
  { href: "/trainer/profile-edit", label: "Eğitmen profili" },
  { href: "/trainer/packages", label: "Paketlerim" },
  { href: "/trainer/orders", label: "Siparişlerim" },
  { href: "/trainer/sessions", label: "Seanslarım" },
  { href: "/trainer/messages", label: "Mesajlar" },
  { href: "/trainer/wallet", label: "Cüzdan" },
  { href: "/trainer/withdrawals", label: "Para Çekme" },
  { href: "/trainer/notifications", label: "Bildirimler" },
  { href: "/trainer/support", label: "Destek" },
];

export default function TrainerLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname();
  const router = useRouter();
  const user = getStoredUser();

  useEffect(() => {
    if (!user) {
      router.replace("/login?redirect=/trainer");
      return;
    }
    const isTrainer = user.roles.includes("Trainer");
    if (!isTrainer && !user.roles.includes("Admin") && !user.roles.includes("SuperAdmin")) {
      router.replace("/");
    }
  }, [user, router]);

  if (!user) return null;

  return (
    <div className="flex gap-6">
      <aside className="w-52 shrink-0 rounded-xl border bg-white p-4 shadow-sm">
        <p className="mb-4 text-xs font-medium uppercase text-slate-400">Eğitmen Paneli</p>
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
