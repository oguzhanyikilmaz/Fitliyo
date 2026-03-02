"use client";

import { Suspense, useState, useEffect } from "react";
import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import { setAuth, getDashboardPathForRole, getRolesFromToken } from "@/lib/auth";

const API_BASE = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
const CLIENT_ID = process.env.NEXT_PUBLIC_OAUTH_CLIENT_ID || "Fitliyo_App";

function normalizeRedirect(redirect: string | null): string | null {
  if (!redirect || !redirect.startsWith("/")) return null;
  const map: Record<string, string> = {
    "/ogrenci": "/student",
    "/egitmen": "/trainer",
    "/admin": "/admin",
  };
  return map[redirect] ?? redirect;
}

function LoginForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (searchParams.get("register") === "ok")
      setSuccess("Kayıt tamamlandı. Giriş yapabilirsiniz.");
  }, [searchParams]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      const form = new URLSearchParams();
      form.set("grant_type", "password");
      form.set("username", userName);
      form.set("password", password);
      form.set("client_id", CLIENT_ID);
      form.set("scope", "Fitliyo openid profile");

      const res = await fetch(`${API_BASE}/connect/token`, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: form.toString(),
      });

      if (!res.ok) {
        const data = await res.json().catch(() => ({}));
        throw new Error((data as { error_description?: string })?.error_description || "Giriş başarısız.");
      }
      const data = (await res.json()) as { access_token: string };
      const roles = getRolesFromToken(data.access_token);
      setAuth(data.access_token, { roles });
      const redirectTo = normalizeRedirect(searchParams.get("redirect"));
      const path = redirectTo ?? getDashboardPathForRole(roles);
      router.push(path);
      router.refresh();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Giriş yapılamadı.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card-apple mx-auto max-w-md p-8">
      <h1 className="font-display text-apple-title font-semibold text-apple-black">Giriş Yap</h1>
      <p className="mt-2 text-apple-body text-apple-gray">
        Hesabınla giriş yap; rolüne göre panele yönlendirileceksin.
      </p>
      <form onSubmit={handleSubmit} className="mt-8 space-y-5">
        {success && (
          <div className="rounded-apple bg-primary/10 p-4 text-apple-body text-primary">
            {success}
          </div>
        )}
        {error && (
          <div className="rounded-apple bg-red-50 p-4 text-apple-body text-red-600">
            {error}
          </div>
        )}
        <div>
          <label htmlFor="userName" className="block text-apple-body font-medium text-apple-black">
            Kullanıcı adı veya e-posta
          </label>
          <input
            id="userName"
            type="text"
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            required
            className="mt-2 w-full rounded-apple border border-apple-grayLighter bg-white px-4 py-3 text-apple-body text-apple-black placeholder:text-apple-grayLight focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
          />
        </div>
        <div>
          <label htmlFor="password" className="block text-apple-body font-medium text-apple-black">
            Şifre
          </label>
          <input
            id="password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            className="mt-2 w-full rounded-apple border border-apple-grayLighter bg-white px-4 py-3 text-apple-body text-apple-black focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
          />
        </div>
        <button
          type="submit"
          disabled={loading}
          className="btn-apple-primary w-full disabled:opacity-50"
        >
          {loading ? "Giriş yapılıyor..." : "Giriş Yap"}
        </button>
      </form>
      <p className="mt-6 text-center text-apple-body text-apple-gray">
        Hesabın yok mu?{" "}
        <Link href="/register" className="link-apple">
          Kayıt ol
        </Link>
      </p>
    </div>
  );
}

export default function LoginPage() {
  return (
    <Suspense fallback={<div className="card-apple mx-auto max-w-md animate-pulse p-8">Yükleniyor...</div>}>
      <LoginForm />
    </Suspense>
  );
}
