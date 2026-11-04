import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "DocViewer Admin | Sign In",
  description: "Sign in to the DocViewer administration panel.",
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return <html lang="en" suppressHydrationWarning><body>{children}</body></html>;
}
