export interface BaseRequest<TData> {
  data: TData;
  context?: string;
}

export interface ApiMessage {
  type: 0 | 1 | 2 | 3;
  title: string | null;
  description: string | null;
}

export interface ValidationError {
  field: string | null;
  message: string | null;
}

export interface ApiErrorBody {
  code: string | null;
  status: number | null;
  details: ValidationError[] | null;
}

export interface BaseResponse<TData> {
  requestId: string | null;
  apiVersion: string | null;
  context: string | null;
  messages: ApiMessage[] | null;
  error: ApiErrorBody | null;
  data?: TData | null;
}
