/**
 * Backend API base URL.
 * Geliştirme: http://localhost:6000 (veya backend portu)
 * Production: ortam değişkeni NEXT_PUBLIC_API_URL
 */
const getBaseUrl = () => {
  if (typeof window !== "undefined") {
    return process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
  }
  return process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
};

export const API_BASE = getBaseUrl();

function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem("fitliyo_token");
}

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
