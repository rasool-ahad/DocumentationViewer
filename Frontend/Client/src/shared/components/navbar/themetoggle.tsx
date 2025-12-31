"use client";
import { FiSun } from "react-icons/fi";
import { FiMoon } from "react-icons/fi";
import { useTheme } from "../../../shared/hooks/use-theme";

export function ThemeToggle() {
  const { theme, toggleTheme } = useTheme();

  return (
    <button onClick={toggleTheme} className="hover:cursor-pointer hover:bg-gray-200 dark:hover:bg-gray-700 hover:rounded-full p-3">
      {theme === "dark" ? <FiSun className="w-6 h-6 lg:w-7 lg:h-7 text-zinc-200" /> : <FiMoon className="w-6 h-6 lg:w-7 lg:h-7 text-zinc-600"/>}
    </button>
  );
}
