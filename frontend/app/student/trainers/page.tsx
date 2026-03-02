"use client";

import { TrainerList } from "@/components/trainer/TrainerList";

export default function StudentTrainersPage() {
  return (
    <TrainerList
      title="Eğitmenleri Keşfet"
      subtitle="Sana uygun eğitmeni bul, paket seç ve seansları planla."
      backHref="/student"
      backLabel="← Panele dön"
    />
  );
}
