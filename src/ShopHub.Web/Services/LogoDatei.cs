using Microsoft.AspNetCore.Components.Forms;

namespace ShopHub.Web.Services;

/// <summary>
/// Liest und prüft hochgeladene Logos (Mandant, Marke): nur PNG und JPEG, begrenzte Größe. Der vom
/// Browser gemeldete Dateityp ist frei wählbar - der Typ wird am Dateikopf erkannt.
/// </summary>
public static class LogoDatei
{
    public const int MaxBytes = 512 * 1024;

    /// <summary>Liest die Datei; bei Problemen ist <c>Fehler</c> gesetzt und die übrigen Werte sind <c>null</c>.</summary>
    public static async Task<(byte[]? Daten, string? ContentType, string? Fehler)> LesenAsync(IBrowserFile datei)
    {
        byte[] daten;
        try
        {
            await using var stream = datei.OpenReadStream(MaxBytes);
            using var puffer = new MemoryStream();
            await stream.CopyToAsync(puffer);
            daten = puffer.ToArray();
        }
        catch (IOException)
        {
            return (null, null, $"Die Datei ist zu groß (maximal {MaxBytes / 1024} KB).");
        }

        var contentType = ErkenneBildtyp(daten);
        return contentType is null
            ? (null, null, "Bitte eine PNG- oder JPEG-Datei auswählen.")
            : (daten, contentType, null);
    }

    private static string? ErkenneBildtyp(byte[] daten)
    {
        ReadOnlySpan<byte> png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        ReadOnlySpan<byte> jpeg = [0xFF, 0xD8, 0xFF];
        if (daten.AsSpan().StartsWith(png)) return "image/png";
        if (daten.AsSpan().StartsWith(jpeg)) return "image/jpeg";
        return null;
    }
}
