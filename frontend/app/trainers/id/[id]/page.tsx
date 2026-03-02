"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import { apiFetch } from "@/lib/api";
import { isAuthenticated } from "@/lib/auth";
import type { TrainerProfileDto } from "@/lib/types";
import { TrainerPackageList } from "@/components/trainer/TrainerPackageList";

export default function TrainerProfileByIdPage() {
  const params = useParams();
  const id = params?.id as string;
  const [trainer, setTrainer] = useState<TrainerProfileDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    apiFetch<TrainerProfileDto>(`/api/app/trainerProfile/${id}`)
      .then(setTrainer)
      .catch((e) => setError(e instanceof Error ? e.message : "Profil yüklenemedi"))
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Profil yükleniyor...</p>
      </div>
    );
  }

  if (error || !trainer) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Eğitmen Profili</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">
          {error ?? "Profil bulunamadı."}
        </div>
        <Link href="/trainers" className="inline-block text-sm font-medium text-fitliyo-green hover:underline">
          ← Eğitmen listesine dön
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-3xl space-y-6">
      <Link href="/trainers" className="inline-block text-sm font-medium text-fitliyo-green hover:underline">
        ← Eğitmen listesine dön
      </Link>

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <div className="flex flex-wrap gap-6">
          {trainer.profilePhotoUrl ? (
            <img
              src={trainer.profilePhotoUrl}
              alt={trainer.trainerFullName ?? "Eğitmen"}
              className="h-24 w-24 rounded-full object-cover"
            />
          ) : (
            <div className="flex h-24 w-24 items-center justify-center rounded-full bg-fitliyo-green/20 text-3xl font-bold text-fitliyo-green">
              {(trainer.trainerFullName ?? "E")[0]}
            </div>
          )}
          <div>
            <h1 className="text-2xl font-bold text-slate-800">{trainer.trainerFullName ?? "Eğitmen"}</h1>
            {(trainer.city || trainer.district) && (
              <p className="text-slate-500">{[trainer.city, trainer.district].filter(Boolean).join(", ")}</p>
            )}
            <div className="mt-1 flex items-center gap-2">
              <span className="text-amber-600">★ {trainer.averageRating > 0 ? trainer.averageRating.toFixed(1) : "—"}</span>
              <span className="text-slate-400">({trainer.totalReviewCount} değerlendirme)</span>
            </div>
            {trainer.bio && <p className="mt-3 text-slate-600">{trainer.bio}</p>}
          </div>
        </div>
      </div>

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-slate-800">Paketler</h2>
        <TrainerPackageList trainerProfileId={trainer.id} isAuthenticated={isAuthenticated()} />
      </div>
    </div>
  );
}
