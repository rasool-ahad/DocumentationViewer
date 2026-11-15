import { httpClient } from "@/shared/api/http-client";
import type { LoginRequest, LoginResponse } from "../types/auth.types";

export const authApi = {
  login(request: LoginRequest, signal?: AbortSignal) {
    return httpClient.post<LoginRequest, LoginResponse>("admin/users/login", { data: request }, signal);
  },
  signOut() {
    return httpClient.post<Record<string, never>, { signedOut: boolean }>("admin/users/logout", { data: {} });
  },
};
