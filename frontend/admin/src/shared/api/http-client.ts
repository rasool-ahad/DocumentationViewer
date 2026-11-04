import { tokenService } from "@/features/auth/api/token.service";
import type { BaseRequest, BaseResponse } from "./types";

type RequestInterceptor = (request: RequestInit) => RequestInit;
type ResponseInterceptor = (response: Response) => Response;

export class HttpError extends Error {
  constructor(public readonly status: number, message: string) {
    super(message);
    this.name = "HttpError";
  }
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}

function responseMessage(body: unknown): string | null {
  if (!isRecord(body)) return null;
  const messages = body.messages;
  if (Array.isArray(messages)) {
    const first = messages.find((message: unknown) => isRecord(message) && typeof message.description === "string");
    if (isRecord(first) && typeof first.description === "string") return first.description;
  }
  const error = body.error;
  if (isRecord(error)) {
    const details = error.details;
    if (Array.isArray(details)) {
      const first = details.find((detail: unknown) => isRecord(detail) && typeof detail.message === "string");
      if (isRecord(first) && typeof first.message === "string") return first.message;
    }
    if (typeof error.code === "string") return error.code;
  }
  return null;
}

class HttpClient {
  private readonly requestInterceptors: RequestInterceptor[] = [];
  private readonly responseInterceptors: ResponseInterceptor[] = [];

  addRequestInterceptor(interceptor: RequestInterceptor): void {
    this.requestInterceptors.push(interceptor);
  }

  addResponseInterceptor(interceptor: ResponseInterceptor): void {
    this.responseInterceptors.push(interceptor);
  }

  async post<TRequest, TResponse>(path: string, request: BaseRequest<TRequest>, signal?: AbortSignal): Promise<BaseResponse<TResponse>> {
    let options: RequestInit = {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(request),
      cache: "no-store",
      signal,
    };
    for (const interceptor of this.requestInterceptors) options = interceptor(options);

    let response = await fetch(`/backend/${path.replace(/^\//, "")}`, options);
    for (const interceptor of this.responseInterceptors) response = interceptor(response);

    const body: unknown = await response.json().catch(() => null);
    if (!response.ok) {
      throw new HttpError(response.status, responseMessage(body) ?? `Request failed (${response.status}).`);
    }
    if (!isRecord(body) || !("data" in body)) {
      throw new HttpError(response.status, "The server returned an invalid response.");
    }
    return body as unknown as BaseResponse<TResponse>;
  }
}

export const httpClient = new HttpClient();

httpClient.addRequestInterceptor((request) => {
  const token = tokenService.getAccessToken();
  if (!token) return request;
  const headers = new Headers(request.headers);
  headers.set("Authorization", `Bearer ${token}`);
  return { ...request, headers };
});

httpClient.addResponseInterceptor((response) => {
  if (response.status === 401 && tokenService.getAccessToken()) tokenService.clear();
  return response;
});
