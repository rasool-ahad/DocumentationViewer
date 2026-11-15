import { storageService, type StorageScope } from "@/shared/services/storage.service";
import type { LoginResponse, StoredSession } from "../types/auth.types";

const SESSION_KEY = "docviewer.admin.session";
export const AUTH_CHANGE_EVENT = "docviewer:auth-change";

function removeStoredSession(): void {
  storageService.remove(SESSION_KEY, "local");
  storageService.remove(SESSION_KEY, "session");
}

function notifyAuthChange(): void {
  if (typeof window !== "undefined") window.dispatchEvent(new Event(AUTH_CHANGE_EVENT));
}

function read(scope: StorageScope): StoredSession | null {
  const session = storageService.get<StoredSession>(SESSION_KEY, scope);
  const expiresAt = session?.expiresAtUtc ? Date.parse(session.expiresAtUtc) : Number.NaN;
  if (!session?.accessToken || !Number.isFinite(expiresAt) || expiresAt <= Date.now()) {
    storageService.remove(SESSION_KEY, scope);
    return null;
  }
  return session;
}

export const tokenService = {
  getSession(): StoredSession | null {
    return read("local") ?? read("session");
  },
  getAccessToken(): string | null {
    return this.getSession()?.accessToken ?? null;
  },
  isSuperAdmin(): boolean {
    const token = this.getAccessToken();
    if (!token) return false;
    try {
      const payload = JSON.parse(atob(token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")));
      const roles = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ?? payload.role;
      return Array.isArray(roles) ? roles.includes("SuperAdmin") : roles === "SuperAdmin";
    } catch { return false; }
  },
  save(response: LoginResponse, rememberMe: boolean): void {
    removeStoredSession();
    storageService.set<StoredSession>(SESSION_KEY, response, rememberMe ? "local" : "session");
    notifyAuthChange();
  },
  clear(): void {
    removeStoredSession();
    notifyAuthChange();
  },
};
