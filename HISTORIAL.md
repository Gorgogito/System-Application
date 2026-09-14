# Historial de Cambios — System-Application

Este archivo es un registro cronológico de los cambios significativos realizados en el
proyecto, con el **porqué** detrás de cada decisión técnica importante. Su propósito es
que cualquier sesión futura (de Claude Code o de otro desarrollador) pueda entender el
contexto histórico sin tener que re-derivarlo del código o del `git log`.

Para arquitectura y convenciones **actuales** del proyecto, ver `CLAUDE.md` — este archivo
es complementario: documenta el *por qué* y la evolución, no el estado actual del código.

Convención: cada entrada nueva se agrega al final, con fecha, resumen y decisiones clave.
No se reescriben entradas pasadas salvo para corregir un error de hecho.

---

## 2026-07-03 — Inicialización del repositorio

Repositorio limpio, sin binarios ni secretos. Arquitectura Clean Architecture de 6 capas
(Domain → Application → Infrastructure/Persistence/Reports → API) en el backend, Blazor
Server + MudBlazor en el frontend.

---

## 2026-07-06 — Diseño responsive completo + fix adjuntos Azure

Sidebar responsive con `IBrowserViewportService` (drawer `Temporary` en xs/sm, `Persistent`
en md+). Corrección de un problema con adjuntos en el entorno de Azure (Blob Storage).

---

## 2026-07-07 — CLAUDE.md

Se documenta por primera vez la arquitectura, comandos y convenciones del proyecto en
`CLAUDE.md`, como referencia para sesiones de Claude Code.

---

## 2026-07-13 — Reproceso de Saldos + UI Modernización

**Problema:** los saldos en `Movement.Balance` y `Account.Balance` se persisten en BD y
podían desincronizarse (transferencias que recalculaban fuera de transacción, ediciones
retroactivas, código muerto histórico en `RecalculateAccountAsync`).

**Decisión:** herramienta administrativa `/finance/reprocess` (solo Admin) que detecta y
corrige inconsistencias sin intervención directa en BD, con auditoría completa vía
`ReprocessLog` (JSON de diffs por movimiento).

**Además:** se estableció el sistema de diseño (`AppTheme`, design tokens CSS) y se
rediseñó el Dashboard. Bug fix: `StatementAccount.SaldoMes` (neto mensual) reemplazado por
`SaldoCierre` (último `Balance` del mes en orden cronológico), porque el badge del grupo
mensual debía mostrar saldo acumulado real, no el neto del período.

---

## 2026-07-03 (decisión transversal) — Estrategia de fechas y zona horaria

**Decisión:** Opción 3 — almacenar siempre en UTC, convertir a hora de Lima (UTC-5, sin
horario de verano) al mostrar.

**Por qué:** el servidor de Azure corre en Canada Central; esta estrategia lo hace
independiente de la ubicación del servidor y escalable a múltiples zonas horarias sin
refactorización futura.

**Cómo se aplica:**
- Backend: `IDateTimeService` (`Infrastructure/Services/DateTimeService.cs`), Singleton.
- Frontend: `IUserTimeZoneService` (`Frontend/Services/UserTimeZoneService.cs`), Scoped,
  con `ToUserTime()`, `UserNow`, `UserToday`.
- **Regla de conversión:** fechas de auditoría (`DateCreated`, `DateModified`, etc.) se
  guardan en UTC y se muestran con `_tz.ToUserTime()`. Fechas de negocio ingresadas por el
  usuario (`Movement.Date`, `Attachment.DocumentDate`) se guardan como hora de Lima **sin**
  conversión — se muestran tal cual. Nunca usar `DateTime.Now`, `DateTime.Today` ni
  `.ToLocalTime()` directamente en código nuevo.

---

## 2026-07-20 — Documentos Seguros + Dashboard + mejoras UI

**Por qué:** permitir guardar documentos de texto enriquecido completamente cifrados en
BD, legibles solo tras re-autenticación (username + password contra BD).

**Diseño de seguridad:** AES-256-CBC (`Infrastructure/Encryption/EncryptionService.cs`),
IV aleatorio prefijado al cipher, la BD nunca contiene texto plano (solo
`Base64(IV+CipherText)`), descifrado solo en backend, clave `Encryption:Key` únicamente en
`IConfiguration` (Azure App Setting `Encryption__Key`), nunca en código fuente. Editor de
texto: `contenteditable` nativo + `execCommand`, módulo ES cargado con `import()` dinámico
(`wwwroot/js/securedoc-editor.js`), sin dependencias externas.

**Nota de seguridad detectada más tarde (2026-09-06):** `SecureDocumentService.GetAllAsync`
recibe un parámetro `requestingUser` pero **no filtra por él** — como el resto del sistema,
Documentos Seguros es un dato compartido entre todos los usuarios autenticados, no privado
por usuario. Ver la entrada de Bitácora Diaria más abajo, donde esto se identificó
explícitamente como el primer módulo que **sí** aísla datos por usuario.

**Además en este commit:** favicon `eating-fruit.ico` agregado a la pestaña del navegador.

---

## 2026-07-20 — Fix: límite de SignalR para documentos grandes

**Problema:** el límite por defecto de mensaje de SignalR (32 KB) bloqueaba el guardado de
documentos de Documentos Seguros con más de 8000 caracteres formateados.

**Fix:** `MaximumReceiveMessageSize` subido a 10 MB en `Frontend/Program.cs`
(`AddSignalR(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024)`).

**Nota importante (relevante para cualquier feature futura con archivos grandes):** este
límite de 10 MB, sumado a `Kestrel.Limits.MaxRequestBodySize = 10 MB` (`API/Program.cs`) y
al `[RequestSizeLimit(10_485_760)]` de `AttachmentController`, forma un límite de punta a
punta de 10 MB para **todo** el flujo navegador → circuito Blazor Server → API. Cualquier
adjunto mayor (ej. video) necesita evitar este pipeline — ver la solución adoptada en
Bitácora Diaria (subida directa a Blob Storage vía SAS).

---

## 2026-09-06 — Módulo Bitácora Diaria con evidencias multimedia

### Contexto y proceso

Se pidió implementar una funcionalidad de diario/bitácora personal: registrar
cronológicamente actividades de cada día (hora inicio/fin + descripción) con evidencia
fotográfica y de video. Se siguió un proceso de **análisis primero, código después**: se
auditó el sistema existente (arquitectura, convenciones, módulo de Adjuntos, seguridad,
manejo de fechas, límites de tamaño de archivo) y se presentó una propuesta completa
(funcional, UX, técnica, modelo de datos, seguridad, validaciones, endpoints, pruebas)
antes de escribir una sola línea de código. La propuesta fue aprobada sin cambios salvo
una decisión explícita del usuario (ver abajo).

### Decisión clave — Privacidad por usuario

**Problema:** ningún módulo existente del sistema (TaskBoard, Finanzas, Documentos
Seguros, Adjuntos) aísla datos por usuario — es un sistema de un solo tenant compartido
donde cualquier usuario autenticado ve todo. Documentos Seguros, pese a tener campos
`CreatedBy`, tampoco filtra por él (`SecureDocumentService.GetAllAsync` ignora el
parámetro `requestingUser`).

**Decisión (confirmada explícitamente por el usuario):** la Bitácora Diaria es **privada
por usuario** — cada usuario ve, edita y elimina únicamente su propia bitácora, sus
actividades y sus evidencias. Es la primera excepción a este patrón en el sistema.

**Cómo se aplica:** `Bitacora.UserId` (nuevo — ningún otro módulo lo tiene), y
`BitacoraService` valida en **cada** operación que `recurso.UserId == usuario autenticado`,
sin excepción de rol (ni siquiera Admin tiene bypass — a diferencia de
`ReprocesarSaldosController`, que sí es `[Authorize(Roles = "Admin")]`).

### Otras decisiones de diseño (con justificación completa en la propuesta original)

- **Tabla propia, no reutilizar `Attachment`:** la entidad genérica de adjuntos existente
  está acoplada a Finanzas (`DocumentDate`, `DocumentConceptId`, `Amount` obligatorios) y su
  componente `AttachmentPanel.razor` no admite video. Se creó un modelo propio
  (`Bitacora` → `BitacoraActividad` → `BitacoraEvidencia`) reutilizando en cambio el
  *servicio* `IBlobStorageService` y su patrón de nombrado de blobs.
- **Evidencia solo a nivel de Actividad, no de Día:** evita una FK polimórfica doble y
  coincide con el mockup original (las evidencias siempre cuelgan de una actividad
  específica).
- **Solapes de horario:** rechazados; se permite contigüidad exacta (`09:00-09:30` seguido
  de `09:30-13:00`). Justificación: una bitácora registra lo que realmente se hizo, y no se
  puede estar en dos actividades a la vez (a diferencia de un calendario de reuniones).
- **Fechas futuras y edición retroactiva:** permitidas sin restricción, coherente con
  `BoardTask.DueDate`/`Planner` que ya lo permiten en el resto del sistema.
- **Video vs. límite de 10 MB de punta a punta (ver entrada del 2026-07-20):** en vez de
  subir el límite global de Kestrel/SignalR (arriesgando memoria del App Service con
  múltiples uploads simultáneos), se implementó **subida directa a Azure Blob Storage vía
  SAS de escritura** — el navegador sube el archivo directo a Blob (bypasseando el circuito
  de Blazor Server y el body de la API), y el backend solo emite el SAS y confirma
  metadata después. Las imágenes (pequeñas) siguen el flujo multipart normal como
  Adjuntos.
- **Validación de archivos — mejora de seguridad respecto al patrón existente:** se
  detectó que `AttachmentService.UploadAsync` confía únicamente en el `ContentType`
  declarado por el navegador (ninguna validación de bytes reales). Para Bitácora se
  implementó validación de **firma binaria** (magic bytes: JPEG, PNG, GIF, WEBP, MP4/MOV
  vía caja `ftyp`, WebM vía firma EBML) — incluso para archivos subidos directo a Blob, vía
  una lectura de rango (`GetBlobHeaderAsync`, primeros 64 bytes) sin descargar el archivo
  completo al servidor.

### Modelo de datos (migración `AddBitacora`, 2026-09-06)

```
Bitacora            (Id, UserId, Fecha, Observacion, UserCreated/DateCreated, UserModified/DateModified, IsActive)
  índice único (UserId, Fecha)
BitacoraActividad    (Id, BitacoraId FK Cascade, HoraInicio/HoraFin TimeOnly, Descripcion, auditoría, IsActive)
  índice (BitacoraId, HoraInicio)
BitacoraEvidencia    (Id, BitacoraActividadId FK Cascade, NombreOriginal/Almacenado, BlobPath, Tipo enum, ContentType, Extension, TamanoBytes, UserCreated/DateCreated)
  índice (BitacoraActividadId)
```

Límites configurables en `appsettings.json` → `Bitacora:*`: `MaxImageSizeBytes` (15 MB),
`MaxVideoSizeBytes` (300 MB), `MaxEvidenciasPorActividad` (20).

### Archivos creados — Backend

- `Domain/Entities/Bitacora/{Bitacora,BitacoraActividad,BitacoraEvidencia}.cs`
- `Domain/Enums/TipoEvidencia.cs`
- `Domain/Interfaces/Bitacora/IBitacoraRepository.cs`
- `Application/DTOs/Bitacora/BitacoraDtos.cs`
- `Application/Interfaces/Bitacora/IBitacoraService.cs`
- `Application/Services/Bitacora/BitacoraService.cs` — lógica principal (propiedad,
  solapes, cuotas, orquestación de Blob)
- `Application/Services/Bitacora/EvidenciaSignatureValidator.cs` — validación de magic
  bytes
- `Application/Validators/Bitacora/BitacoraValidators.cs` — FluentValidation
- `Persistence/Repositories/Bitacora/BitacoraRepository.cs`
- `API/Controllers/Bitacora/BitacoraController.cs` — ruta `api/bitacora`
- Migración: `20260906222728_AddBitacora`
- `Backend/tests/BDAplication.Tests/Services/Bitacora/BitacoraServiceTests.cs` — 14 tests
  (xUnit/Moq/FluentAssertions): solapes, contigüidad exacta, rango horario inválido,
  propiedad del recurso, extensión no permitida, video por ruta equivocada, firma binaria
  inválida, cuota de evidencias, ruta de blob fuera de la actividad, conteo de resumen.

### Archivos modificados — Backend

- `Application/Interfaces/IBlobStorageService.cs` — agregado `GetSasUploadUrlAsync`
  (SAS de escritura) y `GetBlobHeaderAsync` (tamaño + primeros bytes, para validar firma
  sin descargar el archivo completo)
- `Infrastructure/Storage/BlobStorageService.cs` — implementación de ambos métodos
- `Application/BDAplication.Application.csproj` — se agregaron
  `Microsoft.Extensions.Configuration.Abstractions` y `.Binder` (10.0.0) porque
  `BitacoraService` lee límites configurables vía `IConfiguration.GetValue<T>` —
  primer servicio de Application en depender de configuración directamente (el resto de
  servicios no lo necesitaba; los límites de Adjuntos se leen en el Controller).
- `Persistence/Context/ApplicationDbContext.cs` — DbSets + configuración EF de las 3
  entidades nuevas
- `Persistence/Extensions/PersistenceExtensions.cs` — registro DI
- `API/appsettings.json` — sección `Bitacora`

### Archivos creados — Frontend

- `Models/BitacoraModels.cs`
- `Services/Bitacora/BitacoraApiService.cs`
- `Components/Pages/BitacoraPage.razor` — `/bitacora`, timeline (`MudTimeline`) del día,
  navegación por fecha, observación del día, alta/edición/borrado de actividades. Acepta
  `?fecha=yyyy-MM-dd` por query string (usado por la pantalla de consulta).
- `Components/Pages/BitacoraConsultaPage.razor` — `/bitacora/consulta`, resumen por rango
  de fechas con filtro de texto (actividades/imágenes/videos por día).
- `Components/Bitacora/ActividadDialog.razor` — alta/edición de actividad; en modo edición
  incluye la zona de evidencias con drag&drop, pegado desde portapapeles (Ctrl+V) y
  subida (imagen por multipart normal via `OpenReadStream`, video por flujo SAS: metadata
  vía `InputFile.OnChange` sin leer bytes en el circuito + JS `fetch PUT` directo a Blob +
  confirmación al backend).
- `Components/Bitacora/EvidenciaGaleriaDialog.razor` — visor con navegación prev/next,
  `<img>`/`<video controls>` según tipo, botón de descarga.
- `wwwroot/js/bitacora.js` — `wireDropZone` (drag&drop + paste sobre un `<input type=file>`
  subyacente vía `DataTransfer`), `uploadToBlob` (PUT directo a la SAS URL), `clickElement`,
  `openInNewTab`.

### Archivos modificados — Frontend

- `Components/App.razor` — script tag `js/bitacora.js`
- `Components/Layout/MainLayout.razor` — nav link "Bitácora Diaria"
- `Program.cs` — registro DI de `BitacoraApiService`

### Despliegue

Commit `35ffec5` (rama `main`, pusheado a GitHub). Desplegado a Azure siguiendo el patrón
documentado en `CLAUDE.md` (SCM Basic Auth on → `az webapp deploy --type zip` → SCM Basic
Auth off) para `TaskPlannerApi` y `TaskPlanner`. La migración `AddBitacora` se aplicó sola
al arrancar la API (`DbSeeder.SeedAsync` → `Database.MigrateAsync()`), confirmado porque
`/swagger/index.html` respondió 200 después del despliegue (si la migración hubiese
fallado, la API no habría arrancado).

### Fixes post-despliegue (2026-09-07)

**Bug 1 — "Failed to fetch" al subir video:** la cuenta de Blob Storage
(`taskplannerstorage`) no tenía ninguna regla CORS configurada. Como el video se sube
directo desde el navegador a Blob Storage (SAS de escritura, para evitar el límite de
10 MB del servidor — ver más arriba), el navegador bloqueaba el `PUT` entre orígenes
distintos. **Fix (infraestructura, sin cambio de código):**
`az storage cors add --account-name taskplannerstorage --services b` permitiendo
`GET/PUT/POST/OPTIONS/HEAD` desde el dominio de producción de `TaskPlanner` y los
orígenes de `localhost` de desarrollo. Sin este fix, **cualquier** subida directa a Blob
desde el navegador (no solo Bitácora) fallaría igual.

**Bug 2 — pantalla de error genérica intermitente ("An error occurred while processing
your request") al usar Bitácora:** causa raíz en `Frontend/Services/ApiService.cs` —
`GetAsync<T>` usaba `HttpClient.GetFromJsonAsync`, que internamente llama
`EnsureSuccessStatusCode()` y **lanza una excepción** ante cualquier respuesta no-2xx
(401, 404, 500, etc.), en vez de devolver el `ApiResponse<T>.Fail(...)` que el backend sí
envía en el cuerpo. Los demás métodos (`PostAsync`, `PutAsync`, `DeleteAsync`,
`PatchAsync`) ya leían el contenido manualmente sin lanzar — `GetAsync` era la excepción
al patrón. Como ninguna página nueva de Bitácora envolvía sus llamadas iniciales en
`try/catch` (siguiendo el patrón `try/finally` — sin `catch`— ya usado en el resto del
sistema, ej. `TaskBoard.razor`), cualquier fallo transitorio del backend durante un `GET`
(frío arranque de `TaskPlannerApi`, token expirado, etc.) tumbaba el circuito de Blazor
Server completo. **Este bug preexistía en toda la app** (afecta cualquier página que use
`GetAsync<T>`), pero se manifestaba más en Bitácora por ser la pantalla con más llamadas
GET por sesión (cambio de fecha, ver evidencias). **Fix:**
1. `ApiService.GetAsync<T>` corregido para leer la respuesta manualmente (igual que
   Post/Put/Delete/Patch) — beneficia a toda la app, no solo a Bitácora.
2. Se agregó `try/catch` real (no solo `try/finally`) alrededor de las llamadas a la API
   en `BitacoraPage`, `BitacoraConsultaPage`, `ActividadDialog` y
   `EvidenciaGaleriaDialog`, para que un error de conectividad real (no solo un status
   code) muestre un `Snackbar` en vez de tumbar el circuito. **Nota:** el resto de páginas
   del sistema (TaskBoard, Finance, etc.) siguen con el patrón `try/finally` sin `catch`
   — comparten la misma vulnerabilidad residual ante errores de conectividad pura (no de
   status code, ya cubierto por el fix de `ApiService`). No se tocaron porque estaba
   fuera del alcance de este reporte de bug.

### Pendiente / no implementado en esta iteración

- Exportación de la bitácora a PDF/Excel (se decidió no construirlo sin que se pida
  explícitamente; se puede añadir reutilizando `BDAplication.Reports`).
- Reordenar horarios de actividades por drag&drop en el timeline (se descartó por
  complejidad vs. beneficio; editar por formulario ya cubre el caso de uso).
- Costo/cuota real de Azure Storage para el volumen de video esperado — quedó como
  "pendiente de validación" en la propuesta original, no verificable desde el código.

---

## 2026-09-13/14 — Mejora UI/UX: tema claro/oscuro persistente + design system (Fase 1 de 12)

**Contexto:** se pidió un rediseño UI/UX integral inspirado en Vixon (ThemesBrand), sin
alterar funcionalidad. Se auditó primero (inventario de ~25 pantallas vía agente Explore)
antes de tocar código. Hallazgo clave: **ya existía** un sistema de diseño real
(`Helpers/AppTheme.cs` con paleta light/dark propia, `wwwroot/app.css` con tokens
`--ds-*`) — el problema no era la ausencia de sistema, sino su adopción inconsistente:
solo 2 de ~25 pantallas usaban `.app-page-header`, y ~40 puntos de color hardcodeado
duplicaban valores que ya existían como token, rompiendo el modo oscuro donde se tocaban.

**Decisión de persistencia de tema (confirmada por el usuario):** columna
`User.ThemePreference` (BD, fuente de verdad — persiste entre dispositivos) **+** cookie
`theme_mode` (aplica el tema sin parpadeo en la carga inicial, ya que una cookie viaja en
el request HTTP y se puede leer server-side con `IHttpContextAccessor` antes de que el
circuito de Blazor Server exista — a diferencia de `localStorage`, que solo está
disponible después de conectar el circuito). Se descartó localStorage puro por esa razón
específica de Blazor Server, no por preferencia general.

**Decisión sobre el editor de Documentos Seguros (confirmada por el usuario):** el fondo
blanco fijo "tipo MS Word" (ver entrada de 2026-07-20) se reemplazó por tokens `--ds-*`
— ahora se adapta al tema oscuro. Revierte una decisión de diseño anterior.

### Hecho en esta sesión (Fase 1, 3-5, 6, 7 y parte de 8-9 del plan de 12 fases)
- `ThemeToggle.razor` (3 estados: Claro/Oscuro/Sistema) en `MainLayout` y `LoginLayout`
  (antes el login no tenía ningún selector de tema).
- Endpoint `PUT api/master/updatetheme`; `UserDto`/`UserModel` incluyen `ThemePreference`.
- `PriorityPalette.cs` centraliza el color de `Priority`/`TaskBoardStatus` — antes
  duplicado con hex independientes en `TaskCard`, `TaskColumn`, `TaskPlanner`,
  `CreateTaskDialog`, `EditTaskDialog` (si se cambia un color de prioridad, ahora se toca
  un solo archivo).
- ~40 colores hardcodeados migrados a `var(--mud-palette-*)`/`var(--ds-*)` en:
  TaskPlanner, TaskBoard, AttachmentsPage, AttachmentPanel, StatementAccount,
  ReprocesarSaldosPage, ReconnectModal (plantilla de Blazor sin adaptar), scrollbars
  globales.
- Tokens nuevos en `app.css`: `--ds-space-*` (escala de espaciado), `--ds-shadow-*`,
  `--ds-critical`/`--ds-suspended` (para los 2 estados que no tienen slot semántico
  nativo en MudBlazor).
- Componentes nuevos `AppPageHeader.razor`/`EmptyState.razor`, adoptados en
  `Users.razor`/`Roles.razor` como referencia.
- Fix menor: doble indicador de carga simultáneo en `TypeConceptPage`.
- `MainLayout`: estilos inline repetidos del drawer (`style="color:rgba(...)"` en cada
  `MudNavLink`) reemplazados por clases CSS (`.app-nav-link`, `.app-nav-link-sub`).

### Pendiente (no implementado en esta sesión — queda para continuar el plan de 12 fases)
- Adopción de `AppPageHeader`/`EmptyState` en el resto de pantallas (~20 restantes).
- Paginación estándar en las tablas que no la tienen (todas excepto `DocumentConceptsPage`).
- Unificar símbolo de moneda (`$` en diálogos de Finance vs. `S/` en el resto).
- Revisión de accesibilidad (contraste, foco) y responsive dirigida — no se hizo pase
  dedicado, solo lo que la migración de colores pudo tocar incidentalmente.
- **Verificación visual real en navegador** — no se pudo probar (extensión de Chrome
  desconectada esta sesión); todo el trabajo se validó por compilación (0 errores) y
  razonamiento sobre las variables CSS de MudBlazor, no por inspección visual directa.
  Recomendado: abrir la app y probar los 3 modos de tema antes de dar por cerrada la fase.
