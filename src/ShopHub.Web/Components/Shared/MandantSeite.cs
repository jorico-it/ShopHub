using Microsoft.AspNetCore.Components;
using ShopHub.Web.Services;

namespace ShopHub.Web.Components.Shared;

/// <summary>
/// Basisklasse für Seiten, die Daten des aktuellen Mandanten zeigen: setzt <see cref="MandantNr"/>
/// und lädt die Seite neu, wenn ein Admin den Mandanten wechselt. Seiten implementieren
/// <see cref="LadeAsync"/> statt <c>OnInitializedAsync</c>.
/// </summary>
public abstract class MandantSeite : ComponentBase, IDisposable
{
    [Inject] protected MandantKontext Kontext { get; set; } = default!;

    /// <summary>Aktueller Mandant; <c>null</c>, wenn dem Konto keiner zugeordnet ist.</summary>
    protected int? MandantNr { get; private set; }

    protected abstract Task LadeAsync();

    protected override async Task OnInitializedAsync()
    {
        MandantNr = await Kontext.GetMandantNrAsync();
        Kontext.Geaendert += OnMandantGeaendert;
        await LadeAsync();
    }

    private void OnMandantGeaendert() => _ = InvokeAsync(async () =>
    {
        MandantNr = await Kontext.GetMandantNrAsync();
        await LadeAsync();
        StateHasChanged();
    });

    public void Dispose() => Kontext.Geaendert -= OnMandantGeaendert;
}
