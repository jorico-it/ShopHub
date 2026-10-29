using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopHub.Web.Components;
using ShopHub.Web.Data;
using ShopHub.Web.Models;
using ShopHub.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// User Secrets werden nur in der Umgebung "Development" geladen. Ohne Connection String sonst
// erst später die kryptische Meldung "The ConnectionString property has not been initialized".
var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    var secretsId = System.Reflection.Assembly.GetEntryAssembly()
        ?.GetCustomAttribute<Microsoft.Extensions.Configuration.UserSecrets.UserSecretsIdAttribute>()?.UserSecretsId;
    var secretsDatei = secretsId is null ? null : Path.Combine(
        Environment.GetEnvironmentVariable("APPDATA") ?? "(APPDATA nicht gesetzt)", "Microsoft", "UserSecrets", secretsId, "secrets.json");
    throw new InvalidOperationException(
        $"ConnectionStrings:Default fehlt (Umgebung: {builder.Environment.EnvironmentName}). " +
        $"Erwartete Secrets-Datei: {secretsDatei ?? "keine UserSecretsId in der Assembly"} " +
        $"(vorhanden: {(secretsDatei is not null && File.Exists(secretsDatei))}). " +
        "Auf dem Server die Umgebungsvariable ConnectionStrings__Default setzen.");
}

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Factory zusätzlich zum Scoped-Kontext: Komponenten im selben Circuit (z. B. NavMenu und Seite)
// dürfen sich keinen DbContext teilen, da er keine parallelen Zugriffe verträgt.
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sql => sql.MigrationsHistoryTable(AppDbContext.Tabellenpraefix + "__EFMigrationsHistory")),
    ServiceLifetime.Scoped);

// Login/Benutzerverwaltung: Konten werden ausschließlich von Admins angelegt (keine
// öffentliche Registrierung), daher RequireConfirmedAccount = false - es gibt keinen
// Bestätigungs-E-Mail-Versand.
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

// Ohne diese Konfiguration verwendet das Cookie-Schema den Identity-Standardpfad
// "/Account/Login", den es in dieser App nicht gibt - das führt bei jedem
// Auth-Challenge (z. B. der /_blazor-SignalR-Negotiation) zu einer Endlos-
// Redirect-Schleife statt zur echten Login.razor-Seite unter "/login".
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/login";

    // Wer sein Passwort noch ändern muss, wird von der Autorisierung überall abgewiesen außer auf der
    // Passwortseite - statt zur Login-Seite geht es dann dorthin.
    options.Events.OnRedirectToAccessDenied = context =>
    {
        var target = context.HttpContext.User.HasClaim(ApplicationUserClaimsPrincipalFactory.MussPasswortAendernClaimType, "true")
            ? "/passwort-aendern"
            : "/login";
        context.Response.Redirect(target);
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorizationCore(options =>
{
    // Alles außer der Passwortseite verlangt ein Konto, das sein Passwort nicht mehr ändern muss.
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireAssertion(ctx => !ctx.User.HasClaim(ApplicationUserClaimsPrincipalFactory.MussPasswortAendernClaimType, "true"))
        .Build();
    options.AddPolicy("AdminOnly", policy => policy
        .RequireClaim(ApplicationUserClaimsPrincipalFactory.IsAdminClaimType, "true")
        .RequireAssertion(ctx => !ctx.User.HasClaim(ApplicationUserClaimsPrincipalFactory.MussPasswortAendernClaimType, "true")));
    // Für die Passwortseite: angemeldet genügt, auch wenn das Passwort geändert werden muss.
    options.AddPolicy("AngemeldeterBenutzer", policy => policy.RequireAuthenticatedUser());
});

// Aktueller Mandant je Circuit (Konto-Mandant bzw. die Auswahl eines Admins).
builder.Services.AddScoped<MandantKontext>();
builder.Services.AddScoped<BestandImportService>();

var app = builder.Build();

// Erstkonto anlegen, falls die Benutzertabelle noch leer ist - es gibt keine
// öffentliche Registrierung, ohne diesen Bootstrap käme niemand ins System.
// Zugangsdaten kommen aus den User Secrets (Admin:Email / Admin:InitialPassword).
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    if (!userManager.Users.Any())
    {
        var adminEmail = app.Configuration["Admin:Email"];
        var adminPassword = app.Configuration["Admin:InitialPassword"];
        if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
        {
            var admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, IsAdmin = true, MussPasswortAendern = true };
            var result = await userManager.CreateAsync(admin, adminPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                app.Logger.LogError("Anlegen des Admin-Erstkontos fehlgeschlagen: {Errors}", errors);
            }
        }
        else
        {
            app.Logger.LogWarning(
                "Keine Benutzer vorhanden und Admin:Email/Admin:InitialPassword nicht in den User Secrets " +
                "gesetzt - es kann sich niemand anmelden. Siehe dotnet user-secrets set \"Admin:Email\" ... " +
                "--project src/ShopHub.Web");
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// MapStaticAssets() bedient nur das zur Buildzeit erstellte Manifest; UseStaticFiles()
// deckt zusätzlich zur Laufzeit abgelegte Dateien in wwwroot ab (z. B. nach dem IIS-Deploy).
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// Seitenaufrufe protokollieren (Grundlage des Login-Protokolls): nur "echte" Seitenaufrufe (GET ohne
// Dateiendung), keine statischen Assets, keine SignalR-/Framework-Requests und keine Bild-Endpoints für
// Logos, die auf jeder Etikettenseite mehrfach abgerufen werden. Ein Fehler beim Protokollieren darf die
// Seite nicht verhindern.
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (context.Request.Method == HttpMethods.Get
        && path is not null
        && !path.StartsWith("/_blazor", StringComparison.OrdinalIgnoreCase)
        && !path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase)
        && !path.StartsWith("/mandant-logo", StringComparison.OrdinalIgnoreCase)
        && !path.StartsWith("/marke-logo", StringComparison.OrdinalIgnoreCase)
        && !Path.HasExtension(path))
    {
        try
        {
            var dbFactory = context.RequestServices.GetRequiredService<IDbContextFactory<AppDbContext>>();
            await using var db = await dbFactory.CreateDbContextAsync();
            var angemeldet = context.User.Identity?.IsAuthenticated == true;
            var userAgent = context.Request.Headers.UserAgent.ToString();
            db.BesucherLogs.Add(new BesucherLog
            {
                Pfad = path.Length > 500 ? path[..500] : path,
                IpAdresse = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = userAgent.Length > 500 ? userAgent[..500] : userAgent,
                Email = angemeldet ? context.User.Identity!.Name : null,
                MandantNr = int.TryParse(context.User.FindFirst(ApplicationUserClaimsPrincipalFactory.MandantNrClaimType)?.Value, out var mandantNr) ? mandantNr : null,
            });
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "Seitenaufruf konnte nicht protokolliert werden.");
        }
    }

    await next();
});

app.UseAntiforgery();

// Logout wird aus NavMenu (das innerhalb eines interaktiven Circuits läuft) per normalem
// <form method="post"> aufgerufen - Cookie-Auth-Änderungen brauchen eine echte HTTP-Response,
// das geht aus einem SignalR-Circuit heraus nicht direkt.
app.MapPost("/Account/Logout", async (SignInManager<ApplicationUser> signInManager, HttpContext http) =>
{
    await signInManager.SignOutAsync();
    var returnUrl = http.Request.Form["ReturnUrl"].ToString();
    return Results.LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "/login" : returnUrl);
});

// Mandantenlogo aus der Datenbank (für Etiketten und die Mandantenpflege). Normale Benutzer
// dürfen nur das Logo ihres eigenen Mandanten abrufen, Admins jedes.
app.MapGet("/mandant-logo/{mandantNr:int}", async (int mandantNr, System.Security.Claims.ClaimsPrincipal user,
    IDbContextFactory<AppDbContext> dbFactory, HttpContext http) =>
{
    var istAdmin = user.HasClaim(ApplicationUserClaimsPrincipalFactory.IsAdminClaimType, "true");
    var eigeneNr = user.FindFirst(ApplicationUserClaimsPrincipalFactory.MandantNrClaimType)?.Value;
    if (!istAdmin && eigeneNr != mandantNr.ToString())
    {
        return Results.NotFound();
    }

    await using var db = await dbFactory.CreateDbContextAsync();
    var logo = await db.Mandanten.AsNoTracking()
        .Where(m => m.MandantNr == mandantNr && m.Logo != null)
        .Select(m => new { m.Logo, m.LogoContentType })
        .FirstOrDefaultAsync();
    if (logo is null)
    {
        return Results.NotFound();
    }

    http.Response.Headers.XContentTypeOptions = "nosniff";
    http.Response.Headers.CacheControl = "private, max-age=3600";
    return Results.File(logo.Logo!, logo.LogoContentType ?? "application/octet-stream");
});

// Excel-Export der Artikelbestände eines Mandanten (Format wie beim Import); wie bei den Logos darf ein
// normaler Benutzer nur den eigenen Mandanten abrufen.
app.MapGet("/export/artikelbestaende", async (int mandantNr, System.Security.Claims.ClaimsPrincipal user,
    BestandImportService service) =>
{
    var istAdmin = user.HasClaim(ApplicationUserClaimsPrincipalFactory.IsAdminClaimType, "true");
    var eigeneNr = user.FindFirst(ApplicationUserClaimsPrincipalFactory.MandantNrClaimType)?.Value;
    if (!istAdmin && eigeneNr != mandantNr.ToString())
    {
        return Results.NotFound();
    }

    var bytes = await service.ExportAsync(mandantNr);
    return Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        $"Artikelbestaende_Mandant{mandantNr}_{DateTime.Now:yyyyMMdd}.xlsx");
});

// Markenlogo aus der Datenbank; wie beim Mandantenlogo darf ein normaler Benutzer nur Logos von
// Marken seines eigenen Mandanten abrufen.
app.MapGet("/marke-logo/{markeId:int}", async (int markeId, System.Security.Claims.ClaimsPrincipal user,
    IDbContextFactory<AppDbContext> dbFactory, HttpContext http) =>
{
    var istAdmin = user.HasClaim(ApplicationUserClaimsPrincipalFactory.IsAdminClaimType, "true");
    var eigeneNr = user.FindFirst(ApplicationUserClaimsPrincipalFactory.MandantNrClaimType)?.Value;

    await using var db = await dbFactory.CreateDbContextAsync();
    var logo = await db.Marken.AsNoTracking()
        .Where(m => m.MarkeID == markeId && m.Logo != null)
        .Select(m => new { m.MandantNr, m.Logo, m.LogoContentType })
        .FirstOrDefaultAsync();
    if (logo is null || (!istAdmin && eigeneNr != logo.MandantNr.ToString()))
    {
        return Results.NotFound();
    }

    http.Response.Headers.XContentTypeOptions = "nosniff";
    http.Response.Headers.CacheControl = "private, max-age=3600";
    return Results.File(logo.Logo!, logo.LogoContentType ?? "application/octet-stream");
});

// MapStaticAssets() registriert CSS/JS/Bilder als geroutete Endpoints - ohne
// AllowAnonymous() greift die globale RequireAuthenticatedUser()-Fallback-Policy
// auch hier, sodass z. B. das CSS auf der Login-Seite selbst nicht geladen
// werden könnte (Henne-Ei-Problem).
app.MapStaticAssets().AllowAnonymous();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
