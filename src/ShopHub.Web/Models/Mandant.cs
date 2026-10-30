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

    /// <summary>IBAN in Großbuchstaben, in Vierergruppen mit Leerzeichen (höchstens 34 Zeichen + 8 Leerzeichen).</summary>
    public string? Iban { get; set; }

    /// <summary>Logo als Bilddatei (PNG oder JPEG); wird über <c>/mandant-logo/{MandantNr}</c> ausgeliefert.</summary>
    public byte[]? Logo { get; set; }
    public string? LogoContentType { get; set; }

    /// <summary>Breite einer Artikeletikette in mm (Seite Artikeletiketten; wird dort beim Ändern gespeichert).</summary>
    public int EtikettBreiteMm { get; set; } = StandardEtikettBreiteMm;

    /// <summary>Höhe einer Artikeletikette in mm.</summary>
    public int EtikettHoeheMm { get; set; } = StandardEtikettHoeheMm;

    /// <summary>Wie viele Etiketten nebeneinander in einer Reihe gedruckt werden (Druckseite = Reihe).</summary>
    public int EtikettenProReihe { get; set; } = StandardEtikettenProReihe;

    /// <summary>Wie viele Reihen übereinander auf einer Druckseite stehen, also Etiketten pro Spalte.</summary>
    public int EtikettenProSpalte { get; set; } = StandardEtikettenProSpalte;

    public const int StandardEtikettenProSpalte = 1;
    public const int StandardEtikettBreiteMm = 152;
    public const int StandardEtikettHoeheMm = 103;
    public const int StandardEtikettenProReihe = 1;

    /// <summary>UTC; wird von der Datenbank gesetzt (Default SYSUTCDATETIME()).</summary>
    public DateTime ErstelltAm { get; set; }

    /// <summary>UTC; wird beim Speichern im <c>AppDbContext</c> aktualisiert.</summary>
    public DateTime GeaendertAm { get; set; }
}
