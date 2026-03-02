"use client";

import { useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type { PagedResultDto, NotificationDto, GetNotificationListDto } from "@/lib/types";

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

export default function EgitmenBildirimlerPage() {
  const [data, setData] = useState<PagedResultDto<NotificationDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [markingId, setMarkingId] = useState<string | null>(null);

  const load = () => {
    setLoading(true);
    const params: GetNotificationListDto = {
      skipCount: 0,
      maxResultCount: 50,
      sorting: "creationTime desc",
    };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<NotificationDto>>(ApiPaths.Notification.getMyNotificationsAsync(query))
      .then(setData)
      .catch((e) => setError(e instanceof Error ? e.message : "Bildirimler yüklenemedi"))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    load();
  }, []);

  const handleMarkAsRead = (id: string) => {
    setMarkingId(id);
    apiFetch<void>(ApiPaths.Notification.markAsReadAsync(id), { method: "POST" })
      .then(() => load())
      .catch(() => setMarkingId(null))
      .finally(() => setMarkingId(null));
  };

  const handleMarkAllRead = () => {
    setMarkingId("all");
    apiFetch<void>(ApiPaths.Notification.markAllAsReadAsync(), { method: "POST" })
      .then(() => load())
      .catch(() => setMarkingId(null))
      .finally(() => setMarkingId(null));
  };

  if (loading && !data) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Bildirimler yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Bildirimler</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  const items = data?.items ?? [];
  const unreadCount = items.filter((n) => !n.isRead).length;

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <h1 className="text-2xl font-bold text-slate-800">Bildirimler</h1>
        {unreadCount > 0 && (
          <button
            type="button"
            onClick={handleMarkAllRead}
            disabled={markingId === "all"}
            className="rounded-lg border border-slate-300 bg-white px-4 py-2 text-sm font-medium text-slate-700 shadow-sm hover:bg-slate-50 disabled:opacity-50"
          >
            {markingId === "all" ? "İşleniyor..." : "Tümünü okundu işaretle"}
          </button>
        )}
      </div>

      {items.length === 0 ? (
        <div className="rounded-xl border border-slate-200 bg-slate-50 p-8 text-center text-slate-600">
          Henüz bildiriminiz yok.
        </div>
      ) : (
        <ul className="space-y-3">
          {items.map((n) => (
            <li
              key={n.id}
              className={`rounded-xl border p-4 ${
                n.isRead ? "border-slate-200 bg-white" : "border-fitliyo-green/30 bg-fitliyo-green/5"
              }`}
            >
              <div className="flex flex-wrap items-start justify-between gap-2">
                <div className="min-w-0 flex-1">
                  <p className="font-medium text-slate-800">{n.title}</p>
                  {n.body && <p className="mt-1 text-sm text-slate-600">{n.body}</p>}
                  <p className="mt-1 text-xs text-slate-400">{formatDate(n.creationTime)}</p>
                </div>
                {!n.isRead && (
                  <button
                    type="button"
                    onClick={() => handleMarkAsRead(n.id)}
                    disabled={markingId === n.id}
                    className="rounded-lg border border-slate-300 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 hover:bg-slate-50 disabled:opacity-50"
                  >
                    {markingId === n.id ? "..." : "Okundu"}
                  </button>
                )}
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
