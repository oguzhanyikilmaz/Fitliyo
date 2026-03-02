"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import type {
  PagedResultDto,
  FeaturedListingDto,
  GetFeaturedListingListDto,
  TrainerProfileDto,
} from "@/lib/types";

const HOMEPAGE_PAGE_TYPE = 0; // FeaturedListingPageType.Homepage

export function FeaturedTrainers() {
  const [trainers, setTrainers] = useState<TrainerProfileDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const params: GetFeaturedListingListDto = {
      pageType: HOMEPAGE_PAGE_TYPE,
      isActive: true,
      skipCount: 0,
      maxResultCount: 6,
      sorting: "sortOrder asc",
    };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<FeaturedListingDto>>(`/api/app/featuredListing${query}`)
      .then((res) => {
        const ids = (res.items ?? [])
          .map((x) => x.trainerProfileId)
          .filter((id): id is string => !!id);
        if (ids.length === 0) return [];
        return Promise.all(ids.map((id) => apiFetch<TrainerProfileDto>(`/api/app/trainerProfile/${id}`)));
      })
      .then(setTrainers)
      .catch(() => setTrainers([]))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <section>
        <h2 className="font-display text-apple-title font-semibold text-apple-black">Öne Çıkan Eğitmenler</h2>
        <p className="mt-2 text-apple-body text-apple-gray">Yükleniyor...</p>
      </section>
    );
  }

  if (trainers.length === 0) {
    return (
      <section>
        <h2 className="font-display text-apple-title font-semibold text-apple-black">Öne Çıkan Eğitmenler</h2>
        <p className="mt-2 text-apple-body text-apple-gray">Henüz öne çıkan eğitmen yok.</p>
        <Link href="/trainers" className="link-apple mt-3 inline-block text-apple-body">
          Tüm eğitmenleri gör →
        </Link>
      </section>
    );
  }

  return (
    <section>
      <h2 className="font-display text-apple-title font-semibold text-apple-black">Öne Çıkan Eğitmenler</h2>
      <div className="mt-8 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
        {trainers.map((trainer) => {
          const href = trainer.slug
            ? `/trainers/${encodeURIComponent(trainer.slug)}`
            : `/trainers/id/${trainer.id}`;
          return (
            <Link
              key={trainer.id}
              href={href}
              className="card-apple flex p-6 transition hover:shadow-apple-md"
            >
              <div className="flex gap-4">
                {trainer.profilePhotoUrl ? (
                  <img
                    src={trainer.profilePhotoUrl}
                    alt={trainer.trainerFullName ?? "Eğitmen"}
                    className="h-14 w-14 rounded-full object-cover"
                  />
                ) : (
                  <div className="flex h-14 w-14 items-center justify-center rounded-full bg-primary/10 text-xl font-semibold text-primary">
                    {(trainer.trainerFullName ?? "E")[0]}
                  </div>
                )}
                <div className="min-w-0 flex-1">
                  <h3 className="truncate font-semibold text-apple-black">{trainer.trainerFullName ?? "Eğitmen"}</h3>
                  {trainer.city && <p className="text-apple-body text-apple-gray">{trainer.city}</p>}
                  <p className="text-apple-body text-apple-gray">★ {trainer.averageRating > 0 ? trainer.averageRating.toFixed(1) : "—"}</p>
                </div>
              </div>
            </Link>
          );
        })}
      </div>
      <Link href="/trainers" className="link-apple mt-6 inline-block text-apple-body">
        Tüm eğitmenleri keşfet →
      </Link>
    </section>
  );
}
