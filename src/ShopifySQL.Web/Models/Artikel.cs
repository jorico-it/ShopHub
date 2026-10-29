namespace ShopifySQL.Web.Models;

/// <summary>
/// Produktstamm (Tabelle <c>dbo.Artikel</c>). Die verkaufbaren Ausprägungen liegen in
/// <see cref="Artikelvariante"/>.
/// </summary>
public class Artikel : IMitZeitstempel
{
    public int ArtikelID { get; set; }
    public int MandantNr { get; set; }
    public Mandant? Mandant { get; set; }

    /// <summary>Nummer des Produktstamms (eindeutig je Mandant).</summary>
    public string Artikelnummer { get; set; } = "";

    public string Bezeichnung { get; set; } = "";

    public int? MarkeID { get; set; }
    public Marke? Marke { get; set; }

    public string? Kurzbeschreibung { get; set; }
    public string? Beschreibung { get; set; }

    /// <summary>Mehrwertsteuersatz in Prozent (0-100).</summary>
    public decimal Mehrwertsteuersatz { get; set; } = StandardMehrwertsteuersatz;

    public bool Aktiv { get; set; } = true;

    /// <summary>UTC; wird von der Datenbank gesetzt (Default SYSUTCDATETIME()).</summary>
    public DateTime ErstelltAm { get; set; }

    /// <summary>UTC; wird beim Speichern im <c>AppDbContext</c> aktualisiert.</summary>
    public DateTime GeaendertAm { get; set; }

    public List<Artikelvariante> Varianten { get; set; } = [];

    public const decimal StandardMehrwertsteuersatz = 19.00m;
}
