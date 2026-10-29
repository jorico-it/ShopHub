using Microsoft.AspNetCore.Components.Authorization;

namespace ShopifySQL.Web.Services;

/// <summary>
/// Mandant, in dem der angemeldete Benutzer gerade arbeitet (pro Blazor-Circuit). Normale
/// Benutzer arbeiten im Mandanten ihres Kontos; Admins dürfen mit <see cref="WaehleAsync"/>
/// wechseln. Die Auswahl gilt nur für die laufende Sitzung - nach einem Neuladen der Seite oder
/// einer neuen Anmeldung gilt wieder der Mandant des Kontos.
/// </summary>
public class MandantKontext(AuthenticationStateProvider authStateProvider)
{
    private int? _gewaehlteMandantNr;

    /// <summary>Wird ausgelöst, nachdem ein Admin den Mandanten gewechselt hat.</summary>
    public event Action? Geaendert;

    /// <summary>Aktueller Mandant oder <c>null</c>, wenn dem Konto keiner zugeordnet ist.</summary>
    public async Task<int?> GetMandantNrAsync()
    {
        if (_gewaehlteMandantNr is { } gewaehlt && await IstAdminAsync())
        {
            return gewaehlt;
        }

        return await GetKontoMandantNrAsync();
    }

    public async Task<bool> IstAdminAsync()
    {
        var user = (await authStateProvider.GetAuthenticationStateAsync()).User;
        return user.HasClaim(ApplicationUserClaimsPrincipalFactory.IsAdminClaimType, "true");
    }

    /// <summary>Wechselt den Mandanten; wirkt nur für Admins.</summary>
    public async Task WaehleAsync(int mandantNr)
    {
        if (!await IstAdminAsync()) return;

        _gewaehlteMandantNr = mandantNr;
        Geaendert?.Invoke();
    }

    private async Task<int?> GetKontoMandantNrAsync()
    {
        var user = (await authStateProvider.GetAuthenticationStateAsync()).User;
        var claim = user.FindFirst(ApplicationUserClaimsPrincipalFactory.MandantNrClaimType)?.Value;
        return int.TryParse(claim, out var mandantNr) ? mandantNr : null;
    }
}
