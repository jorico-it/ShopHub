using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ShopifySQL.Web.Models;

namespace ShopifySQL.Web.Services;

// IdentityUser hat weder IsAdmin noch MandantNr - die Werte werden beim Login hier als Claims in
// den Cookie geschrieben, damit NavMenu/Seiten sie ohne DB-Roundtrip prüfen können. Eine
// Änderung am Konto greift daher erst nach dem nächsten Login.
public class ApplicationUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<ApplicationUser>(userManager, optionsAccessor)
{
    public const string IsAdminClaimType = "IsAdmin";
    public const string MandantNrClaimType = "MandantNr";

    /// <summary>Nur vorhanden (Wert "true"), solange das Konto sein Passwort noch ändern muss.</summary>
    public const string MussPasswortAendernClaimType = "MussPasswortAendern";

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim(IsAdminClaimType, user.IsAdmin ? "true" : "false"));
        if (user.MussPasswortAendern)
        {
            identity.AddClaim(new Claim(MussPasswortAendernClaimType, "true"));
        }

        if (user.MandantNr is { } mandantNr)
        {
            identity.AddClaim(new Claim(MandantNrClaimType, mandantNr.ToString()));
        }

        return identity;
    }
}
