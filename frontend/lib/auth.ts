/**
 * Auth ve rol yönetimi.
 * Token backend'den OpenIddict /connect/token (grant_type=password) ile alınır.
 * Rol bilgisi token içindeki "role" claim veya /api/abp/application-configuration currentUser.roles ile alınır.
 */

export type UserRole = "Student" | "Trainer" | "Admin" | "SuperAdmin";

export interface AuthUser {
  id: string;
  userName: string;
  email?: string;
  name?: string;
  roles: UserRole[];
}

const TOKEN_KEY = "fitliyo_token";
const USER_KEY = "fitliyo_user";

function parseJwtPayload(token: string): Record<string, unknown> {
  try {
    const base64 = token.split(".")[1];
    if (!base64) return {};
    return JSON.parse(atob(base64)) as Record<string, unknown>;
  } catch {
    return {};
  }
}

/** Token'dan rol çıkar (ABP/OpenIddict genelde "role" claim) */
function getRolesFromTokenInternal(token: string): UserRole[] {
  const payload = parseJwtPayload(token);
  const role = payload.role as string | undefined;
  const roleArr = payload["http://www.aspnetboilerplate.com/identity/roles"] as string[] | undefined;
  const roles: string[] = role ? [role] : Array.isArray(roleArr) ? roleArr : [];
  return roles.filter((r): r is UserRole =>
    ["Student", "Trainer", "Admin", "SuperAdmin"].includes(r)
  );
}

export function getStoredUser(): AuthUser | null {
  if (typeof window === "undefined") return null;
  const raw = localStorage.getItem(USER_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as AuthUser;
  } catch {
    return null;
  }
}

export function setAuth(token: string, user?: Partial<AuthUser>): void {
  if (typeof window === "undefined") return;
  localStorage.setItem(TOKEN_KEY, token);
  const roles = getRolesFromTokenInternal(token);
  const stored: AuthUser = {
    id: (user?.id as string) ?? (parseJwtPayload(token).sub as string),
    userName: (user?.userName as string) ?? (parseJwtPayload(token).preferred_username as string) ?? "",
    email: user?.email,
    name: user?.name,
    roles: roles.length > 0 ? roles : (user?.roles as UserRole[]) ?? [],
  };
  localStorage.setItem(USER_KEY, JSON.stringify(stored));
}

export function clearAuth(): void {
  if (typeof window === "undefined") return;
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}

export function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(TOKEN_KEY);
}

export function isAuthenticated(): boolean {
  return !!getToken();
}

/** Giriş sonrası yönlendirilecek path (role göre, İngilizce URL) */
export function getDashboardPathForRole(roles: UserRole[]): string {
  if (roles.includes("SuperAdmin") || roles.includes("Admin")) return "/admin";
  if (roles.includes("Trainer")) return "/trainer";
  if (roles.includes("Student")) return "/student";
  return "/";
}

export function getDefaultRole(roles: UserRole[]): UserRole | null {
  if (roles.includes("SuperAdmin") || roles.includes("Admin")) return "Admin";
  if (roles.includes("Trainer")) return "Trainer";
  if (roles.includes("Student")) return "Student";
  return null;
}

/** Token'dan roller; dışarıdan kullanım için (örn. giriş sonrası) */
export function getRolesFromToken(token: string): UserRole[] {
  return getRolesFromTokenInternal(token);
}
