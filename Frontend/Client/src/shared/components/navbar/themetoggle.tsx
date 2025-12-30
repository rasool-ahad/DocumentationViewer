"use client";
import { FiSun } from "react-icons/fi";
import { FiMoon } from "react-icons/fi";
import { useTheme } from "../../../shared/hooks/use-theme";

export function ThemeToggle() {
  const { theme, toggleTheme } = useTheme();

  return (
    <button onClick={toggleTheme}>
      {theme === "dark" ? <FiSun className="w-6 h-6 lg:w-7 lg:h-7 " /> : <FiMoon className="w-6 h-6 lg:w-7 lg:h-7 "/>}
    </button>
  );
}
