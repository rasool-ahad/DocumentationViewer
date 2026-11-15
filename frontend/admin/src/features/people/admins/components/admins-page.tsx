"use client";

import { useCallback, useEffect, useState, useSyncExternalStore, type FormEvent, type ReactNode } from "react";
import { CalendarDays, Check, CircleAlert, Ellipsis, Eye, KeyRound, Mail, Pencil, Plus, RefreshCw, Search, ShieldCheck, Trash2, UserRound, UserRoundCheck, UsersRound, X } from "lucide-react";
import { AUTH_CHANGE_EVENT, tokenService } from "@/features/auth/api/token.service";
import type { RequestOrderType } from "@/shared/api/types";
import { adminsApi, type AdminFormData, type AdminPage, type AdminUser } from "../api/admins.api";
import { Badge } from "@/shared/components/ui/badge";
import { Button } from "@/shared/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/shared/components/ui/dialog";
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/shared/components/ui/alert-dialog";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from "@/shared/components/ui/dropdown-menu";
import { Input } from "@/shared/components/ui/input";
import { Label } from "@/shared/components/ui/label";
import { Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious } from "@/shared/components/ui/pagination";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/shared/components/ui/select";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/shared/components/ui/table";

type DialogMode = "create" | "edit" | "detail" | "password" | null;
type ConfirmMode = "delete" | "status" | null;
type SortMode = "newest" | "oldest" | "name";
const pageSize = 10;
const emptyForm: AdminFormData = { userName: "", firstName: "", lastName: "", email: "", role: "Admin", password: "" };

function errorMessage(error: unknown): string {
  return error instanceof Error ? error.message : "Something went wrong. Please try again.";
}

function dateText(value: string | null): string {
  return value ? new Date(value).toLocaleDateString(undefined, { year: "numeric", month: "short", day: "numeric" }) : "Never";
}

export function AdminsPage() {
  const superAdmin = useSyncExternalStore(
    callback => { window.addEventListener(AUTH_CHANGE_EVENT, callback); return () => window.removeEventListener(AUTH_CHANGE_EVENT, callback); },
    () => tokenService.isSuperAdmin(),
    () => false,
  );
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [sort, setSort] = useState<SortMode>("newest");
  const [revision, setRevision] = useState(0);
  const [result, setResult] = useState<AdminPage | null>(null);
  const [loading, setLoading] = useState(true);
  const [listError, setListError] = useState<string | null>(null);
  const [dialog, setDialog] = useState<DialogMode>(null);
  const [confirm, setConfirm] = useState<ConfirmMode>(null);
  const [selected, setSelected] = useState<AdminUser | null>(null);
  const [form, setForm] = useState<AdminFormData>(emptyForm);
  const [busy, setBusy] = useState(false);
  const [actionError, setActionError] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);

  useEffect(() => {
    if (searchInput === search) return;
    const timer = window.setTimeout(() => { setLoading(true); setPage(1); setSearch(searchInput); }, 300);
    return () => window.clearTimeout(timer);
  }, [searchInput, search]);

  useEffect(() => {
    const abort = new AbortController();
    const orders: Record<string, RequestOrderType> = sort === "name" ? { firstName: "Ascending" }
      : { createdOnUtc: sort === "newest" ? "Descending" : "Ascending" };
    adminsApi.list({ paging: { index: page - 1, size: pageSize }, filter: { search }, orders }, abort.signal)
      .then(data => { setResult(data); setListError(null); })
      .catch(error => { if (!abort.signal.aborted) { setResult(null); setListError(errorMessage(error)); } })
      .finally(() => { if (!abort.signal.aborted) setLoading(false); });
    return () => abort.abort();
  }, [page, search, sort, revision]);

  const refresh = useCallback(() => { setLoading(true); setRevision(value => value + 1); }, []);
  function goToPage(nextPage: number) {
    if (nextPage === page || nextPage < 1 || (result && nextPage > Math.ceil(result.base.total / result.base.size))) return;
    setLoading(true);
    setPage(nextPage);
  }
  function startCreate() { setSelected(null); setForm(emptyForm); setActionError(null); setDialog("create"); }
  function startEdit(user: AdminUser) {
    setSelected(user);
    setForm({ userName: user.userName, firstName: user.firstName, lastName: user.lastName,
      email: user.email ?? "", role: user.role, password: "" });
    setActionError(null);
    setDialog("edit");
  }
  async function showDetail(user: AdminUser) {
    setSelected(user); setActionError(null); setDialog("detail");
    try { setSelected(await adminsApi.detail(user.id)); }
    catch (error) { setActionError(errorMessage(error)); }
  }
  function startPassword(user: AdminUser) {
    setSelected(user); setForm({ ...emptyForm }); setActionError(null); setDialog("password");
  }
  function startConfirm(user: AdminUser, mode: ConfirmMode) {
    setSelected(user); setActionError(null); setConfirm(mode);
  }

  async function save(event: FormEvent<HTMLFormElement>) {
    event.preventDefault(); setBusy(true); setActionError(null);
    try {
      if (dialog === "create") {
        await adminsApi.create(form); setNotice("Admin account created."); setPage(1);
      } else if (dialog === "edit" && selected) {
        const { password: _password, ...data } = form;
        void _password;
        await adminsApi.update(selected.id, data); setNotice("Admin account updated.");
      } else if (dialog === "password" && selected) {
        await adminsApi.setPassword(selected.id, form.password); setNotice("Password updated.");
      }
      setDialog(null); refresh();
    } catch (error) { setActionError(errorMessage(error)); }
    finally { setBusy(false); }
  }

  async function confirmAction() {
    if (!selected) return;
    setBusy(true); setActionError(null);
    try {
      if (confirm === "delete") {
        await adminsApi.delete(selected.id); setNotice("Admin account deleted.");
        if (result?.items.length === 1 && page > 1) setPage(page - 1);
      } else if (confirm === "status") {
        await adminsApi.setStatus(selected.id, !selected.isActive);
        setNotice(selected.isActive ? "Admin account deactivated." : "Admin account activated.");
      }
      setConfirm(null); refresh();
    } catch (error) { setActionError(errorMessage(error)); }
    finally { setBusy(false); }
  }

  const totalPages = result ? Math.ceil(result.base.total / result.base.size) : 0;
  const pageNumbers = Array.from({ length: Math.min(totalPages, 5) }, (_, index) =>
    Math.max(1, Math.min(page - 2, totalPages - 4)) + index);
  const pageItems = result?.items ?? [];
  const activeOnPage = pageItems.filter(user => user.isActive).length;
  const superAdminsOnPage = pageItems.filter(user => user.role === "SuperAdmin").length;

  function actionsFor(user: AdminUser) {
    return <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" size="icon" aria-label={`Actions for ${user.userName}`} className="size-9 rounded-xl border border-transparent text-muted-foreground hover:border-border hover:bg-background hover:text-foreground">
          <Ellipsis className="size-4" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-52 rounded-xl p-1.5">
        <DropdownMenuItem className="gap-2 rounded-lg" onSelect={() => void showDetail(user)}><Eye className="size-4" /> View details</DropdownMenuItem>
        <DropdownMenuItem className="gap-2 rounded-lg" onSelect={() => startEdit(user)}><Pencil className="size-4" /> Edit account</DropdownMenuItem>
        <DropdownMenuItem className="gap-2 rounded-lg" onSelect={() => startPassword(user)}><KeyRound className="size-4" /> Set password</DropdownMenuItem>
        <DropdownMenuItem className="gap-2 rounded-lg" onSelect={() => startConfirm(user, "status")}>
          <UserRoundCheck className="size-4" /> {user.isActive ? "Deactivate" : "Activate"}
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem variant="destructive" className="gap-2 rounded-lg" onSelect={() => startConfirm(user, "delete")}><Trash2 className="size-4" /> Delete account</DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>;
  }

  return (
    <main className="mx-auto w-full max-w-[1440px] space-y-6 px-4 pb-12 pt-6 sm:px-7 lg:px-10 lg:pt-9">
      <section className="relative isolate overflow-hidden rounded-[28px] bg-gradient-to-br from-[#172557] via-[#253da4] to-[#6458d9] text-white shadow-[0_20px_55px_-28px_rgba(34,48,145,.72)]">
        <div className="pointer-events-none absolute -right-12 -top-40 size-[420px] rounded-full border border-white/15" aria-hidden="true" />
        <div className="pointer-events-none absolute -right-28 -top-24 size-[380px] rounded-full border border-white/10" aria-hidden="true" />
        <div className="pointer-events-none absolute bottom-[-120px] right-[19%] size-[260px] rounded-full bg-white/5 blur-2xl" aria-hidden="true" />
        <div className="relative flex flex-col gap-8 px-6 py-7 sm:px-9 sm:py-9 lg:flex-row lg:items-end lg:justify-between lg:px-11 lg:py-10">
          <div className="max-w-2xl">
            <div className="mb-5 inline-flex items-center gap-2 rounded-full border border-white/20 bg-white/10 px-3 py-1.5 text-[11px] font-semibold uppercase tracking-[0.15em] text-blue-50">
              <ShieldCheck className="size-3.5" /> People &amp; access
            </div>
            <h1 className="text-3xl font-semibold tracking-[-0.04em] sm:text-4xl lg:text-[42px]">Administrators</h1>
            <p className="mt-3 max-w-xl text-sm leading-6 text-blue-100/90 sm:text-base">A clear view of everyone who can manage your workspace. Review access, keep accounts current, and welcome new teammates.</p>
          </div>
          {superAdmin && <Button onClick={startCreate} className="h-11 w-fit gap-2 rounded-xl bg-white px-5 font-semibold text-[#243a9a] shadow-lg shadow-[#111b54]/15 hover:bg-blue-50"><Plus className="size-4" /> Add administrator</Button>}
        </div>
      </section>

      <section aria-label="Directory summary" className="grid gap-3 sm:grid-cols-3 sm:gap-4">
        <SummaryCard icon={<UsersRound className="size-5" />} label={search ? "Matching accounts" : "Administrators"} value={result && !loading ? String(result.base.total) : "—"} note={search ? "For your current search" : "In this directory"} tone="blue" />
        <SummaryCard icon={<UserRoundCheck className="size-5" />} label="Active accounts" value={result && !loading ? String(activeOnPage) : "—"} note="On this page" tone="green" />
        <SummaryCard icon={<ShieldCheck className="size-5" />} label="Super admins" value={result && !loading ? String(superAdminsOnPage) : "—"} note="On this page" tone="violet" />
      </section>

      {notice && <div role="status" className="flex items-center gap-3 rounded-2xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-800 shadow-sm dark:border-emerald-900 dark:bg-emerald-950 dark:text-emerald-200">
        <span className="flex size-7 items-center justify-center rounded-full bg-emerald-100 dark:bg-emerald-900"><Check className="size-4" /></span>
        <span className="flex-1 font-medium">{notice}</span>
        <Button variant="ghost" size="icon" className="size-8 text-current" onClick={() => setNotice(null)} aria-label="Dismiss notification"><X className="size-4" /></Button>
      </div>}

      <section className="overflow-hidden rounded-[24px] border border-border/80 bg-card shadow-[0_16px_50px_-36px_rgba(25,43,104,.4)]">
        <div className="flex flex-col gap-5 border-b border-border/70 px-5 py-5 sm:px-7 sm:py-6 lg:flex-row lg:items-center lg:justify-between">
          <div>
            <div className="flex items-center gap-3"><span className="flex size-10 items-center justify-center rounded-xl bg-blue-50 text-blue-600 dark:bg-blue-500/15 dark:text-blue-300"><UsersRound className="size-5" /></span><div><h2 className="text-lg font-semibold tracking-tight">Admin directory</h2><p className="text-xs text-muted-foreground">Accounts with workspace access</p></div></div>
          </div>
          <div className="flex w-full flex-col gap-2 sm:flex-row lg:w-auto">
            <div className="relative min-w-0 flex-1 lg:w-72"><Search className="pointer-events-none absolute left-3.5 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" /><Input aria-label="Search administrators" placeholder="Search name, username, email" value={searchInput} onChange={event => setSearchInput(event.target.value)} className="h-10 rounded-xl bg-background pl-10 pr-9" />{searchInput && <button type="button" onClick={() => setSearchInput("")} aria-label="Clear search" className="absolute right-2 top-1/2 flex size-7 -translate-y-1/2 items-center justify-center rounded-lg text-muted-foreground hover:bg-muted hover:text-foreground"><X className="size-4" /></button>}</div>
            <Select value={sort} onValueChange={value => { setSort(value as SortMode); setPage(1); setLoading(true); }}><SelectTrigger aria-label="Sort administrators" className="h-10 w-full rounded-xl bg-background sm:w-42"><SelectValue /></SelectTrigger><SelectContent><SelectItem value="newest">Newest first</SelectItem><SelectItem value="oldest">Oldest first</SelectItem><SelectItem value="name">Name A–Z</SelectItem></SelectContent></Select>
            <Button variant="outline" size="icon" className="hidden size-10 rounded-xl sm:inline-flex" aria-label="Refresh administrators" onClick={refresh}><RefreshCw className="size-4" /></Button>
          </div>
        </div>

        {listError && <div role="alert" className="mx-5 mt-5 flex items-center gap-3 rounded-xl border border-destructive/20 bg-destructive/5 p-3 text-sm text-destructive sm:mx-7"><CircleAlert className="size-4 shrink-0" /><span className="flex-1">{listError}</span><Button variant="outline" size="sm" onClick={refresh}>Retry</Button></div>}

        {loading ? <div className="space-y-3 px-5 py-7 sm:px-7">{Array.from({ length: 5 }, (_, index) => <div key={index} className="flex animate-pulse items-center gap-4 rounded-xl border border-border/60 px-4 py-3"><div className="size-10 rounded-full bg-muted" /><div className="flex-1 space-y-2"><div className="h-3 w-36 rounded bg-muted" /><div className="h-2.5 w-52 max-w-full rounded bg-muted" /></div><div className="hidden h-6 w-20 rounded-full bg-muted sm:block" /></div>)}</div> :
          listError && !result ? <div className="px-6 py-14 text-center text-sm text-muted-foreground">The directory could not be loaded. Use Retry to try again.</div> :
          pageItems.length === 0 ? <div className="flex flex-col items-center px-6 py-16 text-center"><span className="mb-4 flex size-16 items-center justify-center rounded-2xl bg-blue-50 text-blue-500 dark:bg-blue-500/15"><Search className="size-7" /></span><h3 className="text-lg font-semibold">{search ? "No matching administrators" : "No administrators yet"}</h3><p className="mt-2 max-w-sm text-sm leading-6 text-muted-foreground">{search ? "Try another name, username, or email address." : "Add an administrator to start managing workspace access."}</p>{search && <Button variant="outline" className="mt-5 rounded-xl" onClick={() => setSearchInput("")}>Clear search</Button>}{!search && superAdmin && <Button className="mt-5 gap-2 rounded-xl" onClick={startCreate}><Plus className="size-4" /> Add administrator</Button>}</div> : <>
            <div className="hidden overflow-x-auto md:block">
              <Table>
                <TableHeader className="bg-muted/40"><TableRow className="hover:bg-transparent"><TableHead className="h-12 pl-7 text-xs font-semibold uppercase tracking-wider text-muted-foreground">Administrator</TableHead><TableHead className="text-xs font-semibold uppercase tracking-wider text-muted-foreground">Access</TableHead><TableHead className="text-xs font-semibold uppercase tracking-wider text-muted-foreground">Status</TableHead><TableHead className="text-xs font-semibold uppercase tracking-wider text-muted-foreground">Created</TableHead><TableHead className="text-xs font-semibold uppercase tracking-wider text-muted-foreground">Last login</TableHead>{superAdmin && <TableHead className="w-16 pr-7"><span className="sr-only">Actions</span></TableHead>}</TableRow></TableHeader>
                <TableBody>{pageItems.map(user => <TableRow key={user.id} className="border-border/60 hover:bg-blue-50/40 dark:hover:bg-blue-500/5">
                  <TableCell className="py-4 pl-7"><div className="flex min-w-55 items-center gap-3"><AdminAvatar user={user} /><div className="min-w-0"><p className="truncate font-semibold text-foreground">{user.firstName} {user.lastName}</p><p className="truncate text-xs text-muted-foreground">@{user.userName}{user.email ? ` · ${user.email}` : ""}</p></div></div></TableCell>
                  <TableCell><RolePill role={user.role} /></TableCell><TableCell><StatusPill active={user.isActive} /></TableCell>
                  <TableCell className="text-sm text-muted-foreground">{dateText(user.createdOnUtc)}</TableCell><TableCell className="text-sm text-muted-foreground">{dateText(user.lastLoggedOnUtc)}</TableCell>
                  {superAdmin && <TableCell className="pr-7 text-right">{actionsFor(user)}</TableCell>}
                </TableRow>)}</TableBody>
              </Table>
            </div>
            <div className="divide-y divide-border/70 md:hidden">{pageItems.map(user => <div key={user.id} className="space-y-3 px-5 py-4">
              <div className="flex items-start gap-3"><AdminAvatar user={user} /><div className="min-w-0 flex-1"><p className="truncate font-semibold">{user.firstName} {user.lastName}</p><p className="truncate text-xs text-muted-foreground">@{user.userName}</p></div>{superAdmin && actionsFor(user)}</div>
              <div className="flex flex-wrap gap-2"><RolePill role={user.role} /><StatusPill active={user.isActive} /></div>
              {user.email && <p className="flex items-center gap-2 truncate text-xs text-muted-foreground"><Mail className="size-3.5 shrink-0" /> {user.email}</p>}
              <p className="flex items-center gap-2 text-xs text-muted-foreground"><CalendarDays className="size-3.5" /> Joined {dateText(user.createdOnUtc)}</p>
            </div>)}</div>
          </>}

        <div className="flex flex-col gap-3 border-t border-border/70 px-5 py-4 sm:flex-row sm:items-center sm:justify-between sm:px-7">
          <p className="text-sm text-muted-foreground">{result?.base.total ? `Showing ${result.base.index * result.base.size + 1}–${Math.min((result.base.index + 1) * result.base.size, result.base.total)} of ${result.base.total}` : "No results"}</p>
          {totalPages > 1 && <Pagination className="mx-0 w-auto justify-start sm:justify-end"><PaginationContent>
            <PaginationItem><PaginationPrevious href="#" aria-disabled={page === 1} className={page === 1 ? "pointer-events-none opacity-50" : ""} onClick={event => { event.preventDefault(); goToPage(page - 1); }} /></PaginationItem>
            {pageNumbers.map(number => <PaginationItem key={number}><PaginationLink href="#" isActive={number === page} onClick={event => { event.preventDefault(); goToPage(number); }}>{number}</PaginationLink></PaginationItem>)}
            <PaginationItem><PaginationNext href="#" aria-disabled={page >= totalPages} className={page >= totalPages ? "pointer-events-none opacity-50" : ""} onClick={event => { event.preventDefault(); goToPage(page + 1); }} /></PaginationItem>
          </PaginationContent></Pagination>}
        </div>
      </section>

      <Dialog open={dialog !== null} onOpenChange={open => { if (!open && !busy) setDialog(null); }}>
        <DialogContent className="max-h-[90vh] overflow-y-auto rounded-2xl p-0 sm:max-w-xl">
          <div className="border-b border-border/70 bg-muted/30 px-6 py-6 sm:px-7">
            <span className="mb-4 flex size-11 items-center justify-center rounded-xl bg-primary/10 text-primary">{dialog === "password" ? <KeyRound className="size-5" /> : dialog === "detail" ? <UserRound className="size-5" /> : <ShieldCheck className="size-5" />}</span>
            <DialogHeader><DialogTitle className="text-xl tracking-tight">{dialog === "create" ? "Add administrator" : dialog === "edit" ? "Edit administrator" : dialog === "password" ? "Set password" : "Administrator details"}</DialogTitle>
              <DialogDescription className="pt-1">{dialog === "create" ? "Create an account with access to your workspace." : dialog === "edit" ? "Update account details and access level." : dialog === "password" ? "Set a new password for this account." : "Account information and recent activity."}</DialogDescription></DialogHeader>
          </div>
          {dialog === "detail" && selected && <div className="px-6 pb-6 sm:px-7">
            <div className="mb-5 flex items-center gap-3 rounded-xl border border-border/70 bg-muted/30 p-3"><AdminAvatar user={selected} /><div className="min-w-0"><p className="truncate font-semibold">{selected.firstName} {selected.lastName}</p><p className="truncate text-xs text-muted-foreground">@{selected.userName}</p></div></div>
            <div className="grid gap-4 text-sm sm:grid-cols-2"><Detail label="Email" value={selected.email ?? "—"} /><Detail label="Role" value={selected.role === "SuperAdmin" ? "Super admin" : "Admin"} /><Detail label="Status" value={selected.isActive ? "Active" : "Inactive"} /><Detail label="Created" value={dateText(selected.createdOnUtc)} /><Detail label="Last login" value={dateText(selected.lastLoggedOnUtc)} /></div>
          </div>}
          {(dialog === "create" || dialog === "edit" || dialog === "password") && <form id="admin-form" onSubmit={event => void save(event)} className="space-y-4 px-6 pb-2 sm:px-7">
            {dialog !== "password" && <>
              <div className="grid gap-3 sm:grid-cols-2"><Field label="First name" value={form.firstName} onChange={value => setForm({ ...form, firstName: value })} required /><Field label="Last name" value={form.lastName} onChange={value => setForm({ ...form, lastName: value })} required /></div>
              <Field label="Username" value={form.userName} onChange={value => setForm({ ...form, userName: value })} required />
              <Field label="Email" type="email" value={form.email} onChange={value => setForm({ ...form, email: value })} />
              <div className="space-y-2"><Label>Access level</Label><Select value={form.role} onValueChange={value => setForm({ ...form, role: value as AdminFormData["role"] })}><SelectTrigger className="h-10 w-full rounded-xl"><SelectValue /></SelectTrigger><SelectContent><SelectItem value="Admin">Admin</SelectItem><SelectItem value="SuperAdmin">Super admin</SelectItem></SelectContent></Select><p className="text-xs text-muted-foreground">Super admins can manage other administrator accounts.</p></div>
            </>}
            {(dialog === "create" || dialog === "password") && <Field label={dialog === "create" ? "Password" : "New password"} type="password" value={form.password} onChange={value => setForm({ ...form, password: value })} required minLength={8} />}
          </form>}
          {actionError && <p role="alert" className="mx-6 rounded-xl bg-destructive/10 px-3 py-2 text-sm text-destructive sm:mx-7">{actionError}</p>}
          {dialog !== "detail" && <DialogFooter className="border-t border-border/70 px-6 py-4 sm:px-7"><Button type="submit" form="admin-form" disabled={busy} className="h-10 rounded-xl px-5">{busy ? "Saving…" : dialog === "create" ? "Create account" : dialog === "password" ? "Set password" : "Save changes"}</Button></DialogFooter>}
        </DialogContent>
      </Dialog>

      <AlertDialog open={confirm !== null} onOpenChange={open => { if (!open && !busy) setConfirm(null); }}>
        <AlertDialogContent className="rounded-2xl"><div className={`flex size-11 items-center justify-center rounded-xl ${confirm === "delete" ? "bg-destructive/10 text-destructive" : "bg-primary/10 text-primary"}`}>{confirm === "delete" ? <Trash2 className="size-5" /> : <UserRoundCheck className="size-5" />}</div><AlertDialogHeader><AlertDialogTitle>{confirm === "delete" ? "Delete administrator?" : selected?.isActive ? "Deactivate administrator?" : "Activate administrator?"}</AlertDialogTitle>
          <AlertDialogDescription>{confirm === "delete" ? `Delete ${selected?.userName}'s account? They will lose access immediately.` : selected?.isActive ? `${selected?.userName} will no longer be able to access the admin workspace.` : `${selected?.userName} will be able to sign in again.`}</AlertDialogDescription></AlertDialogHeader>
          {actionError && <p role="alert" className="text-sm text-destructive">{actionError}</p>}
          <AlertDialogFooter><AlertDialogCancel disabled={busy} className="rounded-xl">Cancel</AlertDialogCancel><AlertDialogAction disabled={busy} className={confirm === "delete" ? "rounded-xl bg-destructive text-white hover:bg-destructive/90" : "rounded-xl"} onClick={event => { event.preventDefault(); void confirmAction(); }}>{busy ? "Working…" : confirm === "delete" ? "Delete account" : selected?.isActive ? "Deactivate" : "Activate"}</AlertDialogAction></AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </main>
  );
}

function Detail({ label, value }: { label: string; value: string }) {
  return <div className="rounded-xl border border-border/70 bg-card p-3.5"><p className="text-xs text-muted-foreground">{label}</p><p className="mt-1.5 break-words font-medium">{value}</p></div>;
}

function Field({ label, value, onChange, type = "text", required, minLength }: { label: string; value: string; onChange: (value: string) => void; type?: string; required?: boolean; minLength?: number }) {
  const id = `admin-${label.toLowerCase().replaceAll(" ", "-")}`;
  return <div className="space-y-2"><Label htmlFor={id}>{label}</Label><Input id={id} type={type} value={value} onChange={event => onChange(event.target.value)} required={required} minLength={minLength} autoComplete={type === "password" ? "new-password" : "off"} className="h-10 rounded-xl" /></div>;
}

function SummaryCard({ icon, label, value, note, tone }: { icon: ReactNode; label: string; value: string; note: string; tone: "blue" | "green" | "violet" }) {
  const colors = { blue: "bg-blue-50 text-blue-600 dark:bg-blue-500/15 dark:text-blue-300", green: "bg-emerald-50 text-emerald-600 dark:bg-emerald-500/15 dark:text-emerald-300", violet: "bg-violet-50 text-violet-600 dark:bg-violet-500/15 dark:text-violet-300" };
  return <div className="flex items-center gap-4 rounded-2xl border border-border/80 bg-card px-5 py-4 shadow-[0_12px_30px_-28px_rgba(25,43,104,.45)]"><span className={`flex size-11 shrink-0 items-center justify-center rounded-xl ${colors[tone]}`}>{icon}</span><div className="min-w-0"><p className="text-xs font-medium text-muted-foreground">{label}</p><p className="mt-0.5 text-2xl font-semibold leading-none tracking-tight">{value}</p><p className="mt-1 text-[11px] text-muted-foreground">{note}</p></div></div>;
}

function AdminAvatar({ user }: { user: AdminUser }) {
  const colors = ["bg-blue-100 text-blue-700 dark:bg-blue-500/20 dark:text-blue-200", "bg-violet-100 text-violet-700 dark:bg-violet-500/20 dark:text-violet-200", "bg-amber-100 text-amber-700 dark:bg-amber-500/20 dark:text-amber-200", "bg-emerald-100 text-emerald-700 dark:bg-emerald-500/20 dark:text-emerald-200"];
  const initials = `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase() || "AD";
  return <span aria-hidden="true" className={`flex size-10 shrink-0 items-center justify-center rounded-xl text-xs font-bold ring-1 ring-inset ring-black/5 ${colors[user.id % colors.length]}`}>{initials}</span>;
}

function RolePill({ role }: { role: AdminUser["role"] }) {
  return <Badge variant="outline" className={role === "SuperAdmin" ? "gap-1 rounded-full border-violet-200 bg-violet-50 px-2.5 py-1 font-medium text-violet-700 dark:border-violet-500/30 dark:bg-violet-500/10 dark:text-violet-200" : "gap-1 rounded-full border-blue-200 bg-blue-50 px-2.5 py-1 font-medium text-blue-700 dark:border-blue-500/30 dark:bg-blue-500/10 dark:text-blue-200"}>{role === "SuperAdmin" && <ShieldCheck className="size-3" />}{role === "SuperAdmin" ? "Super admin" : "Admin"}</Badge>;
}

function StatusPill({ active }: { active: boolean }) {
  return <span className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-medium ${active ? "bg-emerald-50 text-emerald-700 dark:bg-emerald-500/10 dark:text-emerald-200" : "bg-slate-100 text-slate-600 dark:bg-slate-500/15 dark:text-slate-300"}`}><span className={`size-1.5 rounded-full ${active ? "bg-emerald-500" : "bg-slate-400"}`} />{active ? "Active" : "Inactive"}</span>;
}
