# JORICO ShopHub

Web-App zur Pflege der Daten für Shopify in der SQL-Server-Datenbank `Shopify` auf
87.106.137.233 (gleicher Server und Zugang wie die Marktanalyse-App, nur anderer Datenbankname).

## Stack

- .NET 9, ASP.NET Core Blazor Web App (Interactive Server Render Mode) — ein Projekt, C# für
  UI und Backend, kein separates Frontend-Build.
- EF Core mit SQL Server. Der Connection String liegt **nicht** in `appsettings.json` (dort nur
  ein leerer Platzhalter, da die Datei eingecheckt ist), sondern lokal in den .NET User Secrets
  dieses Projekts (`dotnet user-secrets list --project src/ShopHub.Web`).
- Projektmappe: `ShopHub.sln` im Root, Code unter `src/ShopHub.Web`.
- Layout/Farbschema (orange Sidebar, Bootstrap) stammt aus der Marktanalyse-App
  (`Dropbox\AI_Projects\Marktanalyse_App`).

- Login wie in der Marktanalyse-App: ASP.NET Core Identity (Tabellen `AspNet…` in der Datenbank `Shopify`),
  keine öffentliche Registrierung, Konten legen Admins unter `/benutzerverwaltung` an. Das
  Erstkonto kommt beim Start aus den User Secrets `Admin:Email` / `Admin:InitialPassword`, falls
  die Benutzertabelle leer ist. Anonym erreichbar sind nur `/login`, `/impressum` und `/Error`.

## Struktur

```
src/ShopHub.Web/
  Components/   Razor-Komponenten (Pages, Layout)
  Data/         AppDbContext (IdentityDbContext)
  Models/       Entities (ApplicationUser)
  Services/     ApplicationUserClaimsPrincipalFactory (IsAdmin-Claim)
  Migrations/   EF-Core-Migrationen
  Program.cs    Startup/DI
```

## Befehle

```
dotnet build                                 # Projekt bauen
dotnet run --project src/ShopHub.Web      # lokal starten (http://localhost:5109)
dotnet ef migrations add <Name> --project src/ShopHub.Web
dotnet ef database update --project src/ShopHub.Web
```

Auf einer neuen Maschine zuerst den Connection String hinterlegen:
`dotnet user-secrets set "ConnectionStrings:Default" "..." --project src/ShopHub.Web`

## Fachliche Tabellen

- **Alle fachlichen Tabellen beginnen mit `jor`** (auch die EF-Migrationshistorie `jor__EFMigrationsHistory`); die Identity-Tabellen `AspNet…` behalten bewusst ihre Originalnamen. Das Präfix steht an einer Stelle: `AppDbContext.Tabellenpraefix`. Die Namen unten sind die Entity-Namen, in der Datenbank heißen sie z. B. `jorArtikel`.
- `Mandanten` (PK `MandantNr`, fachliche Nummer ohne Identity) - **jede fachliche Tabelle trägt eine
  `MandantNr`** mit Fremdschlüssel auf `Mandanten`.
  Stammdaten des Mandanten: Adresse (Straße, PLZ, Ort, Land), Telefon, E-Mail, Bankname, IBAN, HR-Nummer, Amtsgericht, Steuer-, USt-Id-, Lucid-ID und
  EORI-Nummer sowie das Logo (`Logo` als `varbinary(max)`, PNG/JPEG bis 512 KB). Das Logo kommt nie
  mit in Listenabfragen (immer auf die benötigten Spalten projizieren) und wird über den Endpoint
  `/mandant-logo/{MandantNr}` ausgeliefert (Admins jedes, andere nur das des eigenen Mandanten).
  Pflege unter `/mandanten` (Liste) und `/mandant-bearbeiten/{MandantNr}`.
- `Marken` (Referenztabelle je Mandant, `Markenname` eindeutig je Mandant; optionales Logo wie beim Mandanten, Endpoint `/marke-logo/{MarkeID}`, Pflege unter `/marken`; Prüfung der Bilddatei in `Services/LogoDatei`).
- `Artikel` (Produktstamm, `Artikelnummer` eindeutig je Mandant, `MarkeID` -> `Marken`).
- `Artikelvarianten` (Tabelle `jorArtikelvarianten`; `SKU` und `GTIN` eindeutig je Mandant, GTIN als
  gefilterter Index; `Variantenmerkmale` im nativen Spaltentyp `json`, in der Artikel-Maske als Tabelle Merkmal/Wert gepflegt mit Vorschlägen der im Mandanten schon verwendeten Merkmale). Weitere Felder: `Lieferant`,
  `AmazonAsin` (10 Zeichen, Großbuchstaben und Ziffern, eindeutig je Mandant, gefilterter Index), `SkuLieferant`, `HsCode` (nur Ziffern), `Ursprungsland` (ISO, 2 Buchstaben), `ProduktHoehe`,
  `ProduktLaenge` (Einheit laut Quelle unbekannt, vermutlich cm); das Gewicht wird in ganzen Gramm eingegeben und angezeigt und `BundleElemente` (Spaltentyp `json`,
  Array `[{"SKU":"B1008","Menge":1}]` mit den Bestandteilen eines Sets; in der Artikel-Maske wird es als Tabelle gepflegt (SKU mit Vorschlagsliste, Menge) und daraus erzeugt; die Oberfläche prüft zusätzlich,
  dass jede SKU im Mandanten existiert).
- `Lagerplaetze` (Tabelle `jorLagerplaetze`, `Bezeichnung` eindeutig je Mandant, Pflege unter `/lagerplaetze`)
  und `ArtikelBestaende` (Tabelle `jorArtikelBestaende`: Bestand je Variante und Lagerplatz, höchstens eine
  Zeile je Variante und Lagerplatz, Pflege unter `/bestaende`, Änderungen werden sofort gespeichert).
  Varianten und Lagerplätze mit Bestandszeilen lassen sich nicht löschen, nur inaktiv setzen.
- Erstbefüllung aus `Artikel.xlsx` (Einsdorf, Mandant 1) am 2026-10-29: 34 Artikel mit je einer Variante, 
  "Bornhake Logistik" mit Bestand 0 (anschließend in den Lagerplatz "Bornhake" übernommen, "Bornhake Logistik" wurde gelöscht). Unterschiede der Blätter: HS-Code und EK-Preis der Sets wurden aus dem Blatt
  "Products" übernommen (Blatt "EANs" hat abweichende HS-Codes 95030042-56 und KalkEK ohne Stifte).
- Artikeletiketten: `/artikeletiketten` druckt je gewählter Variante Markenlogo (Logo der Marke des Artikels, kein Fallback auf das Mandantenlogo), Artikelnummer, EAN-Barcode
  (aus `GTIN`, berechnet in `Services/GtinBarcode`, gezeichnet von `Components/Shared/BarcodeSvg`),
  GTIN-Ziffern und Bezeichnung. Unterstützt EAN-13, EAN-8 und UPC-A mit gültiger Prüfziffer;
  GTIN-14 nicht. Der Barcode ist immer 15 mm hoch, das Logo füllt die restliche Höhe der Kopfzeile. Striche und Lücken haben die GS1-Normbreite (Strichbreite einstellbar 0,264-0,66 mm, Standard 0,33 mm; echte Größe, nicht auf die Etikettenbreite gestreckt) und die Ruhezonen 11/7 Module (EAN-13) bzw. 7/7 (EAN-8). Die Vorschau zeigt nur die ersten 3 Etiketten (alle werden gedruckt). Layout: Logo oben links; unten links die Artikelnummer (nicht fett) über der Bezeichnung, unten rechts der Barcode mit den EAN-Ziffern gleichmäßig verteilt genau unter den Strichen (gleiche Breite, Schrift etwa 7,6 Strichbreiten hoch); die Unterkante der Ziffern (= Unterkante des Barcodes) liegt auf der Unterkante der Bezeichnung. Ist die Variante ein Bundle (`BundleElemente` gefüllt), steht "Bundle: " vor der Bezeichnung und darunter in kleinerer Schrift die Namen der Bestandteile (bei Menge über 1 mit "2 x "; von der Bezeichnung zählt nur der Teil hinter dem letzten Komma, also das Tier, und "für bemalbare Kuscheltiere" bei den Stiften entfällt); dann liegt die Unterkante der Ziffern auf der Unterkante dieser Namenszeile. Eine Etikette je Druckseite, Größe in mm auf der Seite einstellbar (Voreinstellung 152 x 103 mm; Schrift und Abstände skalieren mit der Größe).
- Fremdschlüssel auf `Marken` und `Artikel` laufen über `(MandantNr, ID)`, damit die Datenbank
  mandantenübergreifende Verweise ausschließt. Kein Cascade-Delete.

## Datenbankschema (Code-first)

Das Schema wird ausschließlich per EF-Core-Migrationen verwaltet (Entity-Klasse in `Models/`,
`DbSet` im `AppDbContext`, Feinkonfiguration in `OnModelCreating`). Keine Änderungen direkt im
SQL Server Management Studio, sonst weicht das Schema vom Code ab.

- Umbenennen: EF Core erzeugt dafür `DropColumn`/`AddColumn` und würde die Daten löschen - die
  Migration von Hand auf `RenameColumn` ändern.
- Neue Pflichtspalte auf bestehender Tabelle: Standardwert angeben oder erst nullable anlegen.
- Vor dem Anwenden das SQL prüfen (`dotnet ef migrations script --idempotent`); Live-Server
  per Skript aktualisieren, nicht vom Entwicklungsrechner aus.

## Arbeitsweise

Rolle: Senior Software Architect / Senior .NET-Entwickler / Code Reviewer - Code auf
Enterprise-Niveau, robust, wartbar, sicher.

Priorität bei Konflikten: Korrektheit > Verständlichkeit > Wartbarkeit > Sicherheit >
Performance > Erweiterbarkeit > Eleganz.

- Bestehenden Code zuerst analysieren, bevor Änderungen vorgeschlagen werden; vorhandene
  Architektur und Komponenten respektieren, keine Klassen/Services erfinden, die es nicht gibt.
- Änderungen so klein wie möglich halten; funktionierenden Code nicht ohne Grund verändern;
  nur refaktorieren bei echtem Wartbarkeits-/Lesbarkeits-/Performance-/Sicherheitsgewinn oder
  Bugfix, nie aus Geschmack.
- Bei unklaren Anforderungen nachfragen statt zu raten. Größere Architekturentscheidungen kurz
  begründen.
- Nach größeren Änderungen: kurz erklären was/warum geändert wurde, mögliche Seiteneffekte
  nennen, bei Bedarf Verbesserungen vorschlagen.

Code-Stil:
- Moderne C#-Syntax, Nullable Reference Types korrekt nutzen, wo sinnvoll asynchron.
- Dependency Injection statt statischer Abhängigkeiten, wo es zur bestehenden Architektur passt.
- Sprechende Namen, kurze Methoden/Klassen, keine Magic Numbers, keine Duplikate.
- XML-Kommentare für öffentliche Klassen/Methoden; sonst nur Kommentare, die das "Warum"
  erklären (nicht das "Was") - **auf Deutsch**, konsistent mit dem gesamten bestehenden Code.
- EF Core: keine unnötigen Includes/N+1-Abfragen, `AsNoTracking` bei reinen Lesezugriffen.

## Mandanten

Jeder Benutzer gehört fest zu einem Mandanten (`AspNetUsers.MandantNr`, als Claim `MandantNr`
im Login-Cookie). Admins dürfen im Menü den Mandanten wechseln (`Services/MandantKontext`, gilt
nur für die laufende Sitzung). Seiten mit Mandantendaten erben von `Components/Shared/MandantSeite`
und filtern immer auf `MandantNr`. Ein Konto ohne Mandant sieht keine Mandantendaten. Mandant 1:
"Einsdorf GmbH Good Products". Mandanten pflegen Admins unter `/mandanten`.

## JSON-Spalten

`Variantenmerkmale` und `BundleElemente` haben den nativen SQL-Server-2025-Typ `json` (Kompatibilitätsgrad 170);
die Datenbank lehnt ungültiges JSON selbst ab, in C# sind es `string`. Dafür braucht das Projekt
`Microsoft.Data.SqlClient` 6.1 oder neuer (EF Core 9 bringt nur 5.1 mit, das den Typ nicht kennt und beim
Speichern mit "SqlDbType 35 ungültig" scheitert) - der Paketverweis in der csproj darf nicht entfernt werden.
Eine Datenbank unter SQL Server 2022 oder älter kennt den Typ `json` nicht.

## Import und Export

Menüpunkte `/import` und `/export` (für alle Benutzer, jeweils im aktuellen Mandanten). Bisher gibt es
den Artikelbestände-Import/-Export (`Services/BestandImportService`, Excel über ClosedXML): Spalten
`Artikelnummer`, `Lagerplatz` (= `jorLagerplaetze.Bezeichnung`, auch als "Lagerplaetze.Bezeichnung" erkannt) und
`Menge`. Der Import setzt den Bestand auf die Menge (überschreibt), zeigt vorher eine Prüfung und läuft ganz oder
gar nicht; unbekannte Artikelnummern/Lagerplätze, Dubletten und Nicht-Ganzzahlen sind Fehler. Artikel mit mehreren
Varianten lassen sich über die Artikelnummer nicht zuordnen (Fehler). Der Export `/export/artikelbestaende?mandantNr=`
liefert dasselbe Format.

## Login-Protokoll und Passwortwechsel

- **Login-Protokoll** (`/login-protokoll`, nur Admins) wie in der Marktanalyse-App: eine Middleware in `Program.cs`
  schreibt jeden echten Seitenaufruf (GET ohne Dateiendung, ohne `/_blazor`, `/_framework` und die Logo-Endpoints) mit
  E-Mail, Mandant, IP und User-Agent in `jorBesucherLog`; die Seite zeigt je Nutzer und deutschem Kalendertag den ersten
  und letzten Zugriff und die Zahl der Aufrufe (letzte 180 Tage). Das ist eine Näherung - es gibt kein eigenes
  Login-Ereignis. Aufrufe, die schon die Autorisierung abweist (nicht angemeldet), erscheinen nicht.
- **Passwortwechsel beim ersten Login:** `AspNetUsers.MussPasswortAendern` wird gesetzt, wenn ein Admin ein Konto anlegt oder ein
  Passwort vergibt (und beim Erstkonto aus den User Secrets). Der Claim `MussPasswortAendern` im Cookie lässt die
  Fallback-Richtlinie und `AdminOnly` überall scheitern außer auf `/passwort-aendern` (Richtlinie `AngemeldeterBenutzer`);
  `RedirectToLogin` und das Cookie-Ereignis `OnRedirectToAccessDenied` leiten dorthin um. Die Seite ist bewusst statisches SSR,
  weil nach dem Ändern das Anmelde-Cookie neu ausgestellt wird (`RefreshSignInAsync`). Beim erzwungenen Wechsel wird das aktuelle Passwort nicht abgefragt (Reset-Token); beim freiwilligen schon. Sie steht allen auch freiwillig im Menü.
  Bereits vorhandene Konten sind nicht betroffen (Spalte ist `false`).

## Varianten in der Artikelmaske

Beim Öffnen eines vorhandenen Artikels ist seine erste Variante (kleinste `ArtikelvarianteID`) gleich zur Bearbeitung
aufgeklappt; hat der Artikel noch keine, öffnet sich das Formular für die erste (SKU mit der Artikelnummer vorbelegt, auch direkt
nach dem Anlegen eines neuen Artikels). Die erste Variante lässt sich nicht löschen (Schaltfläche gesperrt, zusätzlich
Prüfung beim Löschen gegen die Datenbank).

## Betrieb

Live seit 2026-10-29 unter https://shophub.jorico.de (IIS auf dem IONOS-Server, Site und App-Pool `ShopHub`,
Ordner `C:\inetpub\shophub`, Zertifikat über win-acme). Der Connection String liegt als Umgebungsvariable
`ConnectionStrings__Default` am App-Pool (SQL-Login `shophub_app`, nur Lesen/Schreiben auf `Shopify`), nie in Dateien.
Veröffentlicht wird wie bei der Marktanalyse per Web Deploy aus Visual Studio (Profil `Server`, Konto `deploy-shophub`;
das Profil liegt wegen `.gitignore` nur lokal), ersatzweise per `dotnet publish` und Kopieren per Remotedesktop. Erstinstallation,
Schritte und Fehlersuche in `Veroeffentlichung.md`.
Schemaänderungen (Migrationen) laufen weiter vom Entwicklungsrechner, da `shophub_app` kein DDL darf. Das Absichern der
Serverzugänge (Ports 1433/3389, Deploy-Konten) steht noch aus.

## Git

Repository: `https://github.com/jorico-it/ShopHub` (privat), Branch `main`. Nach jeder abgeschlossenen Aufgabe wird
committet und - bei fehlerfreiem Build - sofort gepusht (Commit-Nachrichten auf Deutsch, eine Migration gehört in denselben
Commit wie der Code, der sie braucht). Nicht pushen bei rotem Build, halbfertiger Arbeit oder auf Wunsch ("noch nicht
pushen"); nie `bin`, `obj`, `.vs` oder Zugangsdaten einchecken.

## Namen

Visual-Studio-Projekt `ShopHub.Web` (Ordner `src/ShopHub.Web`), Solution `ShopHub.sln`, Namespace `ShopHub.Web` - bis 2026-10-29
hieß es `ShopifySQL.Web`. Der Projektordner heißt seit 2026-10-29 ebenfalls `Dropbox\AI_Projects\ShopHub`. Die User-Secrets-ID
(`7d2f4c1e-...`) ist gleich geblieben, die Secrets gelten also weiter.
