using Flowsy.Core;

namespace Flowsy.Db.Sql.Conventions;

/// <summary>
/// Represents a set of conventions used to handle database parameters.
/// </summary>
public sealed class DbParameterConvention : DbConvention
{
    public DbParameterConvention()
        : this(
            null, null, null,
            (_, _, parameterName, _) => $"@{parameterName}" 
            )
    {
    }

    public DbParameterConvention(string? prefix, string? suffix, CaseConvention? namingConvention, FormatRoutineParameterPlaceholder formatPlaceholder)
        : base(prefix, suffix, namingConvention)
    {
        FormatPlaceholder = formatPlaceholder;
    }
    
    /// <summary>
    /// A delegate to format parameter placeholders.
    /// </summary>
    public FormatRoutineParameterPlaceholder FormatPlaceholder { get; set; }
    
    public override DbConvention Clone()
        => new DbParameterConvention(Prefix, Suffix, Casing, FormatPlaceholder);
}