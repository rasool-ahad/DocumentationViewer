import { ArrowUpRight, FileText, ShieldCheck, Sparkles, UsersRound } from "lucide-react";
import Link from "next/link";
import { Badge } from "@/shared/components/ui/badge";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/shared/components/ui/card";

export default function DashboardPage() {
  return (
    <div className="mx-auto w-full max-w-6xl space-y-8 px-5 py-8 sm:px-8 lg:px-10 lg:py-10">
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.18em] text-primary">Workspace overview</p>
          <h1 className="mt-3 text-3xl font-semibold tracking-tight sm:text-4xl">Welcome to DocViewer</h1>
          <p className="mt-3 max-w-2xl text-sm leading-6 text-muted-foreground sm:text-base">
            Your admin space is ready. Use the navigation to explore the layout as more tools are added.
          </p>
        </div>
        <Badge variant="outline" className="gap-1.5 rounded-full px-3 py-1 text-xs"><Sparkles className="size-3.5" /> Admin workspace</Badge>
      </div>

      <Card className="overflow-hidden border-primary/10 bg-gradient-to-br from-[#173b9a] via-[#214fc4] to-[#5646d8] text-white shadow-lg shadow-blue-900/10">
        <CardContent className="relative flex min-h-56 flex-col justify-between gap-8 py-2 sm:flex-row sm:items-end">
          <div className="max-w-lg">
            <div className="mb-6 flex size-11 items-center justify-center rounded-xl bg-white/15 ring-1 ring-white/25"><FileText className="size-5" /></div>
            <h2 className="text-2xl font-semibold tracking-tight">Everything in one place</h2>
            <p className="mt-2 max-w-md text-sm leading-6 text-blue-50/90">A clear home for your documents, people, and workspace settings.</p>
          </div>
          <div className="flex items-center gap-2 text-sm font-medium text-blue-50"><span>Dashboard preview</span><ArrowUpRight className="size-4" /></div>
          <div className="pointer-events-none absolute -right-16 -top-20 size-64 rounded-full border border-white/15" aria-hidden="true" />
          <div className="pointer-events-none absolute -right-2 -top-8 size-40 rounded-full border border-white/15" aria-hidden="true" />
        </CardContent>
      </Card>

      <div className="grid gap-5 md:grid-cols-2">
        <Card className="border-border/70 shadow-sm">
          <CardHeader>
            <div className="mb-2 flex size-10 items-center justify-center rounded-xl bg-blue-50 text-blue-600 dark:bg-blue-500/15 dark:text-blue-300"><FileText className="size-5" /></div>
            <CardTitle className="text-lg">Documents</CardTitle>
            <CardDescription>Keep your content organized and easy to find.</CardDescription>
          </CardHeader>
          <CardContent className="flex items-center gap-2 border-t border-border/70 pt-5 text-sm text-muted-foreground"><span className="size-1.5 rounded-full bg-amber-400" /> Coming soon</CardContent>
        </Card>
        <Card className="border-border/70 shadow-sm">
          <CardHeader>
            <div className="mb-2 flex size-10 items-center justify-center rounded-xl bg-violet-50 text-violet-600 dark:bg-violet-500/15 dark:text-violet-300"><UsersRound className="size-5" /></div>
            <CardTitle className="text-lg">People &amp; access</CardTitle>
            <CardDescription>Manage who can use your workspace.</CardDescription>
          </CardHeader>
          <CardContent className="border-t border-border/70 pt-5 text-sm"><Link href="/people/admins" className="flex items-center gap-2 font-medium text-primary hover:underline"><ShieldCheck className="size-4" /> Manage administrators <ArrowUpRight className="size-4" /></Link></CardContent>
        </Card>
      </div>
    </div>
  );
}
