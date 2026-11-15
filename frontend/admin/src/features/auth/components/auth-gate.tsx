"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState, type ReactNode } from "react";
import { AUTH_CHANGE_EVENT, tokenService } from "../api/token.service";

type AuthGateProps = {
  children: ReactNode;
  requireAuth: boolean;
};

export function AuthGate({ children, requireAuth }: AuthGateProps) {
  const router = useRouter();
  const [isAllowed, setIsAllowed] = useState(false);

  useEffect(() => {
    let active = true;
    let expiryTimer: ReturnType<typeof setTimeout> | undefined;

    function checkSession() {
      if (!active) return;
      if (expiryTimer) clearTimeout(expiryTimer);

      const session = tokenService.getSession();
      const authenticated = Boolean(session);
      setIsAllowed(authenticated === requireAuth);

      if (authenticated !== requireAuth) {
        router.replace(requireAuth ? "/login" : "/dashboard");
      } else if (session) {
        const remaining = Date.parse(session.expiresAtUtc) - Date.now();
        expiryTimer = setTimeout(checkSession, Math.max(remaining, 0) + 25);
      }
    }

    queueMicrotask(checkSession);
    window.addEventListener("storage", checkSession);
    window.addEventListener("focus", checkSession);
    window.addEventListener(AUTH_CHANGE_EVENT, checkSession);

    return () => {
      active = false;
      if (expiryTimer) clearTimeout(expiryTimer);
      window.removeEventListener("storage", checkSession);
      window.removeEventListener("focus", checkSession);
      window.removeEventListener(AUTH_CHANGE_EVENT, checkSession);
    };
  }, [requireAuth, router]);

  if (!isAllowed) return null;
  return <>{children}</>;
}
