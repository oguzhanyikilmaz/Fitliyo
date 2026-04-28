/**
 * Backend API base URL.
 * Geliştirme: http://localhost:5001
 * Production: ortam değişkeni NEXT_PUBLIC_API_URL
 */
import { clearAuth, getAccessToken, isAccessTokenValid } from "./auth";

const getBaseUrl = () => {
  if (typeof window !== "undefined") {
    return process.env.NEXT_PUBLIC_API_URL || "http://localhost:5001";
  }
  return process.env.NEXT_PUBLIC_API_URL || "http://localhost:5001";
};

export const API_BASE = getBaseUrl();

type AbpErrorBody = {
  error?: {
    code?: string;
    message?: string;
    details?: string;
  };
  message?: string;
};

function getErrorMessageFromBody(body: unknown, fallback: string): string {
  if (!body || typeof body !== "object") return fallback;
  const b = body as AbpErrorBody;
  if (b.error?.message) return b.error.message;
  if (b.error?.details) return b.error.details;
  if (typeof b.message === "string" && b.message) return b.message;
  return fallback;
}

/** 401/403 durumunda token ve kullanıcıyı temizleyip giriş sayfasına yönlendirir */
function handleUnauthorized() {
  if (typeof window === "undefined") return;
  clearAuth();
  const redirect = encodeURIComponent(window.location.pathname + window.location.search);
  window.location.href = `/login?redirect=${redirect}`;
}

/**
 * Backend API çağrısı. Path'ler `lib/api-paths.ts` + Swagger ile uyumlu olmalı.
 * 401/403 → oturum temizlenir, giriş sayfasına yönlendirilir.
 */
export async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = getAccessToken();
  if (token && !isAccessTokenValid()) {
    handleUnauthorized();
    throw new Error("Oturum süreniz doldu. Lütfen tekrar giriş yapın.");
  }

  const headers: HeadersInit = {
    "Content-Type": "application/json",
    ...(options.headers as Record<string, string>),
  };
  if (token && isAccessTokenValid()) {
    (headers as Record<string, string>)["Authorization"] = `Bearer ${token}`;
  }
  const res = await fetch(`${API_BASE}${path}`, { ...options, headers });

  if (res.status === 401 || res.status === 403) {
    const errBody = await res.json().catch(() => ({}));
    handleUnauthorized();
    throw new Error(
      getErrorMessageFromBody(errBody, "Oturum süreniz doldu veya yetkiniz yok. Lütfen tekrar giriş yapın.")
    );
  }

  if (!res.ok) {
    const errBody = await res.json().catch(() => ({}));
    throw new Error(
      getErrorMessageFromBody(errBody, res.statusText || "İstek başarısız")
    );
  }
  if (res.status === 204) {
    return undefined as T;
  }
  return res.json() as Promise<T>;
}

/**
 * ABP sorgu parametreleri: ASP.NET model binding genelde PascalCase (Filter, SkipCount, CategoryId).
 * camelCase TS alanları verilir; ilk harf büyütülür.
 */
export function buildQuery(params: Record<string, string | number | boolean | undefined | null>): string {
  const search = new URLSearchParams();
  for (const [k, v] of Object.entries(params)) {
    if (v === undefined || v === null || v === "") continue;
    const key = k.length > 0 ? k[0].toUpperCase() + k.slice(1) : k;
    search.set(key, String(v));
  }
  const q = search.toString();
  return q ? `?${q}` : "";
}

/** @alias buildQuery — ABP DTO sorguları için */
export const buildAbpQuery = buildQuery;

/** P12: Liste isteklerinde zorunlu sayfalama. Parametresiz getList çağrısı yapılmamalı. */
export const DEFAULT_LIST_PARAMS = {
  skipCount: 0,
  maxResultCount: 50,
  sorting: "creationTime desc",
} as const;
