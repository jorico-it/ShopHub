using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopHub.Web.Models;

namespace ShopHub.Web.Data;

/// <summary>
/// EF-Core-Kontext für die SQL-Server-Datenbank, in der die Daten für Shopify gepflegt werden
/// (Identity-Tabellen für den Login plus Mandanten, Marken, Artikelstamm und Varianten).
/// Alle fachlichen Tabellen tragen eine <c>MandantNr</c>.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    /// <summary>Namenspräfix der fachlichen Tabellen und der EF-Migrationshistorie (nicht der Identity-Tabellen).</summary>
    public const string Tabellenpraefix = "jor";

    private static string Tabellenname(string name) => Tabellenpraefix + name;

    public DbSet<Mandant> Mandanten => Set<Mandant>();
    public DbSet<Marke> Marken => Set<Marke>();
    public DbSet<Artikel> Artikel => Set<Artikel>();
    public DbSet<Artikelvariante> Artikelvarianten => Set<Artikelvariante>();
    public DbSet<Lagerplatz> Lagerplaetze => Set<Lagerplatz>();
    public DbSet<ArtikelBestand> ArtikelBestaende => Set<ArtikelBestand>();
    public DbSet<BesucherLog> BesucherLogs => Set<BesucherLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Die Identity-Tabellen (AspNet...) behalten bewusst ihre Standardnamen ohne Präfix "jor".

        // Constraint-Namen wie im Schema-Entwurf vorgegeben. Eindeutigkeit als Unique Index
        // (HasIndex) statt UNIQUE-Constraint - EF Core kennt nur Indizes; funktional identisch.
        // Fremdschlüssel auf Marke und Artikel enthalten die MandantNr, damit die Datenbank
        // mandantenübergreifende Verweise ausschließt. Kein Cascade-Delete: Stammdaten mit
        // abhängigen Zeilen sollen nicht versehentlich löschbar sein.
        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Mandant).WithMany()
            .HasForeignKey(u => u.MandantNr)
            .HasConstraintName("FK_AspNetUsers_Mandanten")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Mandant>(e =>
        {
            e.ToTable(Tabellenname("Mandanten"));
            e.HasKey(m => m.MandantNr).HasName("PK_Mandanten");
            e.Property(m => m.MandantNr).ValueGeneratedNever();
            e.Property(m => m.Bezeichnung).HasMaxLength(200);
            e.Property(m => m.Geschaeftsfuehrer).HasMaxLength(200);
            e.Property(m => m.Strasse).HasMaxLength(200);
            e.Property(m => m.Plz).HasMaxLength(20);
            e.Property(m => m.Ort).HasMaxLength(100);
            e.Property(m => m.Land).HasMaxLength(100);
            e.Property(m => m.HrNummer).HasMaxLength(50);
            e.Property(m => m.Amtsgericht).HasMaxLength(100);
            e.Property(m => m.EoriNummer).HasMaxLength(20);
            e.Property(m => m.LucidId).HasMaxLength(20);
            e.Property(m => m.Steuernummer).HasMaxLength(30);
            e.Property(m => m.UstIdNr).HasMaxLength(20);
            e.Property(m => m.Telefon).HasMaxLength(50);
            e.Property(m => m.EMail).HasMaxLength(200);
            e.Property(m => m.Bankname).HasMaxLength(100);
            e.Property(m => m.Iban).HasMaxLength(45);
            // Logo: bewusst ohne HasMaxLength -> varbinary(max); die Größe begrenzt die Oberfläche.
            e.Property(m => m.LogoContentType).HasMaxLength(100);
            e.Property(m => m.EtikettBreiteMm).HasDefaultValue(Mandant.StandardEtikettBreiteMm);
            e.Property(m => m.EtikettHoeheMm).HasDefaultValue(Mandant.StandardEtikettHoeheMm);
            e.Property(m => m.EtikettenProReihe).HasDefaultValue(Mandant.StandardEtikettenProReihe);
            e.Property(m => m.EtikettenProSpalte).HasDefaultValue(Mandant.StandardEtikettenProSpalte);
            KonfiguriereAktivUndZeitstempel(e);
        });

        modelBuilder.Entity<Marke>(e =>
        {
            e.ToTable(Tabellenname("Marken"));
            e.HasKey(m => m.MarkeID).HasName("PK_Marken");
            e.HasAlternateKey(m => new { m.MandantNr, m.MarkeID }).HasName("UQ_Marken_Mandant_MarkeID");
            e.HasIndex(m => new { m.MandantNr, m.Markenname }).IsUnique().HasDatabaseName("UQ_Marken_Mandant_Markenname");
            e.HasOne(m => m.Mandant).WithMany()
                .HasForeignKey(m => m.MandantNr)
                .HasConstraintName("FK_Marken_Mandanten")
                .OnDelete(DeleteBehavior.NoAction);
            e.Property(m => m.Markenname).HasMaxLength(100);
            e.Property(m => m.Registrierungsnummer).HasMaxLength(50);
            // Logo und LogoEtiketten: bewusst ohne HasMaxLength -> varbinary(max); die Größe begrenzt die Oberfläche.
            e.Property(m => m.LogoContentType).HasMaxLength(100);
            e.Property(m => m.LogoEtikettenContentType).HasMaxLength(100);
            KonfiguriereAktivUndZeitstempel(e);
        });

        modelBuilder.Entity<Artikel>(e =>
        {
            e.ToTable(Tabellenname("Artikel"), t => t.HasCheckConstraint("CK_Artikel_MwSt",
                "[Mehrwertsteuersatz] >= 0 AND [Mehrwertsteuersatz] <= 100"));
            e.HasKey(a => a.ArtikelID).HasName("PK_Artikel");
            e.HasAlternateKey(a => new { a.MandantNr, a.ArtikelID }).HasName("UQ_Artikel_Mandant_ArtikelID");
            e.HasIndex(a => new { a.MandantNr, a.Artikelnummer }).IsUnique().HasDatabaseName("UQ_Artikel_Mandant_Artikelnummer");
            e.HasOne(a => a.Mandant).WithMany()
                .HasForeignKey(a => a.MandantNr)
                .HasConstraintName("FK_Artikel_Mandanten")
                .OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.Marke).WithMany()
                .HasForeignKey(a => new { a.MandantNr, a.MarkeID })
                .HasPrincipalKey(m => new { m.MandantNr, m.MarkeID })
                .HasConstraintName("FK_Artikel_Marken")
                .OnDelete(DeleteBehavior.NoAction);

            e.Property(a => a.Artikelnummer).HasMaxLength(50);
            e.Property(a => a.Bezeichnung).HasMaxLength(200);
            e.Property(a => a.Kurzbeschreibung).HasMaxLength(500);
            // Beschreibung: bewusst ohne HasMaxLength -> nvarchar(max).

            // HasSentinel: ohne Sentinel würde EF Core den expliziten Wert 0 % als "nicht gesetzt"
            // werten und stattdessen den Datenbank-Default einfügen.
            e.Property(a => a.Mehrwertsteuersatz).HasPrecision(5, 2)
                .HasDefaultValue(Models.Artikel.StandardMehrwertsteuersatz)
                .HasSentinel(Models.Artikel.StandardMehrwertsteuersatz);
            KonfiguriereAktivUndZeitstempel(e);
        });

        modelBuilder.Entity<Artikelvariante>(e =>
        {
            e.ToTable(Tabellenname("Artikelvarianten"), t =>
            {
                t.HasCheckConstraint("CK_Artikelvariante_Preis", "[VerkaufspreisBrutto] >= 0");
                t.HasCheckConstraint("CK_Artikelvariante_Gewicht", "[GewichtGramm] IS NULL OR [GewichtGramm] >= 0");
            });
            e.HasKey(v => v.ArtikelvarianteID).HasName("PK_Artikelvariante");
            // Alternativschlüssel, damit Bestände die Variante über (MandantNr, ID) referenzieren können.
            e.HasAlternateKey(v => new { v.MandantNr, v.ArtikelvarianteID }).HasName("UQ_Artikelvariante_Mandant_ID");
            e.HasIndex(v => new { v.MandantNr, v.SKU }).IsUnique().HasDatabaseName("UQ_Artikelvariante_Mandant_SKU");
            // GTIN ist je Mandant eindeutig; mehrere Varianten ohne GTIN (NULL) sind erlaubt.
            e.HasIndex(v => new { v.MandantNr, v.GTIN }).IsUnique().HasFilter("[GTIN] IS NOT NULL").HasDatabaseName("UX_Artikelvariante_Mandant_GTIN");
            // Die ASIN ist wie SKU und EAN je Mandant eindeutig; mehrere Varianten ohne ASIN (NULL) sind erlaubt.
            e.HasIndex(v => new { v.MandantNr, v.AmazonAsin }).IsUnique().HasFilter("[AmazonAsin] IS NOT NULL")
                .HasDatabaseName("UX_Artikelvariante_Mandant_AmazonAsin");
            e.HasIndex(v => v.ArtikelID).HasDatabaseName("IX_Artikelvariante_ArtikelID");
            e.HasOne(v => v.Mandant).WithMany()
                .HasForeignKey(v => v.MandantNr)
                .HasConstraintName("FK_Artikelvariante_Mandanten")
                .OnDelete(DeleteBehavior.NoAction);
            e.HasOne(v => v.Artikel).WithMany(a => a.Varianten)
                .HasForeignKey(v => new { v.MandantNr, v.ArtikelID })
                .HasPrincipalKey(a => new { a.MandantNr, a.ArtikelID })
                .HasConstraintName("FK_Artikelvariante_Artikel")
                .OnDelete(DeleteBehavior.NoAction);

            e.Property(v => v.SKU).HasMaxLength(50);
            e.Property(v => v.GTIN).HasMaxLength(14);
            e.Property(v => v.Variantenname).HasMaxLength(200);
            e.Property(v => v.AmazonAsin).HasMaxLength(10);
            e.Property(v => v.Lieferant).HasMaxLength(200);
            e.Property(v => v.SkuLieferant).HasMaxLength(50);
            e.Property(v => v.HsCode).HasMaxLength(20);
            e.Property(v => v.Ursprungsland).HasMaxLength(2);
            // Nativer json-Datentyp (SQL Server 2025, Kompatibilitätsgrad 170): die Datenbank erzwingt gültiges JSON
            // selbst, ein ISJSON-Check ist nicht nötig. In C# bleibt es ein string.
            e.Property(v => v.BundleElemente).HasColumnType("json");
            e.Property(v => v.Variantenmerkmale).HasColumnType("json").HasDefaultValueSql("N'{}'");
            e.Property(v => v.VerkaufspreisBrutto).HasPrecision(10, 2);
            e.Property(v => v.EinkaufspreisNetto).HasPrecision(10, 2);
            e.Property(v => v.GewichtGramm).HasPrecision(10, 3);
            KonfiguriereAktivUndZeitstempel(e);
        });

        modelBuilder.Entity<BesucherLog>(e =>
        {
            e.ToTable(Tabellenname("BesucherLog"));
            e.HasKey(b => b.BesucherLogID).HasName("PK_BesucherLog");
            e.Property(b => b.ErstelltAm).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(b => b.Pfad).HasMaxLength(500);
            e.Property(b => b.IpAdresse).HasMaxLength(64);
            e.Property(b => b.UserAgent).HasMaxLength(500);
            e.Property(b => b.Email).HasMaxLength(256);
            e.HasIndex(b => b.ErstelltAm).HasDatabaseName("IX_BesucherLog_ErstelltAm");
            e.HasOne<Mandant>().WithMany()
                .HasForeignKey(b => b.MandantNr)
                .HasConstraintName("FK_BesucherLog_Mandanten")
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Lagerplatz>(e =>
        {
            e.ToTable(Tabellenname("Lagerplaetze"));
            e.HasKey(l => l.LagerplatzID).HasName("PK_Lagerplaetze");
            e.HasAlternateKey(l => new { l.MandantNr, l.LagerplatzID }).HasName("UQ_Lagerplaetze_Mandant_ID");
            e.HasIndex(l => new { l.MandantNr, l.Bezeichnung }).IsUnique().HasDatabaseName("UQ_Lagerplaetze_Mandant_Bezeichnung");
            e.HasOne(l => l.Mandant).WithMany()
                .HasForeignKey(l => l.MandantNr)
                .HasConstraintName("FK_Lagerplaetze_Mandanten")
                .OnDelete(DeleteBehavior.NoAction);
            e.Property(l => l.Bezeichnung).HasMaxLength(100);
            KonfiguriereAktivUndZeitstempel(e);
        });

        modelBuilder.Entity<ArtikelBestand>(e =>
        {
            e.ToTable(Tabellenname("ArtikelBestaende"));
            e.HasKey(b => b.ArtikelBestandID).HasName("PK_ArtikelBestaende");
            // Je Variante und Lagerplatz höchstens eine Bestandszeile.
            e.HasIndex(b => new { b.MandantNr, b.ArtikelvarianteID, b.LagerplatzID }).IsUnique()
                .HasDatabaseName("UQ_ArtikelBestaende_Mandant_Variante_Lagerplatz");
            e.HasOne(b => b.Mandant).WithMany()
                .HasForeignKey(b => b.MandantNr)
                .HasConstraintName("FK_ArtikelBestaende_Mandanten")
                .OnDelete(DeleteBehavior.NoAction);
            e.HasOne(b => b.Artikelvariante).WithMany()
                .HasForeignKey(b => new { b.MandantNr, b.ArtikelvarianteID })
                .HasPrincipalKey(v => new { v.MandantNr, v.ArtikelvarianteID })
                .HasConstraintName("FK_ArtikelBestaende_Artikelvarianten")
                .OnDelete(DeleteBehavior.NoAction);
            e.HasOne(b => b.Lagerplatz).WithMany()
                .HasForeignKey(b => new { b.MandantNr, b.LagerplatzID })
                .HasPrincipalKey(l => new { l.MandantNr, l.LagerplatzID })
                .HasConstraintName("FK_ArtikelBestaende_Lagerplaetze")
                .OnDelete(DeleteBehavior.NoAction);
            KonfiguriereZeitstempel(e);
        });
    }

    // HasSentinel: ohne Sentinel würde EF Core ein explizites Aktiv = false als "nicht gesetzt"
    // werten und stattdessen den Datenbank-Default (1) einfügen.
    private static void KonfiguriereAktivUndZeitstempel<T>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> e)
        where T : class, IMitZeitstempel
    {
        e.Property<bool>("Aktiv").HasDefaultValue(true).HasSentinel(true);
        KonfiguriereZeitstempel(e);
    }

    private static void KonfiguriereZeitstempel<T>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> e)
        where T : class, IMitZeitstempel
    {
        e.Property(x => x.ErstelltAm).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");
        e.Property(x => x.GeaendertAm).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetzeGeaendertAm();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        SetzeGeaendertAm();
        return base.SaveChanges();
    }

    // GeaendertAm hat nur beim Einfügen einen Datenbank-Default; bei Updates muss die App es setzen.
    private void SetzeGeaendertAm()
    {
        var jetzt = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<IMitZeitstempel>().Where(e => e.State == EntityState.Modified))
        {
            entry.Entity.GeaendertAm = jetzt;
        }
    }
}
