using System.Globalization;
using ShopHub.Web.Services;

namespace ShopHub.Web.Components.Pages;

public partial class Artikeletiketten
{
    // Schrift und Abstände wachsen mit der Etikettengröße (Grundlage 60 x 30 mm).
    private double Faktor => Math.Min(Math.Clamp(_breiteMm, MinMm, MaxMm) / 60.0, Math.Clamp(_hoeheMm, MinMm, MaxMm) / 30.0);

    // Der Barcode (mit Ruhezonen, EAN-13) ist breiter als der nutzbare Platz auf dem Etikett.
    private bool BarcodeZuBreit =>
        (double)(GtinBarcode.GesamtModule(95) * Math.Clamp(_modulbreiteMm, GtinBarcode.MinModulbreiteMm, GtinBarcode.MaxModulbreiteMm))
        > Math.Clamp(_breiteMm, MinMm, MaxMm) - 2 * 1.5 * Faktor;

    // Die Ziffern unter dem Barcode sind genau so breit wie die Striche (ohne Ruhezonen) und gleichmäßig verteilt;
    // die Schrift richtet sich nach der Strichbreite (etwa 7,6 Modulbreiten hoch, dann passt jede Ziffer in ihr Raster).
    private string GtinStil(int moduleAnzahl)
    {
        var modul = Math.Clamp(_modulbreiteMm, GtinBarcode.MinModulbreiteMm, GtinBarcode.MaxModulbreiteMm);
        // Der Barcode steht rechtsbündig samt rechter Ruhezone; die Ziffernzeile endet daher um die Ruhezone
        // (EAN-13: 7 Module) vor dem rechten Rand, genau unter dem letzten Strich.
        var (_, ruheRechts) = GtinBarcode.Ruhezonen(moduleAnzahl);
        return $"width: {(moduleAnzahl * modul).ToString("0.###", CultureInfo.InvariantCulture)}mm; "
             + $"font-size: {(modul * 7.6m).ToString("0.###", CultureInfo.InvariantCulture)}mm; "
             + $"margin: 0 {(ruheRechts * modul).ToString("0.###", CultureInfo.InvariantCulture)}mm 0 auto;";
    }

    // Liegt in einer eigenen Datei, weil Razor "@media"/"@page" im @code-Block als Direktiven liest.
    private string Css
    {
        get
        {
            var breite = Math.Clamp(_breiteMm, MinMm, MaxMm).ToString(CultureInfo.InvariantCulture);
            var hoehe = Math.Clamp(_hoeheMm, MinMm, MaxMm).ToString(CultureInfo.InvariantCulture);
            // Der Barcode ist immer 15 mm hoch; das Logo bekommt die restliche Höhe der Kopfzeile.
            var faktor = Faktor.ToString("0.##", CultureInfo.InvariantCulture);
            return $$"""
                .etiketten-druck { display: flex; flex-wrap: wrap; gap: 8px; }
                .etikett { box-sizing: border-box; width: {{breite}}mm; height: {{hoehe}}mm; padding: calc(1.5mm * {{faktor}}); display: flex;
                    flex-direction: column; overflow: hidden; background: #fff; color: #000; border: 1px dashed #999;
                    font-family: Arial, Helvetica, sans-serif; }
                .etikett-kopf { display: flex; align-items: flex-start; flex: 1; min-height: 0; }
                .etikett-logo { max-height: 100%; max-width: 65%; object-fit: contain; }
                .etikett-unten { display: flex; align-items: flex-end; justify-content: space-between; gap: calc(2mm * {{faktor}}); flex: none; margin-top: calc(1mm * {{faktor}}); }
                .etikett-text { flex: 1; min-width: 0; }
                .etikett-code { flex: none; display: flex; flex-direction: column; }
                .etikett-barcode { height: 15mm; flex: none; display: flex; justify-content: flex-end; }
                .etikett-gtin { display: flex; justify-content: space-between; line-height: 1.15; flex: none; }
                .etikett-artnr { font-size: calc(9pt * {{faktor}}); text-align: left; line-height: 1.15; white-space: nowrap; flex: none; }
                .etikett-name { font-size: calc(7.5pt * {{faktor}}); text-align: left; line-height: 1.15; max-height: 2.3em; overflow: hidden; flex: none; }
                .etikett-bundle { font-size: calc(5pt * {{faktor}}); text-align: left; line-height: 1.15; max-height: 3.5em; overflow: hidden; flex: none; }
                .etiketten-druck .etikett:nth-child(n+{{VorschauAnzahl + 1}}) { display: none; }
                @media print {
                    @page { size: {{breite}}mm {{hoehe}}mm; margin: 0; }
                    html, body { margin: 0; padding: 0; background: #fff; }
                    .sidebar, .no-print, #blazor-error-ui { display: none !important; }
                    .page { display: block !important; }
                    main, article.content { padding: 0 !important; margin: 0 !important; }
                    .etiketten-druck { display: block; }
                    .etiketten-druck .etikett:nth-child(n+{{VorschauAnzahl + 1}}) { display: flex; }
                    .etikett { border: none; break-after: page; page-break-after: always; }
                    .etikett:last-child { break-after: auto; page-break-after: auto; }
                }
                """;
        }
    }
}
