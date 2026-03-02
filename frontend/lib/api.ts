/**
 * Backend API base URL.
 * Geliştirme: http://localhost:5000
 * Production: ortam değişkeni NEXT_PUBLIC_API_URL
 */
const getBaseUrl = () => {
  if (typeof window !== "undefined") {
    return process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
  }
  return process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
};

export const API_BASE = getBaseUrl();

const TOKEN_KEY = "fitliyo_token";
const USER_KEY = "fitliyo_user";

function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(TOKEN_KEY);
}

/** 401/403 durumunda token ve kullanıcıyı temizleyip giriş sayfasına yönlendirir */
function handleUnauthorized() {
  if (typeof window === "undefined") return;
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
  const redirect = encodeURIComponent(window.location.pathname + window.location.search);
  window.location.href = `/login?redirect=${redirect}`;
}

/**
 * Backend API çağrısı. Path'ler Swagger ile uyumlu kebab-case (api-paths.ts).
 * Örnek: ApiPaths.Order.getMyOrdersAsync() → /api/app/Order/GetMyOrdersAsync
 * 401/403 → giriş sayfasına yönlendirir.
 */
export async function apiFetch<T>(
  path: string,
  options: RequestInit = {}
): Promise<T> {
  const token = getToken();
  const headers: HeadersInit = {
    "Content-Type": "application/json",
    ...(options.headers as Record<string, string>),
  };
  if (token) {
    (headers as Record<string, string>)["Authorization"] = `Bearer ${token}`;
  }
  const res = await fetch(`${API_BASE}${path}`, { ...options, headers });

  if (res.status === 401 || res.status === 403) {
    handleUnauthorized();
    const err = await res.json().catch(() => ({ error: { message: res.statusText } }));
    throw new Error((err as { error?: { message?: string } })?.error?.message || "Oturum süreniz doldu. Lütfen tekrar giriş yapın.");
  }

  if (!res.ok) {
    const err = await res.json().catch(() => ({ error: { message: res.statusText } }));
    throw new Error((err as { error?: { message?: string } })?.error?.message || res.statusText);
  }
  return res.json() as Promise<T>;
}

/** Query string oluşturur (undefined/null boş bırakılır) */
export function buildQuery(params: Record<string, string | number | boolean | undefined | null>): string {
  const search = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") {
      search.set(key, String(value));
    }
  });
  const q = search.toString();
  return q ? `?${q}` : "";
}

/** P12: Liste isteklerinde zorunlu sayfalama. Parametresiz getList çağrısı yapılmamalı. */
export const DEFAULT_LIST_PARAMS = {
  skipCount: 0,
  maxResultCount: 50,
  sorting: "creationTime desc",
} as const;
