"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { FileText, LayoutDashboard, Settings2, UsersRound } from "lucide-react";
import { BrandIcon } from "@/shared/components/brand-icon";
import {
  Sidebar, SidebarContent, SidebarFooter, SidebarGroup, SidebarGroupContent,
  SidebarGroupLabel, SidebarHeader, SidebarMenu, SidebarMenuBadge,
  SidebarMenuButton, SidebarMenuItem, SidebarSeparator,
} from "@/shared/components/ui/sidebar";

export function AdminSidebar() {
  const pathname = usePathname();
  return (
    <Sidebar collapsible="icon" className="border-r border-sidebar-border">
      <SidebarHeader className="px-4 pb-5 pt-6 group-data-[collapsible=icon]:px-2">
        <Link href="/dashboard" className="flex items-center gap-3 rounded-lg outline-none focus-visible:ring-2 focus-visible:ring-sidebar-ring">
          <span className="flex size-10 shrink-0 items-center justify-center rounded-xl bg-sidebar-primary text-sidebar-primary-foreground"><BrandIcon className="size-5" /></span>
          <span className="min-w-0 group-data-[collapsible=icon]:hidden">
            <span className="block text-base font-bold leading-tight tracking-tight text-white">DocViewer</span>
            <span className="block text-[11px] font-medium tracking-[0.12em] text-sidebar-foreground/60">ADMIN CONSOLE</span>
          </span>
        </Link>
      </SidebarHeader>
      <SidebarSeparator />
      <SidebarContent className="pt-5">
        <SidebarGroup>
          <SidebarGroupLabel className="px-3 text-[11px] font-semibold uppercase tracking-[0.15em]">Workspace</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu className="gap-1.5">
              <SidebarMenuItem>
                <SidebarMenuButton asChild isActive={pathname === "/dashboard"} tooltip="Overview" size="lg" className="rounded-lg px-3 text-sm">
                  <Link href="/dashboard"><LayoutDashboard className="size-4.5" /><span>Overview</span></Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
              <SidebarMenuItem>
                <SidebarMenuButton disabled size="lg" className="rounded-lg px-3" title="Coming soon"><FileText /><span>Documents</span></SidebarMenuButton>
                <SidebarMenuBadge>Soon</SidebarMenuBadge>
              </SidebarMenuItem>
              <SidebarMenuItem>
                <SidebarMenuButton asChild isActive={pathname.startsWith("/people")} tooltip="People" size="lg" className="rounded-lg px-3 text-sm"><Link href="/people/admins"><UsersRound /><span>People</span></Link></SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
      <SidebarFooter className="gap-3 px-4 pb-5 group-data-[collapsible=icon]:px-2">
        <SidebarSeparator className="mx-0" />
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton disabled size="lg" className="rounded-lg px-3" title="Coming soon"><Settings2 /><span>Settings</span></SidebarMenuButton>
            <SidebarMenuBadge>Soon</SidebarMenuBadge>
          </SidebarMenuItem>
        </SidebarMenu>
        <div className="rounded-xl border border-white/10 bg-white/5 p-3 text-xs leading-5 text-sidebar-foreground/70 group-data-[collapsible=icon]:hidden">
          Your workspace is ready for what comes next.
        </div>
      </SidebarFooter>
    </Sidebar>
  );
}
