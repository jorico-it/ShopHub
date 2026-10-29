namespace ShopifySQL.Web.Models;

/// <summary>
/// Verkaufbare Variante eines <see cref="Artikel"/>s (Tabelle <c>dbo.Artikelvariante</c>).
/// </summary>
public class Artikelvariante : IMitZeitstempel
{
    public int ArtikelvarianteID { get; set; }
    public int MandantNr { get; set; }
    public Mandant? Mandant { get; set; }

    public int ArtikelID { get; set; }
    public Artikel? Artikel { get; set; }

    /// <summary>Verkaufbare Artikelnummer (eindeutig je Mandant).</summary>
    public string SKU { get; set; } = "";

    /// <summary>Z. B. EAN-13; als Text wegen führender Nullen.</summary>
    public string? GTIN { get; set; }

    /// <summary>Z. B. "Blau, Größe M".</summary>
    public string? Variantenname { get; set; }

    /// <summary>JSON-Objekt, z. B. {"Farbe":"Blau","Größe":"M"}; Spaltentyp <c>json</c>.</summary>
    public string Variantenmerkmale { get; set; } = "{}";

    /// <summary>Amazon Standard Identification Number: 10 Zeichen aus Großbuchstaben und Ziffern.</summary>
    public string? AmazonAsin { get; set; }

    /// <summary>Lieferant der Variante (Freitext).</summary>
    public string? Lieferant { get; set; }

    /// <summary>Artikelnummer des Lieferanten.</summary>
    public string? SkuLieferant { get; set; }

    /// <summary>Zolltarifnummer (HS-Code), nur Ziffern.</summary>
    public string? HsCode { get; set; }

    /// <summary>Ursprungsland als ISO-Code mit zwei Buchstaben, z. B. "CN".</summary>
    public string? Ursprungsland { get; set; }

    /// <summary>Höhe des Produkts; die Quelle nennt keine Einheit (vermutlich cm).</summary>
    public int? ProduktHoehe { get; set; }

    /// <summary>Länge des Produkts; die Quelle nennt keine Einheit (vermutlich cm).</summary>
    public int? ProduktLaenge { get; set; }

    /// <summary>
    /// Bestandteile eines Sets als JSON-Array, z. B. <c>[{"SKU":"B1008","Menge":1},{"SKU":"B1019","Menge":2}]</c>;
    /// <c>null</c> bei Varianten, die kein Set sind. Spaltentyp <c>json</c>.
    /// </summary>
    public string? BundleElemente { get; set; }

    public decimal VerkaufspreisBrutto { get; set; }
    public decimal? EinkaufspreisNetto { get; set; }
    public decimal? GewichtGramm { get; set; }
    public bool Aktiv { get; set; } = true;

    /// <summary>UTC; wird von der Datenbank gesetzt (Default SYSUTCDATETIME()).</summary>
    public DateTime ErstelltAm { get; set; }

    /// <summary>UTC; wird beim Speichern im <c>AppDbContext</c> aktualisiert.</summary>
    public DateTime GeaendertAm { get; set; }
}
