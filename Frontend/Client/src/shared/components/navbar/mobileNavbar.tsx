"use client";

import { ThemeToggle } from "./themetoggle";
import { useState } from "react";
import Link from "next/link";
import type { NavItem } from "../sidebar/sidebarTypes";
import { cn } from "../../utils/cn";
import Image from "next/image";
import { NavbarData } from "./navbarTypes";
import { FiSearch } from "react-icons/fi";
import { IoMenu, IoClose } from "react-icons/io5";

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
<div className="relative lg:hidden  h-full ">
  <div className="flex items-center justify-between  h-full">
    <div className="flex items-center justify-center gap-2">
      <button
        onClick={() => setIsOpen((p) => !p)}
        className="hover:bg-gray-200 dark:hover:bg-gray-700 hover:rounded-full p-3  cursor-pointer z-50"
        aria-label={isOpen ? "بستن منو" : "باز کردن منو"}
        aria-expanded={isOpen}
      >
        {isOpen ? (
          <IoClose className="w-7 h-7 text-zinc-600 dark:text-zinc-200" />
        ) : (
          <IoMenu className="w-7 h-7 text-zinc-600 dark:text-zinc-200" />
        )}
      </button>

      <Image src={data.logoUrl} alt="" width={32} height={32} />
      <p className="font-medium text-gray-400">{data.documentationTitle}</p>
    </div>

    <div className="flex items-center justify-center gap-5">
      <button className="hover:cursor-pointer hover:bg-gray-200 dark:hover:bg-gray-700 hover:rounded-full p-3">
        <FiSearch className="w-6 h-6 lg:w-7 lg:h-7 text-zinc-600 dark:text-zinc-200" />
      </button>
      <ThemeToggle />
    </div>
  </div>

  {/* پنل منو دقیقاً زیر نوبار */}
{isOpen && (
  <div
    className={cn(
      "absolute left-0 right-0 top-full z-40",
      "h-[calc(100vh-64px)]", // 64px ارتفاع نوبار (اگر نوبارت فرق داره تغییر بده)
      ""
    )}
  >
    <div className="h-full overflow-y-auto pt-6 px-6">
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
  </div>
)}

</div>

    </>
  );
};

export default MobileNav;
