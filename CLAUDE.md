# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Backend
```bash
# Ejecutar en desarrollo
dotnet run --project Backend/src/BDAplication.API

# Compilar
dotnet build Backend/src/BDAplication.API

# Tests
dotnet test Backend/tests/BDAplication.Tests
dotnet test Backend/tests/BDAplication.Tests --filter "FullyQualifiedName~NombreDelTest"

# Migraciones EF Core
dotnet ef migrations add <Nombre> --project Backend/src/BDAplication.Persistence --startup-project Backend/src/BDAplication.API
dotnet ef database update            --project Backend/src/BDAplication.Persistence --startup-project Backend/src/BDAplication.API
```

### Frontend
```bash
# Ejecutar en desarrollo
dotnet run --project Frontend/src/BDAplication.Web

# Compilar (0 errores, 0 warnings esperado)
dotnet build Frontend/src/BDAplication.Web --configuration Release

# Publicar para despliegue
dotnet publish Frontend/src/BDAplication.Web --configuration Release --output publish/frontend
```

### Despliegue a Azure (patrón establecido)
```bash
# 1. Habilitar SCM Basic Auth temporalmente
az resource update --resource-group planner --name scm --namespace Microsoft.Web \
  --resource-type basicPublishingCredentialsPolicies --parent sites/<AppName> \
  --set properties.allow=true

# 2. Desplegar ZIP
az webapp deploy --resource-group planner --name <AppName> \
  --src-path publish/<app>.zip --type zip

# 3. Deshabilitar SCM Basic Auth
az resource update ... --set properties.allow=false
```
- Backend App Service: `TaskPlannerApi`
- Frontend App Service: `TaskPlanner`
- Resource group: `planner` (Canada Central)

---

## Arquitectura

### Backend — Clean Architecture (6 capas)

```
Domain → Application → Infrastructure
                    → Persistence
                    → Reports / OpenXml
                    → API
```

| Proyecto | Responsabilidad |
|---|---|
| `BDAplication.Domain` | Entidades, Interfaces de repositorio, Enums. Sin dependencias externas. |
| `BDAplication.Application` | DTOs, interfaces de servicio, implementaciones de servicio, validadores FluentValidation. |
| `BDAplication.Infrastructure` | JWT (`JwtService`), BCrypt (`PasswordHasher`), `DateTimeService`, `BlobStorageService`. |
| `BDAplication.Persistence` | EF Core + SQL Server, `ApplicationDbContext`, repositorios, migraciones, `DbSeeder`. |
| `BDAplication.Reports` | Generación de reportes Excel/PDF. |
| `BDAplication.API` | Controllers ASP.NET Core, middleware de errores, `Program.cs`. |

**Respuesta estándar de API:** `ApiResponse<T>` con campos `Success`, `Message`, `Data`.

**Startup:** `DbSeeder.SeedAsync` se ejecuta en cada arranque — aplica migraciones pendientes y crea roles/usuario admin si la BD está vacía. Credenciales iniciales: `admin` / `Admin123!`.

**Swagger:** Habilitado sin guard `IsDevelopment` — accesible en producción en `/swagger`.

### Frontend — Blazor Server + MudBlazor 8.6.0

Render mode `InteractiveServer` global (definido en `App.razor`).

| Carpeta | Contenido |
|---|---|
| `Components/Layout/` | `MainLayout` (sidebar responsive), `LoginLayout` (sin sidebar) |
| `Components/Pages/` | Todas las páginas con `@page` |
| `Components/TaskBoard/` | `TaskCard`, `TaskColumn` — Kanban con drag & drop HTML5 |
| `Components/Shared/Attachments/` | `AttachmentPanel` (reutilizable), diálogos de adjuntos |
| `Components/Finance/` | Diálogos de cuentas, movimientos y transferencias |
| `Services/` | Clientes HTTP hacia la API (`ApiService` base + servicios específicos) |
| `Authentication/` | `JwtAuthStateProvider` con `ProtectedSessionStorage` |

**Diálogos en dos modalidades:**
- Inline con `@bind-Visible` (Users, Roles, TaskPlanner, AttachmentPanel)
- Standalone con `IDialogService.ShowAsync<T>()` e `IMudDialogInstance` (TaskBoard, Finance)

**Drag & Drop:**
- `TaskBoard`: HTML5 nativo (`dragstart`/`drop`). No funciona en táctil — hay un `MudMenu` de fallback visible en dispositivos táctiles (`@media (hover: none)` y `max-width: 960px`).
- `TaskPlanner`: Implementado en JavaScript (`wwwroot/taskplanner.js`) con callbacks `[JSInvokable]` a Blazor.

**Responsive sidebar:** `IBrowserViewportService` detecta breakpoints; en xs/sm el drawer es `Temporary` (overlay), en md+ es `Persistent`.

---

## Configuración y secretos

`appsettings.json` contiene únicamente placeholders — los valores reales van como **Application Settings del App Service** usando `__` para claves anidadas:

| App Setting (Azure) | JSON equivalente |
|---|---|
| `ConnectionStrings__DefaultConnection` | `ConnectionStrings:DefaultConnection` |
| `Jwt__Key` | `Jwt:Key` |
| `BlobStorage__Conn` | `BlobStorage:Conn` |
| `BlobStorage__Container` | `BlobStorage:Container` |

> ⚠️ Azure Policy bloquea nombres de App Setting que contengan `AzureBlobStorage`. Usar siempre el prefijo `BlobStorage__`.

CORS: los orígenes permitidos se leen de `AllowedOrigins` en appsettings — agregar nuevas URLs de frontend ahí.

---

## Fechas y zona horaria

- **Almacenamiento:** siempre UTC en base de datos y API.
- **Backend:** `IDateTimeService.UtcNow` / `.ToLimaTime()` — `DateTimeService` usa `TimeZoneInfo`.
- **Frontend:** `IUserTimeZoneService.ToUserTime(utcDateTime)` — hardcoded a Lima/Perú (`SA Pacific Standard Time`, UTC-5, sin horario de verano). Inyectado como Scoped en todos los componentes con `@inject IUserTimeZoneService _tz`.

---

## Azure Blob Storage

`BlobStorageService` registrado como **Singleton** en `InfrastructureExtensions`. Lee `BlobStorage:Conn` y `BlobStorage:Container` de `IConfiguration`. Acceso privado al contenedor; URLs de descarga se generan con SAS tokens temporales (`GetSasUrlAsync`).

Storage account: `taskplannerstorage` — contenedor: `attachments`.
