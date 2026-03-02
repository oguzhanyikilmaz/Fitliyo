"use client";

import { useEffect, useState } from "react";
import { apiFetch } from "@/lib/api";
import type { UserProfileDto, CreateUpdateUserProfileDto } from "@/lib/types";
import { Gender, ActivityLevel, FitnessGoal } from "@/lib/types";

const GENDER_LABELS: Record<number, string> = {
  [Gender.NotSpecified]: "Belirtilmedi",
  [Gender.Male]: "Erkek",
  [Gender.Female]: "Kadın",
  [Gender.Other]: "Diğer",
};

const ACTIVITY_LABELS: Record<number, string> = {
  [ActivityLevel.NotSpecified]: "Belirtilmedi",
  [ActivityLevel.Sedentary]: "Hareketsiz (oturarak çalışan)",
  [ActivityLevel.Light]: "Hafif (haftada 1-3 gün egzersiz)",
  [ActivityLevel.Moderate]: "Orta (haftada 3-5 gün)",
  [ActivityLevel.Active]: "Aktif (haftada 6-7 gün)",
  [ActivityLevel.VeryActive]: "Çok aktif (fiziksel iş + antrenman)",
};

const GOAL_LABELS: Record<number, string> = {
  [FitnessGoal.NotSpecified]: "Belirtilmedi",
  [FitnessGoal.LoseWeight]: "Kilo ver",
  [FitnessGoal.GainMuscle]: "Kilo al / kas",
  [FitnessGoal.Maintain]: "Kilo koru",
  [FitnessGoal.GeneralFitness]: "Genel fitness",
  [FitnessGoal.Performance]: "Performans",
};

function toIsoDate(s: string | null | undefined): string {
  if (!s) return "";
  try {
    return s.slice(0, 10);
  } catch {
    return "";
  }
}

export function UserHealthProfile() {
  const [profile, setProfile] = useState<UserProfileDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    apiFetch<UserProfileDto>("/api/app/userProfile/getMyProfile")
      .then((p) => setProfile({ ...p, userId: p.userId || "" }))
      .catch((e) => setError(e instanceof Error ? e.message : "Profil yüklenemedi"))
      .finally(() => setLoading(false));
  }, []);

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!profile) return;
    setSaving(true);
    setError(null);
    setSuccess(false);
    const input: CreateUpdateUserProfileDto = {
      birthDate: profile.birthDate || null,
      gender: profile.gender,
      heightCm: profile.heightCm ?? null,
      weightKg: profile.weightKg ?? null,
      bloodType: profile.bloodType || null,
      activityLevel: profile.activityLevel,
      fitnessGoal: profile.fitnessGoal,
      chronicConditions: profile.chronicConditions || null,
      allergies: profile.allergies || null,
      medications: profile.medications || null,
      injuries: profile.injuries || null,
      emergencyContact: profile.emergencyContact || null,
      phone: profile.phone || null,
      notes: profile.notes || null,
      waistCm: profile.waistCm ?? null,
      hipCm: profile.hipCm ?? null,
      neckCm: profile.neckCm ?? null,
      targetWeightKg: profile.targetWeightKg ?? null,
      sleepHoursPerNight: profile.sleepHoursPerNight ?? null,
      smoking: profile.smoking ?? null,
      alcoholConsumption: profile.alcoholConsumption || null,
      restingHeartRate: profile.restingHeartRate ?? null,
    };
    apiFetch<UserProfileDto>("/api/app/userProfile/updateMyProfile", {
      method: "PUT",
      body: JSON.stringify(input),
    })
      .then((updated) => {
        setProfile(updated);
        setSuccess(true);
        setTimeout(() => setSuccess(false), 3000);
      })
      .catch((e) => setError(e instanceof Error ? e.message : "Kaydetme başarısız"))
      .finally(() => setSaving(false));
  };

  if (loading) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-apple-body text-apple-gray">Profil yükleniyor...</p>
      </div>
    );
  }

  if (error && !profile) {
    return (
      <div className="rounded-apple border border-apple-grayLighter bg-white p-6">
        <p className="text-apple-body text-red-600">{error}</p>
      </div>
    );
  }

  const p = profile ?? ({} as UserProfileDto);
  const inputClass = "mt-1 w-full rounded-apple border border-apple-grayLighter bg-white px-4 py-2.5 text-apple-body text-apple-black focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20";
  const labelClass = "block text-apple-body font-medium text-apple-black";

  return (
    <div className="space-y-8">
      <h1 className="font-display text-apple-title font-semibold text-apple-black">Sağlık ve Profil Bilgilerim</h1>
      <p className="text-apple-body text-apple-gray">
        Bu bilgiler bazal metabolizma (BMR), BMI ve günlük kalori ihtiyacı (TDEE) hesaplamalarında kullanılır.
      </p>

      {/* Hesaplanan metrikler */}
      {(p.bmi != null || p.bmr != null || p.tdee != null) && (
        <div className="card-apple p-6">
          <h2 className="font-display text-apple-subhead font-semibold text-apple-black mb-4">Hesaplanan Metrikler</h2>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {p.age != null && (
              <div>
                <p className="text-apple-body text-apple-gray">Yaş</p>
                <p className="text-xl font-semibold text-apple-black">{p.age}</p>
              </div>
            )}
            {p.bmi != null && (
              <div>
                <p className="text-apple-body text-apple-gray">BMI (Vücut Kitle İndeksi)</p>
                <p className="text-xl font-semibold text-apple-black">{p.bmi}</p>
                <p className="text-xs text-apple-grayLight">
                  {p.bmi < 18.5 ? "Düşük" : p.bmi <= 24.9 ? "Normal" : p.bmi <= 29.9 ? "Fazla kilolu" : "Obez"}
                </p>
              </div>
            )}
            {p.bmr != null && (
              <div>
                <p className="text-apple-body text-apple-gray">Bazal Metabolizma (BMR)</p>
                <p className="text-xl font-semibold text-apple-black">{Math.round(p.bmr)} kcal/gün</p>
              </div>
            )}
            {p.tdee != null && (
              <div>
                <p className="text-apple-body text-apple-gray">Günlük Kalori İhtiyacı (TDEE)</p>
                <p className="text-xl font-semibold text-apple-black">{Math.round(p.tdee)} kcal/gün</p>
              </div>
            )}
            {(p.idealWeightMinKg != null || p.idealWeightMaxKg != null) && (
              <div className="sm:col-span-2">
                <p className="text-apple-body text-apple-gray">İdeal kilo aralığı (BMI 18.5–24.9)</p>
                <p className="text-xl font-semibold text-apple-black">
                  {p.idealWeightMinKg != null && p.idealWeightMaxKg != null
                    ? `${p.idealWeightMinKg} – ${p.idealWeightMaxKg} kg`
                    : ""}
                </p>
              </div>
            )}
            {p.bodyFatPercentage != null && (
              <div>
                <p className="text-apple-body text-apple-gray">Tahmini vücut yağ % (Navy)</p>
                <p className="text-xl font-semibold text-apple-black">%{p.bodyFatPercentage}</p>
              </div>
            )}
          </div>
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-8">
        {success && (
          <div className="rounded-apple bg-primary/10 p-4 text-apple-body text-primary">Profil kaydedildi.</div>
        )}
        {error && (
          <div className="rounded-apple bg-red-50 p-4 text-apple-body text-red-600">{error}</div>
        )}

        <div className="card-apple p-6 space-y-6">
          <h2 className="font-display text-apple-subhead font-semibold text-apple-black">Kişisel Bilgiler</h2>
          <div className="grid gap-4 sm:grid-cols-2">
            <div>
              <label className={labelClass}>Doğum tarihi</label>
              <input
                type="date"
                className={inputClass}
                value={toIsoDate(p.birthDate)}
                onChange={(e) => setProfile({ ...p, birthDate: e.target.value || null })}
              />
            </div>
            <div>
              <label className={labelClass}>Cinsiyet</label>
              <select
                className={inputClass}
                value={p.gender ?? Gender.NotSpecified}
                onChange={(e) => setProfile({ ...p, gender: Number(e.target.value) })}
              >
                {Object.entries(GENDER_LABELS).map(([k, v]) => (
                  <option key={k} value={k}>{v}</option>
                ))}
              </select>
            </div>
            <div>
              <label className={labelClass}>Boy (cm)</label>
              <input
                type="number"
                min={100}
                max={250}
                step={0.1}
                className={inputClass}
                value={p.heightCm ?? ""}
                onChange={(e) => setProfile({ ...p, heightCm: e.target.value ? Number(e.target.value) : null })}
              />
            </div>
            <div>
              <label className={labelClass}>Kilo (kg)</label>
              <input
                type="number"
                min={30}
                max={300}
                step={0.1}
                className={inputClass}
                value={p.weightKg ?? ""}
                onChange={(e) => setProfile({ ...p, weightKg: e.target.value ? Number(e.target.value) : null })}
              />
            </div>
            <div>
              <label className={labelClass}>Kan grubu</label>
              <input
                type="text"
                placeholder="A+, B-, vb."
                className={inputClass}
                value={p.bloodType ?? ""}
                onChange={(e) => setProfile({ ...p, bloodType: e.target.value || null })}
              />
            </div>
            <div>
              <label className={labelClass}>Aktivite düzeyi</label>
              <select
                className={inputClass}
                value={p.activityLevel ?? ActivityLevel.NotSpecified}
                onChange={(e) => setProfile({ ...p, activityLevel: Number(e.target.value) })}
              >
                {Object.entries(ACTIVITY_LABELS).map(([k, v]) => (
                  <option key={k} value={k}>{v}</option>
                ))}
              </select>
            </div>
            <div>
              <label className={labelClass}>Hedef</label>
              <select
                className={inputClass}
                value={p.fitnessGoal ?? FitnessGoal.NotSpecified}
                onChange={(e) => setProfile({ ...p, fitnessGoal: Number(e.target.value) })}
              >
                {Object.entries(GOAL_LABELS).map(([k, v]) => (
                  <option key={k} value={k}>{v}</option>
                ))}
              </select>
            </div>
            <div>
              <label className={labelClass}>Hedef kilo (kg)</label>
              <input
                type="number"
                min={30}
                max={300}
                step={0.1}
                className={inputClass}
                value={p.targetWeightKg ?? ""}
                onChange={(e) => setProfile({ ...p, targetWeightKg: e.target.value ? Number(e.target.value) : null })}
              />
            </div>
            <div>
              <label className={labelClass}>Telefon</label>
              <input
                type="text"
                className={inputClass}
                value={p.phone ?? ""}
                onChange={(e) => setProfile({ ...p, phone: e.target.value || null })}
              />
            </div>
            <div>
              <label className={labelClass}>Acil durum iletişim</label>
              <input
                type="text"
                className={inputClass}
                value={p.emergencyContact ?? ""}
                onChange={(e) => setProfile({ ...p, emergencyContact: e.target.value || null })}
              />
            </div>
          </div>
        </div>

        <div className="card-apple p-6 space-y-6">
          <h2 className="font-display text-apple-subhead font-semibold text-apple-black">Ölçüler (isteğe bağlı — vücut yağ % için)</h2>
          <div className="grid gap-4 sm:grid-cols-3">
            <div>
              <label className={labelClass}>Bel (cm)</label>
              <input
                type="number"
                min={0}
                step={0.1}
                className={inputClass}
                value={p.waistCm ?? ""}
                onChange={(e) => setProfile({ ...p, waistCm: e.target.value ? Number(e.target.value) : null })}
              />
            </div>
            <div>
              <label className={labelClass}>Kalça (cm)</label>
              <input
                type="number"
                min={0}
                step={0.1}
                className={inputClass}
                value={p.hipCm ?? ""}
                onChange={(e) => setProfile({ ...p, hipCm: e.target.value ? Number(e.target.value) : null })}
              />
            </div>
            <div>
              <label className={labelClass}>Boyun (cm)</label>
              <input
                type="number"
                min={0}
                step={0.1}
                className={inputClass}
                value={p.neckCm ?? ""}
                onChange={(e) => setProfile({ ...p, neckCm: e.target.value ? Number(e.target.value) : null })}
              />
            </div>
          </div>
        </div>

        <div className="card-apple p-6 space-y-6">
          <h2 className="font-display text-apple-subhead font-semibold text-apple-black">Sağlık Bilgileri</h2>
          <div className="grid gap-4 sm:grid-cols-2">
            <div className="sm:col-span-2">
              <label className={labelClass}>Kronik hastalıklar / rahatsızlıklar</label>
              <textarea
                rows={3}
                className={inputClass}
                value={p.chronicConditions ?? ""}
                onChange={(e) => setProfile({ ...p, chronicConditions: e.target.value || null })}
              />
            </div>
            <div className="sm:col-span-2">
              <label className={labelClass}>Alerjiler</label>
              <textarea
                rows={2}
                className={inputClass}
                value={p.allergies ?? ""}
                onChange={(e) => setProfile({ ...p, allergies: e.target.value || null })}
              />
            </div>
            <div className="sm:col-span-2">
              <label className={labelClass}>Kullandığınız ilaçlar</label>
              <textarea
                rows={2}
                className={inputClass}
                value={p.medications ?? ""}
                onChange={(e) => setProfile({ ...p, medications: e.target.value || null })}
              />
            </div>
            <div className="sm:col-span-2">
              <label className={labelClass}>Bilinen sakatlık / yaralanma</label>
              <textarea
                rows={2}
                className={inputClass}
                value={p.injuries ?? ""}
                onChange={(e) => setProfile({ ...p, injuries: e.target.value || null })}
              />
            </div>
            <div>
              <label className={labelClass}>Ortalama uyku (saat/gün)</label>
              <input
                type="number"
                min={0}
                max={24}
                className={inputClass}
                value={p.sleepHoursPerNight ?? ""}
                onChange={(e) => setProfile({ ...p, sleepHoursPerNight: e.target.value ? Number(e.target.value) : null })}
              />
            </div>
            <div>
              <label className={labelClass}>Dinlenim kalp atışı (bpm)</label>
              <input
                type="number"
                min={30}
                max={120}
                className={inputClass}
                value={p.restingHeartRate ?? ""}
                onChange={(e) => setProfile({ ...p, restingHeartRate: e.target.value ? Number(e.target.value) : null })}
              />
            </div>
            <div>
              <label className={labelClass}>Sigara</label>
              <select
                className={inputClass}
                value={p.smoking === null || p.smoking === undefined ? "" : p.smoking ? "1" : "0"}
                onChange={(e) => setProfile({ ...p, smoking: e.target.value === "" ? null : e.target.value === "1" })}
              >
                <option value="">Belirtilmedi</option>
                <option value="0">Hayır</option>
                <option value="1">Evet</option>
              </select>
            </div>
            <div>
              <label className={labelClass}>Alkol tüketimi</label>
              <input
                type="text"
                placeholder="Örn: Nadiren, haftada 1-2"
                className={inputClass}
                value={p.alcoholConsumption ?? ""}
                onChange={(e) => setProfile({ ...p, alcoholConsumption: e.target.value || null })}
              />
            </div>
            <div className="sm:col-span-2">
              <label className={labelClass}>Notlar</label>
              <textarea
                rows={3}
                className={inputClass}
                value={p.notes ?? ""}
                onChange={(e) => setProfile({ ...p, notes: e.target.value || null })}
              />
            </div>
          </div>
        </div>

        <button type="submit" disabled={saving} className="btn-apple-primary disabled:opacity-50">
          {saving ? "Kaydediliyor..." : "Profili Kaydet"}
        </button>
      </form>
    </div>
  );
}
