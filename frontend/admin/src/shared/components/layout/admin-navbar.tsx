"use client";

import { usePathname, useRouter } from "next/navigation";
import { ChevronDown, LogOut } from "lucide-react";
import { useState } from "react";
import { authApi } from "@/features/auth/api/auth.api";
import { tokenService } from "@/features/auth/api/token.service";
import { HttpError } from "@/shared/api/http-client";
import { ThemeSwitch } from "@/shared/components/theme-switch";
import { Avatar, AvatarFallback } from "@/shared/components/ui/avatar";
import { Button } from "@/shared/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger } from "@/shared/components/ui/dropdown-menu";
import { Separator } from "@/shared/components/ui/separator";
import { SidebarTrigger } from "@/shared/components/ui/sidebar";

export function AdminNavbar() {
  const router = useRouter();
  const pathname = usePathname();
  const section = pathname.startsWith("/people") ? "People / Administrators" : "Overview";
  const [isSigningOut, setIsSigningOut] = useState(false);
  const [signOutError, setSignOutError] = useState<string | null>(null);

  async function signOut() {
    if (isSigningOut) return;
    setIsSigningOut(true);
    setSignOutError(null);
    try {
      await authApi.signOut();
      tokenService.clear();
      router.replace("/login");
    } catch (error) {
      if (error instanceof HttpError && error.status === 401) {
        tokenService.clear();
        router.replace("/login");
      } else {
        setSignOutError("Could not sign out. Please try again.");
      }
    } finally {
      setIsSigningOut(false);
    }
  }

  return (
    <header className="flex h-18 shrink-0 items-center gap-3 border-b border-border/70 bg-card px-4 sm:px-7">
      <SidebarTrigger className="size-9 rounded-lg border border-border/70 md:hidden" />
      <Separator orientation="vertical" className="h-5 md:hidden" />
      <div className="min-w-0">
        <p className="text-xs text-muted-foreground">Workspace <span className="mx-1 text-border">/</span> {section}</p>
        <p className="font-semibold leading-5 tracking-tight sm:hidden">{section}</p>
      </div>
      <div className="ml-auto flex items-center gap-2 sm:gap-3">
        <ThemeSwitch className="admin-theme-switch" />
        <Separator orientation="vertical" className="mx-1 hidden h-6 sm:block" />
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" className="h-10 gap-2 rounded-xl px-1.5 sm:px-2" aria-label="Open account menu">
              <Avatar className="size-8"><AvatarFallback className="bg-primary/10 font-semibold text-primary">AD</AvatarFallback></Avatar>
              <span className="hidden text-left sm:block"><span className="block text-xs font-semibold leading-4">Admin account</span><span className="block text-[11px] font-normal leading-4 text-muted-foreground">Workspace access</span></span>
              <ChevronDown className="hidden size-4 text-muted-foreground sm:block" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-52">
            <DropdownMenuLabel>Signed in</DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem disabled={isSigningOut} onSelect={(event) => { event.preventDefault(); void signOut(); }}><LogOut /> {isSigningOut ? "Signing out…" : "Sign out"}</DropdownMenuItem>
            {signOutError && <p role="alert" className="px-2 py-1.5 text-xs text-destructive">{signOutError}</p>}
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </header>
  );
}
