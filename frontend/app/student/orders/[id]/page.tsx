"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import { apiFetch } from "@/lib/api";
import type { OrderDto, SessionDto } from "@/lib/types";

const ORDER_STATUS_LABELS: Record<number, string> = {
  0: "Beklemede",
  1: "Onaylandı",
  2: "Devam ediyor",
  3: "Tamamlandı",
  4: "İptal",
};

const SESSION_STATUS_LABELS: Record<number, string> = {
  0: "Planlandı",
  1: "Devam ediyor",
  2: "Tamamlandı",
  3: "İptal",
};

function formatDate(s: string) {
  try {
    return new Date(s).toLocaleString("tr-TR", {
      day: "2-digit",
      month: "short",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  } catch {
    return s;
  }
}

export default function StudentOrderDetailPage() {
  const params = useParams();
  const id = params?.id as string;
  const [order, setOrder] = useState<OrderDto | null>(null);
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    apiFetch<OrderDto>(`/api/app/order/${id}`)
      .then((o) => {
        setOrder(o);
        return apiFetch<{ items: SessionDto[] }>(`/api/app/order/getSessions?orderId=${id}`);
      })
      .then((res) => setSessions(res.items ?? []))
      .catch((e) => setError(e instanceof Error ? e.message : "Sipariş yüklenemedi"))
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Yükleniyor...</p>
      </div>
    );
  }

  if (error || !order) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Sipariş Detayı</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">
          {error ?? "Sipariş bulunamadı."}
        </div>
        <Link href="/student/orders" className="inline-block text-sm font-medium text-fitliyo-green hover:underline">
          ← Siparişlerime dön
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <Link href="/student/orders" className="inline-block text-sm font-medium text-fitliyo-green hover:underline">
        ← Siparişlerime dön
      </Link>

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <h1 className="text-xl font-bold text-slate-800">Sipariş #{order.orderNumber}</h1>
        <p className="mt-1 text-slate-500">{order.packageTitle}</p>
        <p className="text-sm text-slate-600">{order.trainerFullName}</p>
        <div className="mt-4 flex flex-wrap gap-3">
          <span className="rounded-full bg-slate-100 px-2 py-1 text-sm text-slate-700">
            {ORDER_STATUS_LABELS[order.status] ?? order.status}
          </span>
          <span className="text-sm font-medium text-slate-700">
            {order.netAmount} {order.currency}
          </span>
          <span className="text-sm text-slate-500">{formatDate(order.creationTime)}</span>
        </div>
      </div>

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <h2 className="font-semibold text-slate-800">Seanslar</h2>
        {sessions.length === 0 ? (
          <p className="mt-2 text-sm text-slate-500">Henüz seans planlanmamış.</p>
        ) : (
          <ul className="mt-3 space-y-2">
            {sessions.map((s) => (
              <li key={s.id} className="flex flex-wrap items-center justify-between gap-2 rounded-lg border border-slate-100 p-3">
                <div>
                  <p className="text-sm font-medium text-slate-800">Seans #{s.sequenceNumber}</p>
                  <p className="text-xs text-slate-500">{formatDate(s.scheduledStartTime)}</p>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-xs text-slate-600">{SESSION_STATUS_LABELS[s.status] ?? s.status}</span>
                  {s.meetingUrl && (
                    <a
                      href={s.meetingUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-sm font-medium text-fitliyo-green hover:underline"
                    >
                      Bağlantı
                    </a>
                  )}
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}
