"use client";

import { TrainerList } from "@/components/trainer/TrainerList";

export default function TrainersPage() {
  return (
    <TrainerList
      title="Eğitmenleri Keşfet"
      subtitle="Kategori, şehir veya uzmanlığa göre filtreleyerek eğitmenleri listeleyebilirsin."
      backHref="/"
      backLabel="← Ana sayfaya dön"
    />
  );
}
