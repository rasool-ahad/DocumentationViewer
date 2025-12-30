import type { NavbarData } from "./navbarTypes";
import type { NavItem } from "../sidebar/sidebarTypes";
import MobileNav from "./mobileNavbar";
import { DesktopNav } from "./desktopNavbar";

type Props = {
  data: NavbarData;
  sidebarItems: NavItem[];
};

export function Navbar({ data, sidebarItems }: Props) {
  return (
    <header className="fixed h-16 top-0 left-0 right-0 z-50 mx-6 my-1 bg-slate-900/80 backdrop-blur-lg border-b border-slate-700/50">
      <DesktopNav data={data} />
      <MobileNav navItems={sidebarItems} />
    </header>
  );
}
