# ShopHub veröffentlichen (shophub.jorico.de)

Ziel: IIS-Website `ShopHub` auf dem IONOS-Server (`87.106.137.233`, `WIN-TMO0V8930E4`) neben louis-dor, Marktanalyse und CalcMatch.
Übertragung als veröffentlichten Ordner per Remotedesktop - es wird **kein** neues Web-Deploy-Konto angelegt.
Die App läuft im Modus *Production*: User Secrets gibt es dort nicht, der Connection String kommt aus einer Umgebungsvariable
am App-Pool (`ConnectionStrings__Default`). Er gehört nie in `web.config` oder ins Repository.

## Vorab (einmalig, auf dem eigenen PC)

1. Passwort des Admin-Kontos ändern: App lokal starten, mit `mgmt@jorico.de` anmelden, im Menü "Passwort ändern". Das
   bisherige Passwort steht im Klartext in den User Secrets (`Admin:InitialPassword`).
2. Der Stand muss gebaut und committet sein (`git status` sauber).

## 1. DNS

Beim Anbieter von `jorico.de` einen A-Eintrag anlegen: Name `shophub`, Wert `87.106.137.233`, TTL 300.
Prüfen: `nslookup shophub.jorico.de` liefert `87.106.137.233`.

## 2. Veröffentlichen (eigener PC, PowerShell)

```powershell
cd C:\Users\richard.colman\Dropbox\AI_Projects\ShopHub
dotnet publish src\ShopHub.Web -c Release -o C:\Temp\shophub-publish
Compress-Archive -Path C:\Temp\shophub-publish\* -DestinationPath C:\Temp\shophub.zip -Force
```

## 3. SQL-Login mit minimalen Rechten (SSMS, am Server angemeldet als Administrator)

Passwort erzeugen (nur Buchstaben und Ziffern, damit nichts mit Anführungszeichen kollidiert):

```powershell
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 28 | ForEach-Object { [char]$_ })
```

```sql
USE [master];
CREATE LOGIN [shophub_app] WITH PASSWORD = N'DAS_ERZEUGTE_PASSWORT', CHECK_POLICY = ON;
USE [Shopify];
CREATE USER [shophub_app] FOR LOGIN [shophub_app];
ALTER ROLE db_datareader ADD MEMBER [shophub_app];
ALTER ROLE db_datawriter ADD MEMBER [shophub_app];
```

Die SQL-Zeilen gehören in SSMS (neues Abfragefenster) und nicht in PowerShell. Alternativ in PowerShell mit `sqlcmd -S localhost -E -C -b -Q "..."`
(CREATE LOGIN und CREATE USER in zwei Aufrufen, der zweite mit `-d Shopify`); das Passwort bleibt dabei in einer Variable und erscheint nicht auf dem Bildschirm.
Das Passwort nie in den Chat einfügen. Ist es nicht mehr bekannt: `ALTER LOGIN [shophub_app] WITH PASSWORD = N'neu';` und Schritt 4.4 wiederholen.

Die App braucht zur Laufzeit nur Lesen und Schreiben. Änderungen am Schema (Migrationen) laufen weiter vom Entwicklungsrechner.

## 4. Website auf dem Server (Remotedesktop)

1. `C:\inetpub\shophub` anlegen, `shophub.zip` dorthin kopieren und entpacken (die Dateien direkt in den Ordner, nicht in einen Unterordner).
2. IIS-Manager > Sites > Website hinzufügen: Name `ShopHub`, Physischer Pfad `C:\inetpub\shophub`, Bindung `http`, Port 80,
   Hostname `shophub.jorico.de`. Dabei einen eigenen App-Pool `ShopHub` anlegen.
3. IIS-Manager > Anwendungspools > `ShopHub` > Grundeinstellungen: **.NET CLR-Version = Kein verwalteter Code**.
4. Connection String am App-Pool setzen (PowerShell als Administrator, Passwort aus Schritt 3 einsetzen):

```powershell
& "$env:windir\system32\inetsrv\appcmd.exe" set config -section:system.applicationHost/applicationPools "/+[name='ShopHub'].environmentVariables.[name='ConnectionStrings__Default',value='Server=localhost;Database=Shopify;User Id=shophub_app;Password=DAS_ERZEUGTE_PASSWORT;TrustServerCertificate=True']" /commit:apphost
& "$env:windir\system32\inetsrv\appcmd.exe" recycle apppool /apppool.name:ShopHub
```

Der Wert am App-Pool überstimmt die maschinenweite Variable `ConnectionStrings__Default` (die zeigt auf die Datenbank `Jorico`).
Gibt es den Eintrag schon (Passwort ändern), ihn zuerst entfernen, sonst meldet `appcmd` einen doppelten Eintrag:

```powershell
& "$env:windir\system32\inetsrv\appcmd.exe" set config -section:system.applicationHost/applicationPools "/-[name='ShopHub'].environmentVariables.[name='ConnectionStrings__Default']" /commit:apphost
```

5. IIS-Manager > Anwendungspools > `ShopHub` > Erweiterte Einstellungen > Prozessmodell > **Benutzerprofil laden = True**
   (englisch: Load User Profile). Sonst liegen die Schlüssel für das Anmelde-Cookie nur im Arbeitsspeicher und alle Benutzer sind nach
   jedem Neustart des Pools abgemeldet. Pool danach neu starten.

## 5. HTTPS-Zertifikat (win-acme)

Vorher `nslookup shophub.jorico.de 8.8.8.8` prüfen: nur `87.106.137.233`, **kein** AAAA-Eintrag (IONOS legt teils Standardeinträge auf
`217.160.0.34` und eine IPv6-Adresse an; beide ändern bzw. löschen).

`C:\win-acme\wacs.exe` als Administrator starten (Version 2.2.9): `N` (Create certificate, default settings) > Site-Nummer von `ShopHub`
eingeben (nicht leer lassen, sonst werden alle Sites gescannt) > Enter für die Bindung `shophub.jorico.de` > `y` > `Q`.
win-acme legt die HTTPS-Bindung an und nimmt das Zertifikat in die bestehende Erneuerungsaufgabe auf.
Die HTTP-Weiterleitung in `web.config` lässt die Prüfpfade (`/.well-known/acme-challenge/`) ausdrücklich durch.

## 6. Abnahme

- `https://shophub.jorico.de` zeigt die Anmeldeseite, `http://...` leitet auf HTTPS um.
- Anmelden, Artikel öffnen, Etiketten-Vorschau, Bestandsexport herunterladen.
- Neuen Test-Benutzer anlegen: beim ersten Login muss das Passwort neu gesetzt werden.
- Nach 5 falschen Passwörtern ist das Konto 15 Minuten gesperrt.
- Login-Protokoll (Admin) zeigt den Zugriff.

Fehlersuche: Ereignisanzeige > Windows-Protokolle > Anwendung (Quelle `IIS AspNetCore Module V2`). HTTP-Fehler 500.30 = App startet nicht
(meist Connection String oder fehlende Laufzeit); 500.19 = `web.config` unlesbar (URL-Rewrite-Modul fehlt).
WebSocket-Protokoll muss als IIS-Rolle installiert sein (Blazor Server).
`Login failed for user 'shophub_app'` (Fehler 18456) = Passwort im Connection String stimmt nicht mit dem Login überein: `ALTER LOGIN`
und Schritt 4.4 wiederholen. Zum Prüfen nicht `curl -I` (HEAD) verwenden, das liefert für `/login` eine Umleitung; stattdessen
`curl.exe -s -o NUL -w "%{http_code}`n" https://shophub.jorico.de/login` (erwartet 200) oder den Browser.

## Spätere Aktualisierungen

1. `dotnet publish` wie in Schritt 2. 2. Auf dem Server im Websiteordner eine leere Datei `app_offline.htm` anlegen (stoppt die App,
gibt die Dateien frei). 3. Neuen Inhalt entpacken (`web.config` nur überschreiben, wenn geändert). 4. `app_offline.htm` löschen.
Neue Datenbankmigrationen vorher vom Entwicklungsrechner anwenden (`dotnet ef database update --project src\ShopHub.Web`).
