using System.Globalization;

namespace ShopifySQL.Web.Services;

/// <summary>Anzeige von UTC-Zeitstempeln der Datenbank in deutscher Ortszeit.</summary>
public static class GermanTime
{
    private static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");

    public static string Format(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Zone)
            .ToString("dd.MM.yyyy HH:mm", German);
}
