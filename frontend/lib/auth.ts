"use client";

const TOKEN_KEY = "fitliyo_token";
const USER_KEY = "fitliyo_user";
/** Saniye cinsinden; JWT exp ile karşılaştırmada saat kayması için tampon */
const EXPIRY_SKEW_SEC = 45;

export interface StoredUser {
  id?: string;
  userName?: string;
  roles: string[];
}

export function getAccessToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(TOKEN_KEY);
}

function parseJwtPayload(token: string): Record<string, unknown> | null {
  try {
    const parts = token.split(".");
    if (parts.length !== 3) return null;
    const payload = parts[1];
    const base64 = payload.replace(/-/g, "+").replace(/_/g, "/");
    const json = decodeURIComponent(
      atob(base64)
        .split("")
        .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
        .join("")
    );
    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return null;
  }
}

/** JWT exp (Unix saniye) varsa ve süresi dolmuşsa false */
export function isAccessTokenValid(): boolean {
  const token = getAccessToken();
  if (!token) return false;
  const payload = parseJwtPayload(token);
  const exp = payload?.exp;
  if (exp === undefined || exp === null) return true;
  const expNum = typeof exp === "number" ? exp : Number(exp);
  if (Number.isNaN(expNum)) return true;
  return Math.floor(Date.now() / 1000) < expNum - EXPIRY_SKEW_SEC;
}

const ROLE_CLAIM = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

/** ABP / OpenIddict JWT rol claim'lerini dizi yapar (tekil veya çoklu) */
export function getRolesFromTokenPayload(payload: Record<string, unknown> | null): string[] {
  if (!payload) return [];
  const fromUri = payload[ROLE_CLAIM];
  if (Array.isArray(fromUri)) return fromUri.map(String);
  if (typeof fromUri === "string" && fromUri) return [fromUri];

  const r = payload.role;
  if (Array.isArray(r)) return r.map(String);
  if (typeof r === "string" && r) return [r];
  if (r != null) return [String(r)];

  return [];
}

export function getRolesFromToken(token: string): string[] {
  return getRolesFromTokenPayload(parseJwtPayload(token));
}

export function setAuth(accessToken: string, opts: { roles?: string[] } = {}) {
  if (typeof window === "undefined") return;
  localStorage.setItem(TOKEN_KEY, accessToken);
  const payload = parseJwtPayload(accessToken);
  const sub = payload?.sub as string | undefined;
  const name = (payload?.name ?? payload?.unique_name) as string | undefined;
  const fromJwt = getRolesFromTokenPayload(payload);
  const roleList = fromJwt.length ? fromJwt : opts.roles ?? [];
  localStorage.setItem(
    USER_KEY,
    JSON.stringify({
      id: sub,
      userName: name ?? sub,
      roles: roleList,
    })
  );
}

export function getStoredUser(): StoredUser | null {
  if (typeof window === "undefined") return null;
  const raw = localStorage.getItem(USER_KEY);
  if (!raw) {
    const token = getAccessToken();
    if (!token) return null;
    if (!isAccessTokenValid()) return null;
    const payload = parseJwtPayload(token);
    const sub = payload?.sub as string | undefined;
    const name = (payload?.name ?? payload?.unique_name) as string | undefined;
    return {
      id: sub,
      userName: name ?? sub,
      roles: getRolesFromTokenPayload(payload),
    };
  }
  try {
    return JSON.parse(raw) as StoredUser;
  } catch {
    return null;
  }
}

export function clearAuth() {
  if (typeof window === "undefined") return;
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}

export function isAuthenticated(): boolean {
  return !!getAccessToken() && isAccessTokenValid();
}

export function getDashboardPathForRole(roles: string[]): string {
  if (roles?.includes("Admin") || roles?.includes("SuperAdmin")) return "/admin";
  if (roles?.includes("Trainer")) return "/trainer";
  return "/student";
}

export function getDefaultRole(roles: string[]): string {
  if (roles?.includes("Admin") || roles?.includes("SuperAdmin")) return "Admin";
  if (roles?.includes("Trainer")) return "Trainer";
  return "Student";
}
