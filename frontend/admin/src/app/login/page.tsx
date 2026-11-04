import Image from "next/image";
import { BrandIcon } from "@/features/auth/components/brand-icon";
import { LoginForm } from "@/features/auth/components/login-form";
import { ThemeSwitch } from "@/features/auth/components/theme-switch";

export default function LoginPage() {
  return (
    <main className="login-page">
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
    </main>
  );
}
