"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";

const API_BASE = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
const APP_NAME = "Fitliyo";

export default function RegisterPage() {
  const router = useRouter();
  const [userName, setUserName] = useState("");
  const [emailAddress, setEmailAddress] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [role, setRole] = useState<"Student" | "Trainer">("Student");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    if (password !== confirmPassword) {
      setError("Şifreler eşleşmiyor.");
      return;
    }
    if (password.length < 6) {
      setError("Şifre en az 6 karakter olmalıdır.");
      return;
    }
    setLoading(true);
    try {
      const res = await fetch(`${API_BASE}/api/account/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          userName,
          emailAddress,
          password,
          appName: APP_NAME,
          Role: role,
        }),
      });

      if (!res.ok) {
        const data = await res.json().catch(() => ({}));
        const msg = (data as { error?: { message?: string }; message?: string })?.error?.message
          ?? (data as { message?: string })?.message
          ?? "Kayıt başarısız.";
        throw new Error(msg);
      }
      router.push("/login?register=ok");
      router.refresh();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Kayıt yapılamadı.");
    } finally {
      setLoading(false);
    }
  };

  const inputClass = "mt-2 w-full rounded-apple border border-apple-grayLighter bg-white px-4 py-3 text-apple-body text-apple-black placeholder:text-apple-grayLight focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20";
  const labelClass = "block text-apple-body font-medium text-apple-black";

  return (
    <div className="card-apple mx-auto max-w-md p-8">
      <h1 className="font-display text-apple-title font-semibold text-apple-black">Kayıt Ol</h1>
      <p className="mt-2 text-apple-body text-apple-gray">
        Öğrenci veya eğitmen olarak hesap oluştur.
      </p>
      <form onSubmit={handleSubmit} className="mt-8 space-y-5">
        {error && (
          <div className="rounded-apple bg-red-50 p-4 text-apple-body text-red-600">
            {error}
          </div>
        )}
        <div>
          <label htmlFor="userName" className={labelClass}>Kullanıcı adı</label>
          <input id="userName" type="text" value={userName} onChange={(e) => setUserName(e.target.value)} required autoComplete="username" className={inputClass} />
        </div>
        <div>
          <label htmlFor="emailAddress" className={labelClass}>E-posta</label>
          <input id="emailAddress" type="email" value={emailAddress} onChange={(e) => setEmailAddress(e.target.value)} required autoComplete="email" className={inputClass} />
        </div>
        <div>
          <label htmlFor="role" className={labelClass}>Hesap tipi</label>
          <select id="role" value={role} onChange={(e) => setRole(e.target.value as "Student" | "Trainer")} className={inputClass}>
            <option value="Student">Öğrenci</option>
            <option value="Trainer">Eğitmen</option>
          </select>
        </div>
        <div>
          <label htmlFor="password" className={labelClass}>Şifre</label>
          <input id="password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required minLength={6} autoComplete="new-password" className={inputClass} />
          <p className="mt-1 text-apple-body text-apple-grayLight">En az 6 karakter</p>
        </div>
        <div>
          <label htmlFor="confirmPassword" className={labelClass}>Şifre tekrar</label>
          <input id="confirmPassword" type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} required autoComplete="new-password" className={inputClass} />
        </div>
        <button type="submit" disabled={loading} className="btn-apple-primary w-full disabled:opacity-50">
          {loading ? "Kayıt yapılıyor..." : "Kayıt Ol"}
        </button>
      </form>
      <p className="mt-6 text-center text-apple-body text-apple-gray">
        Zaten hesabın var mı?{" "}
        <Link href="/login" className="link-apple">Giriş yap</Link>
      </p>
    </div>
  );
}
