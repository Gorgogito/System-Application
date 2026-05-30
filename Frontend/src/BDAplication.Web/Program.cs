using BDAplication.Web.Authentication;
using BDAplication.Web.Components;
using BDAplication.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MudBlazor
builder.Services.AddMudServices();

// Authentication state
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddAuthorizationCore();

// HttpClient apuntando al Backend API
// En Blazor Server las llamadas las hace el servidor — el cert dev no es de confianza por defecto
builder.Services.AddScoped(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["ApiBaseUrl"] ?? "https://localhost:7161";

    var handler = new HttpClientHandler
    {
        // Ignorar validación de certificado en desarrollo
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
});

// API Services
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<UserApiService>();
builder.Services.AddScoped<RoleApiService>();
builder.Services.AddScoped<BacklogApiService>();
builder.Services.AddScoped<PlannerApiService>();
builder.Services.AddScoped<TaskBoardApiService>();

// Finance
builder.Services.AddScoped<BDAplication.Web.Services.Finance.AccountApiService>();
builder.Services.AddScoped<BDAplication.Web.Services.Finance.TypeConceptApiService>();
builder.Services.AddScoped<BDAplication.Web.Services.Finance.MovementApiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
