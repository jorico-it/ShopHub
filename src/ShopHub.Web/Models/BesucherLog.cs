namespace ShopHub.Web.Models;

/// <summary>
/// Protokoll der Seitenaufrufe (Tabelle <c>jorBesucherLog</c>), Grundlage des Login-Protokolls: ein
/// "Login" wird als erster Seitenaufruf eines Nutzers an einem Tag genähert. Anonyme Aufrufe
/// (z. B. die Login-Seite) haben keine <see cref="Email"/>.
/// </summary>
public class BesucherLog
{
    public int BesucherLogID { get; set; }

    /// <summary>UTC; wird von der Datenbank gesetzt (Default SYSUTCDATETIME()).</summary>
    public DateTime ErstelltAm { get; set; }

    public string Pfad { get; set; } = "";
    public string? IpAdresse { get; set; }
    public string? UserAgent { get; set; }
    public string? Email { get; set; }

    /// <summary>Mandant des angemeldeten Kontos; <c>null</c> bei anonymen Aufrufen und Konten ohne Mandant.</summary>
    public int? MandantNr { get; set; }
}
