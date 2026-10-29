namespace ShopHub.Web.Models;

/// <summary>
/// Mandant (Tabelle <c>dbo.Mandanten</c>). Jede fachliche Tabelle trägt die <see cref="MandantNr"/>
/// und gehört damit genau einem Mandanten.
/// </summary>
public class Mandant : IMitZeitstempel
{
    /// <summary>Mandantennummer; fachliche Nummer, wird vergeben und nicht von der Datenbank gezählt.</summary>
    public int MandantNr { get; set; }

    public string Bezeichnung { get; set; } = "";
    public bool Aktiv { get; set; } = true;

    /// <summary>Name der Geschäftsführung, z. B. für Impressum und Belege.</summary>
    public string? Geschaeftsfuehrer { get; set; }

    public string? Strasse { get; set; }
    public string? Plz { get; set; }
    public string? Ort { get; set; }
    public string? Land { get; set; }

    /// <summary>Handelsregisternummer, z. B. "HRB 12345".</summary>
    public string? HrNummer { get; set; }

    /// <summary>Registergericht des Handelsregisters, z. B. "Amtsgericht Essen".</summary>
    public string? Amtsgericht { get; set; }
    public string? EoriNummer { get; set; }

    /// <summary>Registrierungsnummer im Verpackungsregister LUCID, z. B. "DE1234567890123456".</summary>
    public string? LucidId { get; set; }
    public string? Steuernummer { get; set; }

    /// <summary>Umsatzsteuer-Identifikationsnummer.</summary>
    public string? UstIdNr { get; set; }
    public string? Telefon { get; set; }
    public string? EMail { get; set; }

    public string? Bankname { get; set; }

    /// <summary>IBAN ohne Leerzeichen in Großbuchstaben.</summary>
    public string? Iban { get; set; }

    /// <summary>Logo als Bilddatei (PNG oder JPEG); wird über <c>/mandant-logo/{MandantNr}</c> ausgeliefert.</summary>
    public byte[]? Logo { get; set; }
    public string? LogoContentType { get; set; }

    /// <summary>UTC; wird von der Datenbank gesetzt (Default SYSUTCDATETIME()).</summary>
    public DateTime ErstelltAm { get; set; }

    /// <summary>UTC; wird beim Speichern im <c>AppDbContext</c> aktualisiert.</summary>
    public DateTime GeaendertAm { get; set; }
}
