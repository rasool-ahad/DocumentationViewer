"use client";

import { createContext, useContext, useEffect, useState } from "react";
import type { Theme } from "../shared/theme/theme.type";
import { storage } from "../shared/utils/storage";

interface ThemeContextValue {
  theme: Theme;
  toggleTheme: () => void;
}

const ThemeContext = createContext<ThemeContextValue | null>(null);

export function ThemeProvider({ children }: { children: React.ReactNode }) {
  const [theme, setTheme] = useState<Theme>("dark");

 
  useEffect(() => {
    const storedTheme = storage.get<Theme | null>("theme", null);

    if (storedTheme) {
      setTheme(storedTheme);
      document.documentElement.dataset.theme = storedTheme;
      return;
    }

    const systemTheme: Theme = window.matchMedia(
      "(prefers-color-scheme: dark)"
    ).matches
      ? "dark"
      : "light";

    setTheme(systemTheme);
    document.documentElement.dataset.theme = systemTheme;
  }, []);


  const toggleTheme = () => {
    const newTheme: Theme = theme === "dark" ? "light" : "dark";
    setTheme(newTheme);
    storage.set("theme", newTheme);
    document.documentElement.dataset.theme = newTheme;
  };

  return (
    <ThemeContext.Provider value={{ theme, toggleTheme }}>
      {children}
    </ThemeContext.Provider>
  );
}

export function useThemeContext() {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error("useThemeContext must be used inside ThemeProvider");
  }
  return context;
}
