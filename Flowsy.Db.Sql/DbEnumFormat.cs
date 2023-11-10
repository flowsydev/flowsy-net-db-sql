namespace Flowsy.Db.Sql;

/// <summary>
/// Specifies how to treat enum values when sent as query parameters.
/// </summary>
public enum DbEnumFormat
{
    /// <summary>
    /// The ordinal value is sent.
    /// </summary>
    Ordinal,
    
    /// <summary>
    /// The string representation is sent.
    /// </summary>
    Name
}