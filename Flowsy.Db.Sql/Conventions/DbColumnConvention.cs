using Flowsy.Core;

namespace Flowsy.Db.Sql.Conventions;

public sealed class DbColumnConvention : DbConvention
{
    public DbColumnConvention()
    {
    }

    public DbColumnConvention(string? prefix, string? suffix, CaseConvention? casing) : base(prefix, suffix, casing)
    {
    }

    /// <summary>
    /// Formats the name of a column.
    /// </summary>
    /// <returns>The formatted column name.</returns>
    public override DbConvention Clone()
        => new DbColumnConvention(Prefix, Suffix, Casing);
}