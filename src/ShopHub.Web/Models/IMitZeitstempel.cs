namespace ShopHub.Web.Models;

/// <summary>
/// Tabellen mit <c>ErstelltAm</c>/<c>GeaendertAm</c> (UTC). <c>GeaendertAm</c> wird beim Speichern
/// im <c>AppDbContext</c> aktualisiert.
/// </summary>
public interface IMitZeitstempel
{
    DateTime ErstelltAm { get; set; }
    DateTime GeaendertAm { get; set; }
}
