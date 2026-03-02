import Link from "next/link";
import type { TrainerProfileDto } from "@/lib/types";

interface TrainerCardProps {
  trainer: TrainerProfileDto;
}

export function TrainerCard({ trainer }: TrainerCardProps) {
  const href = trainer.slug
    ? `/trainers/${encodeURIComponent(trainer.slug)}`
    : `/trainers/id/${trainer.id}`;

  return (
    <Link
      href={href}
      className="card-apple block p-6 transition hover:shadow-apple-md"
    >
      <div className="flex gap-4">
        {trainer.profilePhotoUrl ? (
          <img
            src={trainer.profilePhotoUrl}
            alt={trainer.trainerFullName ?? "Eğitmen"}
            className="h-16 w-16 rounded-full object-cover"
          />
        ) : (
          <div className="flex h-16 w-16 items-center justify-center rounded-full bg-primary/10 text-2xl font-semibold text-primary">
            {(trainer.trainerFullName ?? "E")[0]}
          </div>
        )}
        <div className="min-w-0 flex-1">
          <h2 className="truncate font-semibold text-apple-black">
            {trainer.trainerFullName ?? "Eğitmen"}
          </h2>
          {trainer.city && <p className="text-apple-body text-apple-gray">{trainer.city}</p>}
          <div className="mt-1 flex items-center gap-2 text-apple-body text-apple-gray">
            <span>★ {trainer.averageRating > 0 ? trainer.averageRating.toFixed(1) : "—"}</span>
            <span>({trainer.totalReviewCount} değerlendirme)</span>
          </div>
          {trainer.bio && (
            <p className="mt-2 line-clamp-2 text-apple-body text-apple-gray">{trainer.bio}</p>
          )}
        </div>
      </div>
    </Link>
  );
}
