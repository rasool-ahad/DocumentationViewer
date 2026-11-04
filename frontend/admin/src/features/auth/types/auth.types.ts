export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  tokenType: string;
  expiresAtUtc: string;
}

export interface StoredSession {
  accessToken: string;
  tokenType: string;
  expiresAtUtc: string;
}
