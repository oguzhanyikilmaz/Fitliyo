"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type { PagedResultDto, ServicePackageDto, GetPackageListDto } from "@/lib/types";

interface TrainerPackageListProps {
  trainerProfileId: string;
  isAuthenticated?: boolean;
}

export function TrainerPackageList({ trainerProfileId, isAuthenticated }: TrainerPackageListProps) {
  const [data, setData] = useState<PagedResultDto<ServicePackageDto> | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const params: GetPackageListDto = {
      trainerProfileId,
      skipCount: 0,
      maxResultCount: 20,
    };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<ServicePackageDto>>(ApiPaths.ServicePackage.getListAsync(query))
      .then(setData)
      .catch(() => setData({ items: [], totalCount: 0 }))
      .finally(() => setLoading(false));
  }, [trainerProfileId]);

  if (loading) return <p className="text-apple-body text-apple-gray">Paketler yükleniyor...</p>;

  const items = data?.items ?? [];
  if (items.length === 0) {
    return <p className="text-apple-body text-apple-gray">Henüz paket eklenmemiş.</p>;
  }

  return (
    <div className="mt-4 space-y-3">
      {items.map((pkg) => (
        <div
          key={pkg.id}
          className="flex flex-wrap items-center justify-between gap-4 rounded-apple border border-apple-grayLighter/50 bg-apple-bg/50 p-4"
        >
          <div>
            <p className="font-medium text-apple-black">{pkg.title}</p>
            <p className="text-apple-body text-apple-gray">
              {pkg.discountedPrice ?? pkg.price} {pkg.currency}
              {pkg.sessionCount != null && ` · ${pkg.sessionCount} seans`}
            </p>
          </div>
          {isAuthenticated ? (
            <Link href={`/student/packages/${pkg.id}`} className="btn-apple-primary">
              Paket Seç
            </Link>
          ) : (
            <Link href="/login" className="btn-apple-secondary">
              Giriş Yap / Satın Al
            </Link>
          )}
        </div>
      ))}
    </div>
  );
}
