using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ShopifySQL.Web.Services;

/// <summary>Erkennt typische SQL-Server-Fehler beim Speichern, um verständliche Meldungen zu zeigen.</summary>
public static class DbFehler
{
    private const int DuplicateKeyIndex = 2601;
    private const int DuplicateKeyConstraint = 2627;
    private const int ForeignKeyViolation = 547;

    public static bool IstEindeutigkeitsverletzung(DbUpdateException ex) =>
        ex.InnerException is SqlException { Number: DuplicateKeyIndex or DuplicateKeyConstraint };

    public static bool IstFremdschluesselverletzung(DbUpdateException ex) =>
        ex.InnerException is SqlException { Number: ForeignKeyViolation };
}
