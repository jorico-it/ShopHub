namespace ShopHub.Web.Models;

/// <summary>
/// Referenztabelle der Marken je Mandant (Tabelle <c>dbo.Marken</c>); wird vom
/// <see cref="Artikel"/> über <c>MarkeID</c> referenziert.
/// </summary>
public class Marke : IMitZeitstempel
{
    public int MarkeID { get; set; }
    public int MandantNr { get; set; }
    public Mandant? Mandant { get; set; }

    /// <summary>Markenname, eindeutig je Mandant.</summary>
    public string Markenname { get; set; } = "";

    public bool Aktiv { get; set; } = true;

    /// <summary>Logo als Bilddatei (PNG oder JPEG); wird über <c>/marke-logo/{MarkeID}</c> ausgeliefert.</summary>
    public byte[]? Logo { get; set; }
    public string? LogoContentType { get; set; }

    /// <summary>UTC; wird von der Datenbank gesetzt (Default SYSUTCDATETIME()).</summary>
    public DateTime ErstelltAm { get; set; }

    /// <summary>UTC; wird beim Speichern im <c>AppDbContext</c> aktualisiert.</summary>
    public DateTime GeaendertAm { get; set; }
}
