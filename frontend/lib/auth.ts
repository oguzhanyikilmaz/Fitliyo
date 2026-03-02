"use client";

const TOKEN_KEY = "fitliyo_token";
const USER_KEY = "fitliyo_user";

export interface StoredUser {
  id?: string;
  userName?: string;
  roles: string[];
}

function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(TOKEN_KEY);
}

export function setAuth(accessToken: string, opts: { roles?: string[] } = {}) {
  if (typeof window === "undefined") return;
  localStorage.setItem(TOKEN_KEY, accessToken);
  const payload = parseJwtPayload(accessToken);
  const sub = payload?.sub as string | undefined;
  const name = (payload?.name ?? payload?.unique_name) as string | undefined;
  const roles = opts.roles ?? (payload?.role ? [payload.role] : []) as string[];
  const arr = payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
  const roleList = Array.isArray(arr) ? arr : arr ? [arr] : roles;
  localStorage.setItem(
    USER_KEY,
    JSON.stringify({
      id: sub,
      userName: name ?? sub,
      roles: roleList.length ? roleList : roles,
    })
  );
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

export function getStoredUser(): StoredUser | null {
  if (typeof window === "undefined") return null;
  const raw = localStorage.getItem(USER_KEY);
  if (!raw) {
    const token = getToken();
    if (token) {
      const payload = parseJwtPayload(token);
      const sub = payload?.sub as string | undefined;
      const name = (payload?.name ?? payload?.unique_name) as string | undefined;
      const arr = payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
      const roles = (Array.isArray(arr) ? arr : arr ? [arr] : []) as string[];
      return { id: sub, userName: name ?? sub, roles };
    }
    return null;
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
  return !!getToken();
}

export function getRolesFromToken(token: string): string[] {
  const payload = parseJwtPayload(token);
  const arr = payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
  if (Array.isArray(arr)) return arr as string[];
  if (arr) return [arr as string];
  return (payload?.role ? [payload.role as string] : []);
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
