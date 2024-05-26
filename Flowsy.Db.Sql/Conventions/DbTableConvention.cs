using Flowsy.Core;

namespace Flowsy.Db.Sql.Conventions;

/// <summary>
/// Represents a set of conventions used to handle database tables.
/// </summary>
public sealed class DbTableConvention : DbSchemaChildConvention
{
    public DbTableConvention() : this(string.Empty, string.Empty, null)
    {
    }

    public DbTableConvention(string? prefix, string? suffix, CaseConvention? casing) : base(prefix, suffix, casing)
    {
    }

    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <returns>A new instance with the same values as the current instance.</returns>
    public override DbConvention Clone()
        => new DbTableConvention(Prefix, Suffix, Casing);
}