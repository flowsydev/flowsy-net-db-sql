namespace Flowsy.Db.Sql;

/// <summary>
/// Specifies the format used to send DateTimeOffset values to the underlying database.
/// </summary>
public enum DbDateTimeOffsetFormat
{
    /// <summary>
    /// Local time zone
    /// </summary>
    Local,
    
    /// <summary>
    /// Central time zone
    /// </summary>
    Utc
}