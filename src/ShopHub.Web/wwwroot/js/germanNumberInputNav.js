// Pfeiltasten hoch/runter springen in den Annahmen-Eingabefeldern (GermanNumberInput) zum
// vorherigen/nächsten Feld, statt den Cursor im Text zu bewegen - fühlt sich wie in Excel an.
// Ein einziger dokumentweiter Listener (statt einem pro Feld) reicht aus und bleibt auch nach
// Neu-Rendern der Blazor-Komponenten gültig, solange die Elemente das Marker-Attribut tragen.
const MARKER = "data-german-number-input";

// Nur Ziffern, deutsches Dezimalkomma/-punkt sowie +/- (für Prozent-Abweichungen wie in
// ScenarioFactorSetEditor.razor) als Zeichen-Tasten zulassen - Buchstaben/Symbole werden direkt
// beim Tippen abgefangen, statt erst beim Verlassen des Felds durch einen fehlschlagenden
// decimal.TryParse in GermanNumberInput.razor.HandleChange stillschweigend verworfen zu werden.
const ALLOWED_CHAR = /^[0-9,.\-+]$/;
const ALLOWED_CONTROL_KEYS = new Set([
    "Backspace", "Delete", "Tab", "Enter", "Escape",
    "ArrowLeft", "ArrowRight", "ArrowUp", "ArrowDown", "Home", "End",
]);

export function init() {
    if (window._germanNumberInputNavInitialized) return;
    window._germanNumberInputNavInitialized = true;

    document.addEventListener("keydown", (e) => {
        const target = e.target;
        if (!(target instanceof HTMLElement) || !target.hasAttribute(MARKER)) return;

        if (e.key === "ArrowUp" || e.key === "ArrowDown") {
            const fields = Array.from(document.querySelectorAll(`[${MARKER}]`));
            const currentIndex = fields.indexOf(target);
            if (currentIndex === -1) return;

            const nextIndex = e.key === "ArrowUp" ? currentIndex - 1 : currentIndex + 1;
            const nextField = fields[nextIndex];
            if (!nextField) return;

            e.preventDefault();
            nextField.focus();
            nextField.select();
            return;
        }

        // Tastenkombinationen (Kopieren/Einfügen/Ausschneiden/Alles markieren) und Steuertasten
        // nicht blockieren - nur einzelne Zeichen-Tasten auf Ziffern/Trennzeichen/Vorzeichen
        // einschränken.
        if (e.ctrlKey || e.metaKey || e.altKey || ALLOWED_CONTROL_KEYS.has(e.key)) return;
        if (e.key.length === 1 && !ALLOWED_CHAR.test(e.key)) {
            e.preventDefault();
        }
    });

    // Zahlenfelder (type="number") mit max-Attribut kappen den Wert schon beim Tippen. Ohne das bliebe ein zu großer
    // Wert sichtbar stehen, wenn der Server ihn auf denselben Wert wie zuvor kappt: Blazor sieht dann keine Änderung
    // und frischt das Feld nicht auf, die Anzeige weicht vom gespeicherten Wert ab.
    document.addEventListener("input", (e) => {
        const target = e.target;
        if (!(target instanceof HTMLInputElement) || !target.hasAttribute(MARKER) || target.type !== "number") return;

        const max = Number(target.max);
        if (target.max !== "" && Number.isFinite(max) && target.value !== "" && Number(target.value) > max) {
            target.value = String(max);
        }
    });

    // Eingefügten Text ebenso bereinigen, damit z. B. aus Excel kopierte Werte mit Währungs-
    // symbol/Leerzeichen ("123,45 €") nicht unbemerkt Buchstaben/Symbole ins Feld bringen.
    document.addEventListener("paste", (e) => {
        const target = e.target;
        if (!(target instanceof HTMLInputElement) || !target.hasAttribute(MARKER)) return;

        const text = (e.clipboardData ?? window.clipboardData)?.getData("text") ?? "";
        const sanitized = text.replace(/[^0-9,.\-+]/g, "");
        if (sanitized === text) return;

        e.preventDefault();
        const start = target.selectionStart ?? target.value.length;
        const end = target.selectionEnd ?? target.value.length;
        target.value = target.value.slice(0, start) + sanitized + target.value.slice(end);
        const cursor = start + sanitized.length;
        target.setSelectionRange(cursor, cursor);
    });
}
