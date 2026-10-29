namespace ShopHub.Web.Services;

/// <summary>
/// Berechnet das Strichmuster eines EAN-Barcodes aus einer GTIN: EAN-13, EAN-8 sowie UPC-A
/// (12 Stellen, wird wie üblich als EAN-13 mit führender 0 gedruckt). GTIN-14 wird nicht unterstützt.
/// </summary>
public static class GtinBarcode
{
    /// <summary>Nominale Modulbreite (X-Dimension) von EAN-13 nach GS1: 0,33 mm (100 %).</summary>
    public const decimal NominaleModulbreiteMm = 0.33m;

    /// <summary>Zulässige Modulbreite nach GS1: 80 % bis 200 % der nominalen.</summary>
    public const decimal MinModulbreiteMm = 0.264m;
    public const decimal MaxModulbreiteMm = 0.66m;

    private const int ModuleEan8 = 67;

    /// <summary>
    /// Ruhezone links und rechts in Modulbreiten nach GS1: EAN-13 11 und 7, EAN-8 7 und 7
    /// (bei <paramref name="moduleAnzahl"/> = Zahl der Strichmodule ohne Ruhezone).
    /// </summary>
    public static (int Links, int Rechts) Ruhezonen(int moduleAnzahl) => moduleAnzahl == ModuleEan8 ? (7, 7) : (11, 7);

    /// <summary>Breite des Symbols samt Ruhezonen in Modulen.</summary>
    public static int GesamtModule(int moduleAnzahl)
    {
        var (links, rechts) = Ruhezonen(moduleAnzahl);
        return links + moduleAnzahl + rechts;
    }

    // Zeichensatz A ("L") der Ziffern 0-9; C ("R") ist das Bitkomplement, B ("G") dessen Spiegelung.
    private static readonly string[] CodeL =
        ["0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011"];

    // Zeichensatzfolge der linken sechs Ziffern von EAN-13 nach der ersten Ziffer.
    private static readonly string[] Paritaet =
        ["LLLLLL", "LLGLGG", "LLGGLG", "LLGGGL", "LGLLGG", "LGGLLG", "LGGGLL", "LGLGLG", "LGLGGL", "LGGLGL"];

    /// <summary>
    /// Erzeugt die Module (true = Strich) ohne Ruhezone. Gibt <c>false</c> mit Fehlertext zurück,
    /// wenn die GTIN nicht als EAN druckbar ist (Länge, Zeichen oder Prüfziffer).
    /// </summary>
    public static bool TryErzeugen(string? gtin, out bool[] module, out string? fehler)
    {
        module = [];
        var ziffern = gtin?.Trim() ?? "";
        if (ziffern.Length == 0 || !ziffern.All(char.IsAsciiDigit))
        {
            fehler = "EAN besteht nicht nur aus Ziffern.";
            return false;
        }

        if (ziffern.Length == 12)
        {
            ziffern = "0" + ziffern;
        }

        if (ziffern.Length is not (8 or 13))
        {
            fehler = $"EAN mit {ziffern.Length} Stellen ist kein EAN-8/EAN-13.";
            return false;
        }

        if (!PruefzifferGueltig(ziffern))
        {
            fehler = "Die Prüfziffer der EAN stimmt nicht.";
            return false;
        }

        var bits = new System.Text.StringBuilder("101");
        if (ziffern.Length == 13)
        {
            var folge = Paritaet[ziffern[0] - '0'];
            for (var i = 0; i < 6; i++)
            {
                bits.Append(Zeichen(ziffern[1 + i], folge[i]));
            }

            bits.Append("01010");
            for (var i = 7; i < 13; i++)
            {
                bits.Append(Zeichen(ziffern[i], 'R'));
            }
        }
        else
        {
            for (var i = 0; i < 4; i++)
            {
                bits.Append(Zeichen(ziffern[i], 'L'));
            }

            bits.Append("01010");
            for (var i = 4; i < 8; i++)
            {
                bits.Append(Zeichen(ziffern[i], 'R'));
            }
        }

        bits.Append("101");
        module = bits.ToString().Select(c => c == '1').ToArray();
        fehler = null;
        return true;
    }

    private static string Zeichen(char ziffer, char satz)
    {
        var l = CodeL[ziffer - '0'];
        var r = new string(l.Select(c => c == '0' ? '1' : '0').ToArray());
        return satz switch
        {
            'L' => l,
            'R' => r,
            _ => new string(r.Reverse().ToArray()),
        };
    }

    // GS1-Prüfziffer: von rechts (ohne Prüfziffer) abwechselnd mit 3 und 1 gewichtet.
    private static bool PruefzifferGueltig(string ziffern)
    {
        var summe = 0;
        for (var i = ziffern.Length - 2; i >= 0; i--)
        {
            var gewicht = (ziffern.Length - 2 - i) % 2 == 0 ? 3 : 1;
            summe += (ziffern[i] - '0') * gewicht;
        }

        return (10 - summe % 10) % 10 == ziffern[^1] - '0';
    }
}
