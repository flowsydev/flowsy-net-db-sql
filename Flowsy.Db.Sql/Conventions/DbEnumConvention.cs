using System.Data;
using Flowsy.Core;

namespace Flowsy.Db.Sql.Conventions;

/// <summary>
/// Represents a set of conventions used to handle enum values.
/// </summary>
public sealed class DbEnumConvention : DbConvention
{
    public DbEnumConvention()
        : this(null, null, null, DbEnumFormat.Name)
    {
    }

    public DbEnumConvention(string? prefix, string? suffix, CaseConvention? casing, DbEnumFormat valueFormat) 
        : base(prefix, suffix, casing)
    {
        ValueFormat = valueFormat;
    }
    
    /// <summary>
    /// The format used to handle enum values.
    /// </summary>
    public DbEnumFormat ValueFormat { get; set; }

    /// <summary>
    /// Resolves the final type for an enum sent to the underlying database. 
    /// </summary>
    /// <param name="e">The enum value</param>
    /// <returns>The final type used to send the enum value to the underlying database.</returns>
    public DbType ResolveType(Enum e)
        => ValueFormat == DbEnumFormat.Name
            ? DbType.String
            : ResolveOrdinalType(e);
    
    /// <summary>
    /// Resolves the final value for an enum sent to the underlying database. 
    /// </summary>
    /// <param name="e">The enum value</param>
    /// <returns>The final value sent to the underlying database.</returns>
    public object ResolveValue(Enum e)
        => ValueFormat == DbEnumFormat.Name
            ? $"{Prefix}{e.ToString()?.ApplyConvention(Casing) ?? ResolveOrdinalValue(e)}{Suffix}"
            : ResolveOrdinalValue(e);

    /// <summary>
    /// Resolves the ordinal value for an enum sent to the underlying database.
    /// </summary>
    /// <param name="e">The enum value</param>
    /// <returns></returns>
    public object ResolveOrdinalValue(Enum e)
        => e.GetTypeCode() switch
        {
            TypeCode.Byte => Convert.ToByte(e),
            TypeCode.Int16 => Convert.ToInt16(e),
            TypeCode.Int64 => Convert.ToInt64(e),
            _ => Convert.ToInt32(e)
        };
    
    /// <summary>
    /// Resolves the type for an enum sent to the underlying database.
    /// </summary>
    /// <param name="e">The enum value</param>
    /// <returns>The database type.</returns>
    public DbType ResolveOrdinalType(Enum e)
        => e.GetTypeCode() switch
        {
            TypeCode.Byte => DbType.Byte,
            TypeCode.Int16 => DbType.Int16,
            TypeCode.Int64 => DbType.Int64,
            _ => DbType.Int32
        };

    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <returns>A new instance with the same values as the current instance.</returns>
    public override DbConvention Clone()
        => new DbEnumConvention(Prefix, Suffix, Casing, ValueFormat);
}