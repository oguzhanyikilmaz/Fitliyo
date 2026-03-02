"use client";

import { useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import type { PagedResultDto, TrainerProfileDto, GetTrainerListDto } from "@/lib/types";
import { TrainerCard } from "./TrainerCard";

interface TrainerListProps {
  title?: string;
  subtitle?: string;
  backHref?: string;
  backLabel?: string;
}

export function TrainerList({ title = "Eğitmenleri Keşfet", subtitle, backHref, backLabel }: TrainerListProps) {
  const [data, setData] = useState<PagedResultDto<TrainerProfileDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const params: GetTrainerListDto = {
      skipCount: 0,
      maxResultCount: 24,
      sorting: "creationTime desc",
    };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<TrainerProfileDto>>(`/api/app/trainerProfile${query}`)
      .then(setData)
      .catch((e) => setError(e instanceof Error ? e.message : "Liste yüklenemedi"))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-apple-body text-apple-gray">Eğitmenler yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="font-display text-apple-title font-semibold text-apple-black">{title}</h1>
        <div className="card-apple border border-apple-grayLighter p-4 text-apple-body text-apple-gray">{error}</div>
        {backHref && (
          <a href={backHref} className="link-apple inline-block text-apple-body">
            {backLabel ?? "← Geri"}
          </a>
        )}
      </div>
    );
  }

  const items = data?.items ?? [];

  return (
    <div className="space-y-8">
      <h1 className="font-display text-apple-title font-semibold text-apple-black">{title}</h1>
      {subtitle && <p className="text-apple-subhead text-apple-gray">{subtitle}</p>}

      {items.length === 0 ? (
        <div className="card-apple border border-dashed border-apple-grayLighter p-12 text-center text-apple-body text-apple-gray">
          Henüz listelenecek eğitmen yok.
        </div>
      ) : (
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {items.map((trainer) => (
            <TrainerCard key={trainer.id} trainer={trainer} />
          ))}
        </div>
      )}

      {backHref && (
        <a href={backHref} className="link-apple inline-block text-apple-body">
          {backLabel ?? "← Geri"}
        </a>
      )}
    </div>
  );
}
