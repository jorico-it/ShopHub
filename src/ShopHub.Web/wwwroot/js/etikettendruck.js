// Druck der Artikeletiketten (Seite Artikeletiketten): Die Seite enthält je gewählter Variante nur eine Vorlage
// (.etikett-vorlage mit data-anzahl). Erst hier entstehen die Kopien und werden zu Druckseiten angeordnet:
// je Seite "proSpalte" Reihen zu "proReihe" Etiketten, zeilenweise gefüllt. So rendert und überträgt Blazor
// nie jedes einzelne Etikett. Der Druckbereich ist nur beim Drucken da und wird danach wieder entfernt.
const AUSGABE = "etiketten-druck-ausgabe";

export function drucke(proReihe, proSpalte) {
    document.querySelectorAll(`.${AUSGABE}`).forEach((alt) => alt.remove());

    const ausgabe = document.createElement("div");
    ausgabe.className = AUSGABE;

    let seite = null;
    let reihe = null;
    let reihenAufSeite = 0;
    let etikettenInReihe = 0;

    for (const vorlage of document.querySelectorAll(".etikett-vorlage")) {
        const anzahl = parseInt(vorlage.dataset.anzahl, 10) || 0;
        const etikett = vorlage.firstElementChild;
        if (!etikett) continue;

        for (let i = 0; i < anzahl; i++) {
            if (reihe === null || etikettenInReihe >= proReihe) {
                if (seite === null || reihenAufSeite >= proSpalte) {
                    seite = document.createElement("div");
                    seite.className = "etiketten-seite";
                    ausgabe.append(seite);
                    reihenAufSeite = 0;
                }

                reihe = document.createElement("div");
                reihe.className = "etiketten-reihe";
                seite.append(reihe);
                reihenAufSeite++;
                etikettenInReihe = 0;
            }

            reihe.append(etikett.cloneNode(true));
            etikettenInReihe++;
        }
    }

    if (!ausgabe.hasChildNodes()) return;

    document.body.append(ausgabe);
    const aufraeumen = () => {
        ausgabe.remove();
        window.removeEventListener("afterprint", aufraeumen);
    };
    window.addEventListener("afterprint", aufraeumen);

    // Verzögert, weil window.print() blockiert, bis der Druckdialog geschlossen ist; der Blazor-Aufruf soll sofort zurückkehren.
    setTimeout(() => window.print(), 0);
}
