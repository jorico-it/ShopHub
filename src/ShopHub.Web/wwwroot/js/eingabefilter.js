// Begrenzt Eingabefelder mit dem Attribut data-eingabefilter schon beim Tippen und Einfügen:
//   "ziffern"       - nur Ziffern (PLZ, Telefon)
//   "land-ziffern"  - zwei Buchstaben als Ländercode, ab der 3. Stelle nur Ziffern (IBAN, USt-IdNr., EORI)
// Ein einziger dokumentweiter Listener reicht und bleibt auch nach dem Neu-Rendern von Blazor gültig.
// Die Filter arbeiten Zeichen für Zeichen von links, damit sich die Cursorposition aus dem gefilterten
// Text vor dem Cursor ergibt. Der Blazor-Wert wird beim Verlassen des Felds (change) übernommen.
const ATTRIBUT = "data-eingabefilter";

function filtere(art, text) {
    let ergebnis = "";
    for (const zeichen of text) {
        if (art === "ziffern") {
            if (/[0-9]/.test(zeichen)) ergebnis += zeichen;
        } else if (art === "land-ziffern") {
            if (ergebnis.length < 2) {
                if (/[A-Za-z]/.test(zeichen)) ergebnis += zeichen.toUpperCase();
            } else if (/[0-9]/.test(zeichen)) {
                ergebnis += zeichen;
            }
        }
    }
    return ergebnis;
}

export function init() {
    if (window._eingabefilterInitialized) return;
    window._eingabefilterInitialized = true;

    document.addEventListener("input", (e) => {
        const ziel = e.target;
        if (!(ziel instanceof HTMLInputElement) || !ziel.hasAttribute(ATTRIBUT)) return;

        const art = ziel.getAttribute(ATTRIBUT);
        const gefiltert = filtere(art, ziel.value);
        if (gefiltert === ziel.value) return;

        const cursor = ziel.selectionStart ?? ziel.value.length;
        const neuerCursor = filtere(art, ziel.value.slice(0, cursor)).length;
        ziel.value = gefiltert;
        ziel.setSelectionRange(neuerCursor, neuerCursor);
    });
}
