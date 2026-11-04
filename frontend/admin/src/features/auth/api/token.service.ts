import { storageService, type StorageScope } from "@/shared/services/storage.service";
import type { LoginResponse, StoredSession } from "../types/auth.types";

const SESSION_KEY = "docviewer.admin.session";

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
  save(response: LoginResponse, rememberMe: boolean): void {
    this.clear();
    storageService.set<StoredSession>(SESSION_KEY, response, rememberMe ? "local" : "session");
  },
  clear(): void {
    storageService.remove(SESSION_KEY, "local");
    storageService.remove(SESSION_KEY, "session");
  },
};
