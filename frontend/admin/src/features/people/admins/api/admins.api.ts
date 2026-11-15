import { httpClient } from "@/shared/api/http-client";
import type { PaginationRequest, PaginationResponse } from "@/shared/api/types";

export type AdminRole = "Admin" | "SuperAdmin";

export interface AdminUser {
  id: number;
  userName: string;
  firstName: string;
  lastName: string;
  email: string | null;
  role: AdminRole;
  isActive: boolean;
  createdOnUtc: string;
  lastLoggedOnUtc: string | null;
}

export interface AdminUserFilter { search?: string; }
export type AdminPage = PaginationResponse<AdminUser>;

export interface AdminFormData {
  userName: string;
  firstName: string;
  lastName: string;
  email: string;
  role: AdminRole;
  password: string;
}

const base = "admin/users";

export const adminsApi = {
  async list(request: PaginationRequest<AdminUserFilter>, signal?: AbortSignal) {
    const params = new URLSearchParams({
      "paging.index": String(request.paging.index),
      "paging.size": String(request.paging.size),
    });
    if (request.paging.draw !== undefined) params.set("paging.draw", String(request.paging.draw));
    for (const [field, order] of Object.entries(request.orders ?? {})) params.set(`orders[${field}]`, order);
    if (request.filter?.search?.trim()) params.set("filter.search", request.filter.search.trim());
    return (await httpClient.get<AdminPage>(`${base}?${params}`, signal)).data!;
  },
  async detail(id: number) { return (await httpClient.get<AdminUser>(`${base}/${id}`)).data!; },
  async create(data: AdminFormData) { return (await httpClient.post(`${base}`, { data }, undefined)).data; },
  async update(id: number, data: Omit<AdminFormData, "password">) {
    return (await httpClient.put(`${base}/${id}`, { data })).data;
  },
  async setPassword(id: number, password: string) {
    return (await httpClient.put(`${base}/${id}/password`, { data: { password } })).data;
  },
  async setStatus(id: number, isActive: boolean) {
    return (await httpClient.patch(`${base}/${id}/status`, { data: { isActive } })).data;
  },
  async delete(id: number) { return (await httpClient.delete(`${base}/${id}`)).data; },
};
