export type StorageScope = "local" | "session";

function getStorage(scope: StorageScope): Storage | null {
  if (typeof window === "undefined") return null;
  return scope === "local" ? window.localStorage : window.sessionStorage;
}

export const storageService = {
  get<TValue>(key: string, scope: StorageScope): TValue | null {
    try {
      const raw = getStorage(scope)?.getItem(key);
      return raw ? JSON.parse(raw) as TValue : null;
    } catch {
      return null;
    }
  },
  set<TValue>(key: string, value: TValue, scope: StorageScope): void {
    try {
      getStorage(scope)?.setItem(key, JSON.stringify(value));
    } catch {
      // Private browsing or full storage may prevent persistence.
    }
  },
  remove(key: string, scope: StorageScope): void {
    try {
      getStorage(scope)?.removeItem(key);
    } catch {
      // Storage may be unavailable in restricted browsers.
    }
  },
};
