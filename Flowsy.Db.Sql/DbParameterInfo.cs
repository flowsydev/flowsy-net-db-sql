using System.Data;

namespace Flowsy.Db.Sql;

/// <summary>
/// Contains the information for a database parameter 
/// </summary>
public sealed class DbParameterInfo
{
    public DbParameterInfo()
        : this(string.Empty, null, null, null, null)
    {
    }

    public DbParameterInfo(string name, DbType? type, ParameterDirection? direction, int? size, object? value)
    {
        Name = name;
        Type = type;
        Direction = direction;
        Size = size;
        Value = value;
    }

    /// <summary>
    /// The parameter name.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The parameter data type.
    /// </summary>
    public DbType? Type { get; set; }
    
    /// <summary>
    /// The parameter direction.
    /// </summary>
    public ParameterDirection? Direction { get; set; }
    
    /// <summary>
    /// The parameter size.
    /// </summary>
    public int? Size { get; set; }
    
    /// <summary>
    /// The parameter value.
    /// </summary>
    public object? Value { get; set; }
}