"use client";
import { FiSun } from "react-icons/fi";
import { FiMoon } from "react-icons/fi";
import { useTheme } from "../../../shared/hooks/use-theme";

export function ThemeToggle() {
  const { theme, toggleTheme } = useTheme();

  return (
    <button onClick={toggleTheme}>
      {theme === "dark" ? <FiSun /> : <FiMoon />}
    </button>
  );
}
