# DocumentViewer

This repository contains a .NET backend skeleton and a reserved `frontend/` folder for a future React application.

The backend follows the layer layout of the neighboring Tea.ExamPlatform project. Its DDD, Redis, Guard, and ResultPattern framework source projects are included under `backend/src/0.Frameworks/`. IdentityServer and its contracts are excluded. The remaining ASP.NET Core Identity types in the DDD toolkit are separate from IdentityServer.

The DocumentViewer layer projects and the Admin and My API gateways currently contain only project references and minimal host entry points. No domain features, data connections, routes, or frontend application have been configured.

Build the backend with:

```powershell
dotnet build backend/DocumentViewer.slnx
```

