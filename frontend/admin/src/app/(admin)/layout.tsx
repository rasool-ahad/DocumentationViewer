import { AuthGate } from "@/features/auth/components/auth-gate";
import { AdminShell } from "@/shared/components/layout/admin-shell";

export default function AdminLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return <AuthGate requireAuth><AdminShell>{children}</AdminShell></AuthGate>;
}
