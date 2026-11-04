"use client";

import { Moon, Sun } from "lucide-react";
import { useEffect, useState } from "react";
import { Button } from "@/shared/components/ui/button";
import { storageService } from "@/shared/services/storage.service";

const THEME_KEY = "docviewer.admin.theme";

export function ThemeSwitch() {
  const [theme, setTheme] = useState<"light" | "dark">("light");

  useEffect(() => {
    const saved = storageService.get<"light" | "dark">(THEME_KEY, "local");
    const initial = saved === "dark" ? "dark" : "light";
    document.documentElement.classList.toggle("dark", initial === "dark");
    queueMicrotask(() => setTheme(initial));
  }, []);

  function updateTheme(next: "light" | "dark") {
    setTheme(next);
    document.documentElement.classList.toggle("dark", next === "dark");
    storageService.set(THEME_KEY, next, "local");
  }

  return (
    <div className="theme-switch" role="group" aria-label="Color theme">
      <Button type="button" variant="ghost" size="icon" data-active={theme === "light"} aria-label="Light theme" aria-pressed={theme === "light"} onClick={() => updateTheme("light")}><Sun size={19} /></Button>
      <Button type="button" variant="ghost" size="icon" data-active={theme === "dark"} aria-label="Dark theme" aria-pressed={theme === "dark"} onClick={() => updateTheme("dark")}><Moon size={19} /></Button>
    </div>
  );
}
