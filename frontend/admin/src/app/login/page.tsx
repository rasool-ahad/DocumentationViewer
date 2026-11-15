import Image from "next/image";
import { AuthGate } from "@/features/auth/components/auth-gate";
import { LoginForm } from "@/features/auth/components/login-form";
import { BrandIcon } from "@/shared/components/brand-icon";
import { ThemeSwitch } from "@/shared/components/theme-switch";

export default function LoginPage() {
  return (
    <AuthGate requireAuth={false}><main className="login-page">
      <div className="login-shell">
        <section className="promo-panel" aria-label="About DocViewer">
          <div className="brand"><BrandIcon className="size-10" /><span>DocViewer</span></div>
          <div className="promo-copy">
            <h2>Your Documentation,<br /><span>Organized &amp; Accessible</span></h2>
            <p>Create, manage and share beautiful documentation with a modern and intuitive experience.</p>
          </div>
          <Image className="promo-art" src="/document-illustration.svg" width={630} height={380} alt="Illustration of organized documents" priority />
        </section>
        <section className="form-panel" aria-labelledby="login-title">
          <ThemeSwitch />
          <div className="form-content">
            <header className="login-heading">
              <BrandIcon className="login-mark" />
              <h1 id="login-title">Welcome Back!</h1>
              <p>Sign in to your account to continue</p>
            </header>
            <LoginForm />
          </div>
        </section>
      </div>
    </main></AuthGate>
  );
}
