namespace ShopHub.Web.Models;

/// <summary>
/// Lagerplatz eines Mandanten, z. B. ein Logistikdienstleister oder ein eigenes Lager
/// (Tabelle <c>jorLagerplaetze</c>). Der Bestand je Variante steht in <see cref="ArtikelBestand"/>.
/// </summary>
public class Lagerplatz : IMitZeitstempel
{
    public int LagerplatzID { get; set; }
    public int MandantNr { get; set; }
    public Mandant? Mandant { get; set; }

    /// <summary>Name des Lagerplatzes, eindeutig je Mandant.</summary>
    public string Bezeichnung { get; set; } = "";

    public bool Aktiv { get; set; } = true;

    /// <summary>UTC; wird von der Datenbank gesetzt (Default SYSUTCDATETIME()).</summary>
    public DateTime ErstelltAm { get; set; }

    /// <summary>UTC; wird beim Speichern im <c>AppDbContext</c> aktualisiert.</summary>
    public DateTime GeaendertAm { get; set; }
}
