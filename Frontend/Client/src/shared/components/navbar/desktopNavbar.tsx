import Image from "next/image";
import type { NavbarData } from "./navbarTypes";

type Props = {
  data: NavbarData;
};

export function DesktopNav({ data }: Props) {
  return (
    <header className="hidden lg:flex items-center justify-between ">
      <div className="flex items-center gap-3">
        <Image src={data.logoUrl} alt="logo" width={32} height={32} />
        <span className="font-semibold">{data.documentationTitle}</span>
      </div>
    </header>
  );
}
