using System.Data;
using Flowsy.Core;

namespace Flowsy.Db.Sql.Conventions;

/// <summary>
/// Represents a set of conventions used to handle database routines.
/// </summary>
public sealed class DbRoutineConvention : DbConvention
{
    public DbRoutineConvention()
        : this(null, null, null, DbRoutineType.StoredProcedure)
    {
    }

    public DbRoutineConvention(string? prefix, string? suffix, CaseConvention? namingConvention, DbRoutineType defaultType)
        : base(prefix, suffix, namingConvention)
    {
        DefaultType = defaultType;
    }
    
    /// <summary>
    /// Default routine type to use when executing queries.
    /// </summary>
    public DbRoutineType DefaultType { get; set; }

    /// <summary>
    /// Resolves a CommandType value based on the provided DbRoutineType.
    /// </summary>
    /// <param name="routineType"></param>
    /// <returns></returns>
    public CommandType ResolveCommandType(DbRoutineType routineType)
        => routineType == DbRoutineType.StoredFunction ? CommandType.Text : CommandType.StoredProcedure;
    
    public override DbConvention Clone()
        => new DbRoutineConvention(Prefix, Suffix, Casing, DefaultType);
}