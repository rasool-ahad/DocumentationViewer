"use client";

import { ThemeToggle } from "./themetoggle";
import { useState } from "react";
import Link from "next/link";
import type { NavItem } from "../sidebar/sidebarTypes";
import { cn } from "../../utils/cn";
import Image from "next/image";

interface MobileNavProps {
  navItems: NavItem[];
}

const MobileNav: React.FC<MobileNavProps> = ({ navItems }) => {
  const [isOpen, setIsOpen] = useState(false);

  const handleLinkClick = () => {
    setIsOpen(false);
  };

  return (
    <>
      <div className="flex items-center justify-center lg:hidden">
        <div className="flex items-center justify-center gap-2">
          <button
            onClick={() => setIsOpen(!isOpen)}
            className="p-2 rounded-lg text-white hover:bg-slate-800 transition-colors"
            aria-label="منو"
          >
            <div className="w-6 h-6 flex flex-col justify-between">
              <span
                className={cn(
                  "w-full h-0.5 bg-white transition-all",
                  isOpen && "rotate-45 translate-y-2.5"
                )}
              />
              <span
                className={cn(
                  "w-full h-0.5 bg-white transition-all",
                  isOpen && "opacity-0"
                )}
              />
              <span
                className={cn(
                  "w-full h-0.5 bg-white transition-all",
                  isOpen && "-rotate-45 -translate-y-2.5"
                )}
              />
            </div>
          </button>
          <Image src="" alt="logo" width={32} height={32} />
          <span className="font-semibold"></span>
        </div>
        <div className="flex items-center justify-center gap-2">
          <ThemeToggle />
        </div>
      </div>
  <div className={cn(
        "fixed inset-0 z-40 bg-slate-900/95 backdrop-blur-lg transition-all",
        "pt-24 px-6 overflow-y-auto",
        isOpen ? "translate-x-0 opacity-100" : "translate-x-full opacity-0"
      )}>
        <div className="space-y-10">
          {navItems.map(section => (
            <div key={section.id} className="space-y-4">
              {/* main item */}
              <div className="text-xl font-semibold text-white">
                {section.label}
              </div>

              {/* sub items */}
              {section.children && (
                <div className="flex flex-col gap-3 pl-4">
                  {section.children.map(child => (
                    <Link
                      key={child.id}
                      href={child.href ?? "#"}
                      onClick={handleLinkClick}
                      className="text-sm text-slate-300 hover:text-green-400 transition"
                    >
                      {child.label}
                    </Link>
                  ))}
                </div>
              )}

              {/* item بدون ساب‌منو */}
              {!section.children && section.href && (
                <Link
                  href={section.href}
                  onClick={handleLinkClick}
                  className="text-lg text-white hover:text-green-400 transition"
                >
                  {section.label}
                </Link>
              )}
            </div>
          ))}
        </div>
      </div>
    </>
  );
};

export default MobileNav;
