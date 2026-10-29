namespace ShopHub.Web.Models;

/// <summary>
/// Bestand einer <see cref="Artikelvariante"/> an einem <see cref="Lagerplatz"/>
/// (Tabelle <c>jorArtikelBestaende</c>); je Variante und Lagerplatz höchstens eine Zeile.
/// </summary>
public class ArtikelBestand : IMitZeitstempel
{
    public int ArtikelBestandID { get; set; }
    public int MandantNr { get; set; }
    public Mandant? Mandant { get; set; }

    public int ArtikelvarianteID { get; set; }
    public Artikelvariante? Artikelvariante { get; set; }

    public int LagerplatzID { get; set; }
    public Lagerplatz? Lagerplatz { get; set; }

    /// <summary>Verfügbare Stückzahl.</summary>
    public int Bestand { get; set; }

    /// <summary>UTC; wird von der Datenbank gesetzt (Default SYSUTCDATETIME()).</summary>
    public DateTime ErstelltAm { get; set; }

    /// <summary>UTC; wird beim Speichern im <c>AppDbContext</c> aktualisiert.</summary>
    public DateTime GeaendertAm { get; set; }
}
