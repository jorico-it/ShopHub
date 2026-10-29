using Microsoft.AspNetCore.Identity;

namespace ShopHub.Web.Models;

public class ApplicationUser : IdentityUser
{
    public bool IsAdmin { get; set; }

    /// <summary>
    /// Das Konto muss sein Passwort ändern, bevor es die App nutzen kann: gesetzt, wenn ein Admin das
    /// Konto anlegt oder ein neues Passwort vergibt, und zurückgesetzt, sobald der Benutzer selbst ein
    /// neues Passwort gewählt hat.
    /// </summary>
    public bool MussPasswortAendern { get; set; }

    /// <summary>
    /// Mandant, zu dem das Konto gehört. Normale Benutzer arbeiten ausschließlich in diesem
    /// Mandanten; Admins dürfen den Mandanten wechseln (siehe <c>MandantKontext</c>). Ein Konto
    /// ohne Mandant sieht keine Mandantendaten.
    /// </summary>
    public int? MandantNr { get; set; }
    public Mandant? Mandant { get; set; }
}
