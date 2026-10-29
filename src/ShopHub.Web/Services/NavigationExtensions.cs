using Microsoft.AspNetCore.Components;

namespace ShopHub.Web.Services;

public static class NavigationExtensions
{
    /// <summary>
    /// <c>true</c>, wenn <paramref name="url"/> (relativ oder absolut) innerhalb dieser App liegt. Verhindert, dass ein
    /// manipulierter <c>returnUrl</c> nach dem Login auf eine fremde Seite führt ("//host", "https://fremd.de").
    /// </summary>
    public static bool IstLokal(this NavigationManager navigation, string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        try
        {
            return navigation.ToAbsoluteUri(url).ToString().StartsWith(navigation.BaseUri, StringComparison.OrdinalIgnoreCase);
        }
        catch (UriFormatException)
        {
            return false;
        }
    }
}
