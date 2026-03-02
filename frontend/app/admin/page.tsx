"use client";

import { useEffect, useState } from "react";
import { apiFetch } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type { DashboardDto } from "@/lib/types";

export default function AdminDashboardPage() {
  const [data, setData] = useState<DashboardDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    apiFetch<DashboardDto>(ApiPaths.Admin.getDashboardAsync())
      .then(setData)
      .catch((e) => setError(e instanceof Error ? e.message : "Dashboard yüklenemedi"))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Admin Dashboard</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  const d = data!;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Admin Dashboard</h1>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Toplam eğitmen</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">{d.totalTrainers}</p>
        </div>
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Aktif eğitmen</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">{d.activeTrainers}</p>
        </div>
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Toplam öğrenci</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">{d.totalStudents}</p>
        </div>
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Toplam sipariş</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">{d.totalOrders}</p>
        </div>
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Bekleyen sipariş</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">{d.pendingOrders}</p>
        </div>
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Tamamlanan sipariş</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">{d.completedOrders}</p>
        </div>
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Toplam gelir</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">
            ₺{d.totalRevenue?.toLocaleString("tr-TR", { minimumFractionDigits: 2 }) ?? "0,00"}
          </p>
        </div>
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Toplam komisyon</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">
            ₺{d.totalCommission?.toLocaleString("tr-TR", { minimumFractionDigits: 2 }) ?? "0,00"}
          </p>
        </div>
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Toplam yorum</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">{d.totalReviews}</p>
        </div>
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Ortalama puan</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">
            {d.averagePlatformRating?.toFixed(1) ?? "—"}
          </p>
        </div>
      </div>
    </div>
  );
}
