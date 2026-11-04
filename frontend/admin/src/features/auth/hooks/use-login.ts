"use client";

import { useState } from "react";
import { authApi } from "../api/auth.api";
import { tokenService } from "../api/token.service";
import type { LoginRequest, LoginResponse } from "../types/auth.types";

export function useLogin() {
  const [isPending, setIsPending] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function login(request: LoginRequest, rememberMe: boolean): Promise<LoginResponse | null> {
    setIsPending(true);
    setError(null);
    try {
      const response = await authApi.login(request);
      if (!response.data?.accessToken) throw new Error("No access token was returned by the server.");
      tokenService.save(response.data, rememberMe);
      return response.data;
    } catch (cause: unknown) {
      setError(cause instanceof Error ? cause.message : "Sign in failed. Please try again.");
      return null;
    } finally {
      setIsPending(false);
    }
  }

  return { login, isPending, error, clearError: () => setError(null) };
}
