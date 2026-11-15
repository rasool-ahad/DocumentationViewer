# DocumentViewer

This repository contains a .NET backend and a Next.js admin frontend in `frontend/admin`.

The backend follows the layer layout of the neighboring Tea.ExamPlatform project. Its DDD, Redis, Guard, and ResultPattern framework source projects are included under `backend/src/0.Frameworks/`. IdentityServer and its contracts are excluded. The remaining ASP.NET Core Identity types in the DDD toolkit are separate from IdentityServer.

The backend currently exposes `POST /api/admin/users/login` and `POST /api/admin/users/logout`. The Admin gateway proxies these routes to the Web API. The My gateway is configured for future `/api/my/*` endpoints; there are no My API actions yet.

Build the backend with:

```powershell
dotnet build backend/DocumentViewer.slnx
```

## Run the admin API

1. Start SQL Server and Redis. Set `SqlDatabase__ConnectionString` and the `Redis` settings if the local defaults do not apply. Redis stores revoked access-token IDs until their tokens expire.
2. In the Web API terminal, set `Jwt__SigningKey` to a random secret of at least 32 UTF-8 bytes and `SuperAdmin__Password` to the initial password, meeting the configured Identity password rules. Keep both values in environment variables or configured .NET user secrets; never commit them.
3. Start the Web API and Admin gateway in separate terminals:

```powershell
dotnet run --project backend/src/5.Presentations/DocumentViewer.WebApi --launch-profile DocumentViewer.Api.Debug
dotnet run --project backend/src/5.Presentations/ApiGateways/DocumentViewer.ApiGateway.Admin --launch-profile DocumentViewer.ApiGateway.Admin.Debug
```

With the Debug launch profiles, open the Web API Swagger UI at `https://localhost:63075/swagger` to switch between Admin and My. Each gateway also has its own Swagger UI: Admin at `https://localhost:63078/swagger` and My at `https://localhost:63071/swagger`. The gateways serve their matching documents at `/swagger/v1/swagger.json` by forwarding to the Web API, so start the Web API first. The My document has no operations until a My controller is added.

In Development, the Web API applies the initial EF Core migration on startup. In every environment, startup seeds the `SuperAdmin` role and `superadmin` user if they are missing. Subsequent starts do not change the password. A configured user that already exists without the role causes startup to fail, avoiding an accidental privilege grant. For production, apply reviewed migrations during deployment and start one API instance to seed before scaling out.

Send a JSON body to `https://localhost:63078/api/admin/users/login`:

```json
{ "userName": "superadmin", "password": "<configured password>" }
```

The request middleware also accepts its usual `{ "data": { ... }, "context": "..." }` wrapper. The response middleware wraps the token response in its standard `data` envelope. Failed credentials return HTTP 401, while account lockout follows the Identity settings. The access token expires after `Jwt:AccessTokenLifetimeMinutes` (60 by default); there is no refresh token endpoint.

To sign out, send `POST https://localhost:63078/api/admin/users/logout` with `Authorization: Bearer <access token>`. A successful response has `data.signedOut: true`. The current token is then rejected by the Web API; other sessions remain signed in.

Set `Jwt__Issuer`, `Jwt__Audience`, and `Jwt__AccessTokenLifetimeMinutes` to override their defaults. The gateway's local downstream host and port are in each gateway's `ocelot.json`; update them for deployment.

## Run the admin frontend

The login page is based on `docviewer-login-editable.svg`. It uses shadcn/ui components, local form state, a typed API client, and the backend's request and response envelopes. The backend authenticates with a username, so the form uses `Username` in place of the reference image's email field.

```powershell
cd frontend/admin
Copy-Item .env.example .env.local
npm install
npm run dev
```

Open `http://localhost:3000/login`. `API_BASE_URL` in `.env.local` points to the Admin gateway's HTTP development profile (`http://localhost:63079`) by default. Change it for another environment and restart Next.js. Next.js proxies `/backend/*` to the gateway, so the browser can call the API on the same origin. The Web API and gateway must both be running for login to work.

Successful login stores the access token in local storage when **Remember me** is checked, or session storage otherwise. The dashboard displays the admin layout with a responsive sidebar and navbar; its feature cards are placeholders for future admin tools. Missing or expired sessions redirect to login, while signed-in users are redirected from login to the dashboard. Sign out calls the API to revoke the current token before clearing the browser session. The backend currently has no refresh token endpoint.

