"use client";

import { useRouter } from "next/navigation";
import { useEffect } from "react";
import { tokenService } from "@/features/auth/api/token.service";

export default function Home() {
  const router = useRouter();

  useEffect(() => {
    router.replace(tokenService.getAccessToken() ? "/dashboard" : "/login");
  }, [router]);

  return null;
}
