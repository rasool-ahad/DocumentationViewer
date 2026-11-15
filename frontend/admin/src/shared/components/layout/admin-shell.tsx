"use client";

import type { ReactNode } from "react";
import { AdminNavbar } from "./admin-navbar";
import { AdminSidebar } from "./admin-sidebar";
import { SidebarInset, SidebarProvider } from "@/shared/components/ui/sidebar";

export function AdminShell({ children }: { children: ReactNode }) {
  return (
    <SidebarProvider>
      <AdminSidebar />
      <SidebarInset className="min-w-0 bg-[#f6f8fc] dark:bg-[#11172b]">
        <AdminNavbar />
        {children}
      </SidebarInset>
    </SidebarProvider>
  );
}
