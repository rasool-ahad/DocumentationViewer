# DocumentViewer

This repository contains a .NET backend skeleton and a reserved `frontend/` folder for a future React application.

The backend follows the layer layout of the neighboring Tea.ExamPlatform project. Its DDD, Redis, Guard, and ResultPattern framework source projects are included under `backend/src/0.Frameworks/`. IdentityServer and its contracts are excluded. The remaining ASP.NET Core Identity types in the DDD toolkit are separate from IdentityServer.

The backend currently exposes one endpoint: `POST /api/admin/users/login`. The Admin gateway proxies this route to the Web API. The My gateway is configured for future `/api/my/*` endpoints; there are no My API actions yet.

Build the backend with:

```powershell
dotnet build backend/DocumentViewer.slnx
```

## Run the admin login API

1. Start SQL Server and set `SqlDatabase__ConnectionString` if the default local connection string does not apply.
2. In the Web API terminal, set `Jwt__SigningKey` to a random secret of at least 32 UTF-8 bytes and `SuperAdmin__Password` to the initial password, meeting the configured Identity password rules. Keep both values in environment variables or configured .NET user secrets; never commit them.
3. Start the Web API and Admin gateway in separate terminals:

```powershell
dotnet run --project backend/src/5.Presentations/DocumentViewer.WebApi --launch-profile DocumentViewer.WebApi
dotnet run --project backend/src/5.Presentations/ApiGateways/DocumentViewer.ApiGateway.Admin --launch-profile DocumentViewer.ApiGateway.Admin
```

In Development, the Web API applies the initial EF Core migration on startup. In every environment, startup seeds the `SuperAdmin` role and `superadmin` user if they are missing. Subsequent starts do not change the password. A configured user that already exists without the role causes startup to fail, avoiding an accidental privilege grant. For production, apply reviewed migrations during deployment and start one API instance to seed before scaling out.

Send a JSON body to `https://localhost:63078/api/admin/users/login`:

```json
{ "userName": "superadmin", "password": "<configured password>" }
```

The request middleware also accepts its usual `{ "data": { ... }, "context": "..." }` wrapper. The response middleware wraps the token response in its standard `data` envelope. Failed credentials return HTTP 401, while account lockout follows the Identity settings. The access token expires after `Jwt:AccessTokenLifetimeMinutes` (60 by default); there is no refresh token endpoint.

Set `Jwt__Issuer`, `Jwt__Audience`, and `Jwt__AccessTokenLifetimeMinutes` to override their defaults. The gateway's local downstream host and port are in each gateway's `ocelot.json`; update them for deployment.

