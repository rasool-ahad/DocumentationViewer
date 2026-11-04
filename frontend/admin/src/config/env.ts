export const env = {
  apiBaseUrl: (process.env.API_BASE_URL ?? "http://localhost:63079").replace(/\/$/, ""),
} as const;
