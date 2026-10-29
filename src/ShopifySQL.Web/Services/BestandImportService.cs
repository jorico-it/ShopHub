using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using ShopifySQL.Web.Data;
using ShopifySQL.Web.Models;

namespace ShopifySQL.Web.Services;

/// <summary>Eine geprüfte Zeile der Importdatei; bei <see cref="Fehler"/> ungleich <c>null</c> wird sie nicht importiert.</summary>
public sealed record BestandImportZeile(
    int ExcelZeile, string Artikelnummer, string Lagerplatz, int? Menge,
    int? ArtikelvarianteID, int? LagerplatzID, int? AlterBestand, string? Fehler);

/// <summary>Die Importdatei hat nicht das erwartete Format (z. B. fehlende Spalte).</summary>
public sealed class ImportFormatException(string message) : Exception(message);

/// <summary>
/// Import und Export der Artikelbestände als Excel-Datei mit den Spalten Artikelnummer, Lagerplatz
/// (= <c>Lagerplaetze.Bezeichnung</c>) und Menge. Der Import setzt den Bestand der Variante am Lagerplatz
/// auf die Menge und überschreibt damit den bisherigen Wert; er läuft nur ganz oder gar nicht.
/// </summary>
public sealed class BestandImportService(IDbContextFactory<AppDbContext> dbFactory)
{
    public const int MaxZeilen = 5000;

    private static readonly string[] ArtikelnummerKoepfe = ["artikelnummer"];
    private static readonly string[] LagerplatzKoepfe = ["lagerplatz", "lagerplaetzebezeichnung", "lagerplatzbezeichnung"];
    private static readonly string[] MengeKoepfe = ["menge", "bestand"];

    /// <summary>Liest die Datei und prüft jede Zeile gegen die Datenbank des Mandanten; schreibt nichts.</summary>
    public async Task<List<BestandImportZeile>> PruefenAsync(Stream xlsx, int mandantNr)
    {
        var rohzeilen = Lesen(xlsx);

        await using var db = await dbFactory.CreateDbContextAsync();
        var lagerplaetze = await db.Lagerplaetze.AsNoTracking()
            .Where(l => l.MandantNr == mandantNr)
            .Select(l => new { l.LagerplatzID, l.Bezeichnung })
            .ToListAsync();

        var nummern = rohzeilen.Select(z => z.Artikelnummer).Where(n => n.Length > 0).Distinct().ToList();
        var varianten = await db.Artikelvarianten.AsNoTracking()
            .Where(v => v.MandantNr == mandantNr && nummern.Contains(v.Artikel!.Artikelnummer))
            .Select(v => new { v.ArtikelvarianteID, v.Artikel!.Artikelnummer })
            .ToListAsync();

        var bestaende = await db.ArtikelBestaende.AsNoTracking()
            .Where(b => b.MandantNr == mandantNr)
            .Select(b => new { b.ArtikelvarianteID, b.LagerplatzID, b.Bestand })
            .ToListAsync();

        var ergebnis = new List<BestandImportZeile>();
        var gesehen = new HashSet<(int, int)>();
        foreach (var roh in rohzeilen)
        {
            string? fehler = roh.MengeFehler;
            int? varianteId = null, lagerplatzId = null, alt = null;

            var treffer = varianten.Where(v => v.Artikelnummer.Equals(roh.Artikelnummer, StringComparison.OrdinalIgnoreCase)).ToList();
            if (roh.Artikelnummer.Length == 0) fehler ??= "Artikelnummer fehlt.";
            else if (treffer.Count == 0) fehler ??= $"Artikelnummer \"{roh.Artikelnummer}\" gibt es im Mandanten nicht.";
            else if (treffer.Count > 1) fehler ??= $"Der Artikel \"{roh.Artikelnummer}\" hat mehrere Varianten - der Bestand lässt sich über die Artikelnummer nicht eindeutig zuordnen.";
            else varianteId = treffer[0].ArtikelvarianteID;

            var lager = lagerplaetze.FirstOrDefault(l => l.Bezeichnung.Equals(roh.Lagerplatz, StringComparison.OrdinalIgnoreCase));
            if (roh.Lagerplatz.Length == 0) fehler ??= "Lagerplatz fehlt.";
            else if (lager is null) fehler ??= $"Lagerplatz \"{roh.Lagerplatz}\" gibt es im Mandanten nicht.";
            else lagerplatzId = lager.LagerplatzID;

            if (fehler is null && !gesehen.Add((varianteId!.Value, lagerplatzId!.Value)))
            {
                fehler = "Artikelnummer und Lagerplatz kommen in der Datei mehrfach vor.";
            }

            if (varianteId is { } v && lagerplatzId is { } l)
            {
                alt = bestaende.FirstOrDefault(b => b.ArtikelvarianteID == v && b.LagerplatzID == l)?.Bestand;
            }

            ergebnis.Add(new BestandImportZeile(roh.ExcelZeile, roh.Artikelnummer, lager?.Bezeichnung ?? roh.Lagerplatz,
                roh.Menge, varianteId, lagerplatzId, alt, fehler));
        }

        return ergebnis;
    }

    /// <summary>Schreibt die fehlerfreien Zeilen in einer Transaktion; gibt die Zahl der tatsächlich geänderten oder neuen Bestände zurück.</summary>
    public async Task<int> ImportierenAsync(IReadOnlyList<BestandImportZeile> zeilen, int mandantNr)
    {
        if (zeilen.Any(z => z.Fehler is not null))
        {
            throw new InvalidOperationException("Die Datei enthält Fehler und wird nicht importiert.");
        }

        await using var db = await dbFactory.CreateDbContextAsync();
        var vorhanden = await db.ArtikelBestaende.Where(b => b.MandantNr == mandantNr).ToListAsync();

        var geaendert = 0;
        foreach (var z in zeilen)
        {
            var bestand = vorhanden.FirstOrDefault(b => b.ArtikelvarianteID == z.ArtikelvarianteID && b.LagerplatzID == z.LagerplatzID);
            if (bestand is null)
            {
                db.ArtikelBestaende.Add(new ArtikelBestand
                {
                    MandantNr = mandantNr,
                    ArtikelvarianteID = z.ArtikelvarianteID!.Value,
                    LagerplatzID = z.LagerplatzID!.Value,
                    Bestand = z.Menge!.Value,
                });
                geaendert++;
            }
            else if (bestand.Bestand != z.Menge)
            {
                bestand.Bestand = z.Menge!.Value;
                geaendert++;
            }
        }

        await db.SaveChangesAsync();
        return geaendert;
    }

    /// <summary>Alle Bestandszeilen des Mandanten im Format der Importdatei.</summary>
    public async Task<byte[]> ExportAsync(int mandantNr)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var zeilen = await db.ArtikelBestaende.AsNoTracking()
            .Where(b => b.MandantNr == mandantNr)
            .OrderBy(b => b.Lagerplatz!.Bezeichnung).ThenBy(b => b.Artikelvariante!.Artikel!.Artikelnummer).ThenBy(b => b.Artikelvariante!.SKU)
            .Select(b => new { b.Artikelvariante!.Artikel!.Artikelnummer, Lagerplatz = b.Lagerplatz!.Bezeichnung, Menge = b.Bestand })
            .ToListAsync();

        using var mappe = new XLWorkbook();
        var blatt = mappe.AddWorksheet("Artikelbestände");
        blatt.Cell(1, 1).Value = "Artikelnummer";
        blatt.Cell(1, 2).Value = "Lagerplatz";
        blatt.Cell(1, 3).Value = "Menge";
        blatt.Row(1).Style.Font.Bold = true;

        for (var i = 0; i < zeilen.Count; i++)
        {
            blatt.Cell(i + 2, 1).Value = zeilen[i].Artikelnummer;
            blatt.Cell(i + 2, 2).Value = zeilen[i].Lagerplatz;
            blatt.Cell(i + 2, 3).Value = zeilen[i].Menge;
        }

        blatt.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        mappe.SaveAs(stream);
        return stream.ToArray();
    }

    private sealed record Rohzeile(int ExcelZeile, string Artikelnummer, string Lagerplatz, int? Menge, string? MengeFehler);

    private static List<Rohzeile> Lesen(Stream xlsx)
    {
        XLWorkbook mappe;
        try
        {
            mappe = new XLWorkbook(xlsx);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            throw new ImportFormatException("Die Datei lässt sich nicht als Excel-Datei (.xlsx) lesen.");
        }

        using (mappe)
        {
            var blatt = mappe.Worksheets.First();
            var bereich = blatt.RangeUsed();
            if (bereich is null) throw new ImportFormatException("Die Datei enthält keine Daten.");

            var kopf = bereich.FirstRow();
            int Spalte(string[] namen) => kopf.Cells()
                .Where(c => namen.Contains(Normalisiere(c.GetString())))
                .Select(c => c.Address.ColumnNumber).FirstOrDefault();

            int spalteNr = Spalte(ArtikelnummerKoepfe), spalteLager = Spalte(LagerplatzKoepfe), spalteMenge = Spalte(MengeKoepfe);
            if (spalteNr == 0 || spalteLager == 0 || spalteMenge == 0)
            {
                throw new ImportFormatException("In der ersten Zeile müssen die Spaltenüberschriften \"Artikelnummer\", \"Lagerplatz\" und \"Menge\" stehen.");
            }

            var ergebnis = new List<Rohzeile>();
            foreach (var zeile in bereich.RowsUsed().Skip(1))
            {
                var nummer = zeile.Cell(spalteNr).GetString().Trim();
                var lager = zeile.Cell(spalteLager).GetString().Trim();
                var mengeZelle = zeile.Cell(spalteMenge);
                if (nummer.Length == 0 && lager.Length == 0 && mengeZelle.IsEmpty()) continue;

                if (ergebnis.Count >= MaxZeilen) throw new ImportFormatException($"Die Datei hat mehr als {MaxZeilen} Zeilen.");

                var (menge, mengeFehler) = LeseMenge(mengeZelle);
                ergebnis.Add(new Rohzeile(zeile.RowNumber(), nummer, lager, menge, mengeFehler));
            }

            if (ergebnis.Count == 0) throw new ImportFormatException("Die Datei enthält keine Datenzeilen.");
            return ergebnis;
        }
    }

    private static (int? Menge, string? Fehler) LeseMenge(IXLCell zelle)
    {
        if (zelle.IsEmpty()) return (null, "Menge fehlt.");

        double wert;
        if (zelle.DataType == XLDataType.Number)
        {
            wert = zelle.GetDouble();
        }
        else if (!double.TryParse(zelle.GetString().Trim(), System.Globalization.NumberStyles.Number,
                     System.Globalization.CultureInfo.GetCultureInfo("de-DE"), out wert))
        {
            return (null, $"Menge \"{zelle.GetString()}\" ist keine Zahl.");
        }

        if (wert != Math.Floor(wert)) return (null, "Die Menge muss eine ganze Zahl sein.");
        if (wert < 0 || wert > int.MaxValue) return (null, "Die Menge muss zwischen 0 und 2.147.483.647 liegen.");
        return ((int)wert, null);
    }

    // "Lagerplaetze.Bezeichnung", "Lagerplätze Bezeichnung" und "lagerplaetze_bezeichnung" werden gleich behandelt.
    private static string Normalisiere(string text) =>
        new(text.ToLowerInvariant().Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue").Replace("ß", "ss")
            .Where(char.IsLetter).ToArray());
}
