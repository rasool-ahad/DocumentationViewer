"use client";

import { ArrowRight, Eye, EyeOff, LockKeyhole, UserRound } from "lucide-react";
import { useRouter } from "next/navigation";
import { useState, type FormEvent } from "react";
import { Button } from "@/shared/components/ui/button";
import { Checkbox } from "@/shared/components/ui/checkbox";
import { Input } from "@/shared/components/ui/input";
import { Label } from "@/shared/components/ui/label";
import { useLogin } from "../hooks/use-login";

export function LoginForm() {
  const router = useRouter();
  const { login, isPending, error, clearError } = useLogin();
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [rememberMe, setRememberMe] = useState(true);
  const [showPassword, setShowPassword] = useState(false);
  const [validationError, setValidationError] = useState<string | null>(null);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setValidationError(null);
    const trimmedUserName = userName.trim();
    if (!trimmedUserName || !password) {
      setValidationError("Enter your username and password.");
      return;
    }
    const result = await login({ userName: trimmedUserName, password }, rememberMe);
    if (result) router.replace("/dashboard");
  }

  function resetError() {
    setValidationError(null);
    clearError();
  }

  const formError = validationError ?? error;

  return (
    <form className="login-form" onSubmit={handleSubmit} noValidate>
      <div className="field">
        <Label htmlFor="userName">Username</Label>
        <div className="input-wrap">
          <UserRound aria-hidden="true" />
          <Input id="userName" name="userName" type="text" autoComplete="username" placeholder="Enter your username" value={userName} onChange={(event) => { setUserName(event.target.value); resetError(); }} aria-invalid={Boolean(formError)} aria-describedby={formError ? "login-error" : undefined} required />
        </div>
      </div>
      <div className="field">
        <Label htmlFor="password">Password</Label>
        <div className="input-wrap">
          <LockKeyhole aria-hidden="true" />
          <Input id="password" name="password" type={showPassword ? "text" : "password"} autoComplete="current-password" placeholder="Enter your password" value={password} onChange={(event) => { setPassword(event.target.value); resetError(); }} aria-invalid={Boolean(formError)} aria-describedby={formError ? "login-error" : undefined} required />
          <Button className="password-toggle" type="button" variant="ghost" size="icon" aria-label={showPassword ? "Hide password" : "Show password"} aria-pressed={showPassword} onClick={() => setShowPassword((current) => !current)}>
            {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
          </Button>
        </div>
      </div>
      <div className="remember-row">
        <Checkbox id="rememberMe" checked={rememberMe} onCheckedChange={(checked) => setRememberMe(checked === true)} />
        <Label htmlFor="rememberMe">Remember me</Label>
      </div>
      {formError && <p id="login-error" className="form-error" role="alert">{formError}</p>}
      <Button className="submit-button w-full" type="submit" disabled={isPending}>
        {isPending ? "Signing in…" : "Sign In"}
        {!isPending && <ArrowRight size={19} aria-hidden="true" />}
      </Button>
    </form>
  );
}
