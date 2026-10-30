using System.Globalization;
using ShopHub.Web.Services;

namespace ShopHub.Web.Components.Pages;

public partial class Artikeletiketten
{
    // Schrift und Abstände wachsen mit der Etikettengröße (Grundlage 60 x 30 mm).
    private double Faktor => Math.Min(Math.Clamp(_breiteMm, MinMm, MaxMm) / 60.0, Math.Clamp(_hoeheMm, MinMm, MaxMm) / 30.0);

    // Breite des schmalsten Strichs und der schmalsten Lücke: fest die Norm-Breite 0,33 mm (GS1), nicht einstellbar.
    private const decimal Strichbreite = GtinBarcode.NominaleModulbreiteMm;

    // Der Barcode (mit Ruhezonen, EAN-13) ist breiter als der nutzbare Platz auf dem Etikett.
    private bool BarcodeZuBreit =>
        (double)(GtinBarcode.GesamtModule(95) * Strichbreite) > Math.Clamp(_breiteMm, MinMm, MaxMm) - 2 * 1.5 * Faktor;

    // Die Ziffern unter dem Barcode sind genau so breit wie die Striche (ohne Ruhezonen) und gleichmäßig verteilt;
    // die Schrift richtet sich nach der Strichbreite (etwa 7,6 Modulbreiten hoch, dann passt jede Ziffer in ihr Raster).
    private string GtinStil(int moduleAnzahl)
    {
        var modul = Strichbreite;
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
            // Eine Druckseite: so breit wie alle Etiketten einer Reihe nebeneinander, so hoch wie alle Reihen übereinander.
            var seitenbreite = SeitenbreiteMm.ToString(CultureInfo.InvariantCulture);
            var seitenhoehe = SeitenhoeheMm.ToString(CultureInfo.InvariantCulture);
            // Der Barcode ist immer 15 mm hoch; das Logo bekommt die restliche Höhe der Kopfzeile.
            var faktor = Faktor.ToString("0.##", CultureInfo.InvariantCulture);
            return $$"""
                .etiketten-vorschau { display: flex; flex-wrap: wrap; gap: 8px; }
                .etiketten-vorlagen, .etiketten-druck-ausgabe { display: none; }
                .etiketten-seite { display: flex; flex-direction: column; }
                .etiketten-reihe { display: flex; flex-wrap: nowrap; }
                .etikett { box-sizing: border-box; flex: none; width: {{breite}}mm; height: {{hoehe}}mm; padding: calc(1.5mm * {{faktor}}); display: flex;
                    flex-direction: column; overflow: hidden; background: #fff; color: #000; border: 1px dashed #999;
                    font-family: Arial, Helvetica, sans-serif; }
                .etikett-kopf { display: flex; align-items: flex-start; flex: 1; min-height: 0; }
                .etikett-logo { max-height: 100%; max-width: 65%; object-fit: contain; }
                .etikett-unten { display: flex; align-items: flex-end; justify-content: space-between; gap: calc(2mm * {{faktor}}); flex: none; margin-top: calc(1mm * {{faktor}}); }
                .etikett-text { flex: 1; min-width: 0; }
                .etikett-code { flex: none; display: flex; flex-direction: column; }
                .etikett-barcode { height: 15mm; flex: none; display: flex; justify-content: flex-end; }
                .etikett-gtin { display: flex; justify-content: space-between; line-height: 1.15; flex: none; }
                .etikett-artnr { font-size: calc(7pt * {{faktor}}); text-align: left; line-height: 1.15; white-space: nowrap; flex: none; }
                .etikett-name { font-size: calc(5.5pt * {{faktor}}); text-align: left; line-height: 1.15; max-height: 2.3em; overflow: hidden; flex: none; }
                .etikett-bundle { font-size: calc(5pt * {{faktor}}); text-align: left; line-height: 1.15; max-height: 3.5em; overflow: hidden; flex: none; }
                @media print {
                    @page { size: {{seitenbreite}}mm {{seitenhoehe}}mm; margin: 0; }
                    html, body { margin: 0; padding: 0; background: #fff; }
                    /* Gedruckt wird nur der von etikettendruck.js angehängte Druckbereich, die ganze App ist ausgeblendet. */
                    body > :not(.etiketten-druck-ausgabe) { display: none !important; }
                    .etiketten-druck-ausgabe { display: block; }
                    .etiketten-seite { width: {{seitenbreite}}mm; break-after: page; page-break-after: always; }
                    .etiketten-seite:last-child { break-after: auto; page-break-after: auto; }
                    .etiketten-reihe { width: {{seitenbreite}}mm; }
                    .etikett { border: none; }
                }
                """;
        }
    }
}
