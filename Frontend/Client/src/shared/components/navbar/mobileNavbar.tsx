"use client";

import { ThemeToggle } from "./themetoggle";
import { useState } from "react";
import Link from "next/link";
import type { NavItem } from "../sidebar/sidebarTypes";
import { cn } from "../../utils/cn";
import Image from "next/image";
import { NavbarData } from "./navbarTypes";
import { FiSearch } from "react-icons/fi";
interface MobileNavProps {
  navItems: NavItem[];
  data: NavbarData;
}

const MobileNav: React.FC<MobileNavProps> = ({ navItems, data }) => {
  const [isOpen, setIsOpen] = useState(false);

  const handleLinkClick = () => {
    setIsOpen(false);
  };

  return (
    <>
      <div className="flex items-center justify-between lg:hidden">
        <div className="flex items-center justify-center gap-2">
          <button
            onClick={() => setIsOpen(!isOpen)}
            className="p-1 rounded-lg text-white  transition-colors cursor-pointer"
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
          <Image src={data.logoUrl} alt="" width={32} height={32} />
          <p className="font-medium text-gray-400">{data.documentationTitle}</p>
        </div>
        <div className="flex items-center justify-center gap-5">
          <button className="hover:cursor-pointer hover:bg-gray-200 dark:hover:bg-gray-700 hover:rounded-full p-3">
            <FiSearch className="w-6 h-6 lg:w-7 lg:h-7 text-zinc-600  dark:text-zinc-200 " />
          </button>

          <ThemeToggle />
        </div>
      </div>
      <div
        className={cn(
          "fixed inset-0 z-40 bg-slate-900/95 backdrop-blur-lg transition-all",
          "pt-24 px-6 overflow-y-auto",
          isOpen ? "translate-x-0 opacity-100" : "translate-x-full opacity-0"
        )}
      >
        <div className="space-y-10">
          {navItems.map((section) => (
            <div key={section.id} className="space-y-4">
              {/* main item */}
              <div className="text-xl font-semibold text-white">
                {section.label}
              </div>

              {/* sub items */}
              {section.children && (
                <div className="flex flex-col gap-3 pl-4">
                  {section.children.map((child) => (
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
