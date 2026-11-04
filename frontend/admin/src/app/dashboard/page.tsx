"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { Button } from "@/shared/components/ui/button";
import { tokenService } from "@/features/auth/api/token.service";

export default function DashboardPage() {
  const router = useRouter();
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    if (!tokenService.getAccessToken()) router.replace("/login");
    else queueMicrotask(() => setIsAuthenticated(true));
  }, [router]);

  if (!isAuthenticated) return null;

  return (
    <main className="min-h-screen bg-background p-8 text-foreground">
      <div className="mx-auto flex max-w-5xl items-center justify-between">
        <h1 className="text-2xl font-bold">DocViewer Admin</h1>
        <Button variant="outline" onClick={() => { tokenService.clear(); router.replace("/login"); }}>Sign out</Button>
      </div>
      <p className="mx-auto mt-16 max-w-5xl text-muted-foreground">You are signed in. The admin dashboard can be built here.</p>
    </main>
  );
}
