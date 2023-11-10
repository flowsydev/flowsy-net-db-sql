using System.Data;
using Dapper;

namespace Flowsy.Db.Sql;

public abstract partial class DbRepository
{
    /// <summary>
    /// Resolves the name for a stored routine using the conventions set for this repository.
    /// </summary>
    /// <param name="simpleName">The routine simple name.</param>
    /// <returns>The final routine name.</returns>
    protected virtual string ResolveRoutineName(string simpleName)
        => Options.Conventions.Routines.Format(simpleName);

    /// <summary>
    /// Resolves the default routine type using the conventions set for this repository.
    /// </summary>
    /// <returns>The default routine type.</returns>
    protected virtual DbRoutineType ResolveRoutineType()
        => Options.Conventions.Routines.DefaultType;

    /// <summary>
    /// Resolves the command type for a routine type using the conventions set for this repository.
    /// </summary>
    /// <param name="routineType">The routine type. If a null value is provided, the default value from conventions will be used.</param>
    /// <returns>The command type.</returns>
    protected virtual CommandType ResolveRoutineCommandType(DbRoutineType? routineType = null)
        => Options.Conventions.Routines.ResolveCommandType(routineType ?? Options.Conventions.Routines.DefaultType);

    /// <summary>
    /// Resolves the required SQL statement to invoke the target stored routine or stored function.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name.</param>
    /// <param name="parameters">The routine parameters.</param>
    /// <param name="routineType">The type of routine.</param>
    /// <returns>The final SQL statement for the routine.</returns>
    protected virtual string ResolveRoutineStatement(
        string routineSimpleName, 
        DynamicParameters parameters,
        DbRoutineType? routineType = null
        )
    {
        var routineName = ResolveRoutineName(routineSimpleName);
        var finalRoutineType = routineType ?? Options.Conventions.Routines.DefaultType;
        
        if (finalRoutineType != DbRoutineType.StoredFunction)
            return routineName;
        
        var parameterNames = string.Join(
            ", ",
            parameters.ParameterNames.Select(
                parameterName => ResolveRoutineParameterPlaceholder(
                    routineName,
                    finalRoutineType,
                    parameterName,
                    parameters.Get<object?>(parameterName)
                )
            )
            );
        return $"select * from {routineName}({parameterNames})";
    }

    /// <summary>
    /// Resolves the name for a routine parameter using the conventions set for this repository.
    /// </summary>
    /// <param name="simpleName">The parameter simple name</param>
    /// <returns>The final parameter name.</returns>
    protected virtual string ResolveRoutineParameterName(string simpleName)
        => Options.Conventions.Parameters.Format(simpleName);

    /// <summary>
    /// Resolves the name for a routine parameter using the conventions set for this repository.
    /// </summary>
    /// <param name="routineName">The routine name.</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="parameterName">The parameter name.</param>
    /// <param name="parameterValue">The parameter value.</param>
    /// <returns></returns>
    protected virtual string ResolveRoutineParameterPlaceholder(
        string routineName,
        DbRoutineType routineType,
        string parameterName,
        object? parameterValue
        )
        => Options.Conventions.Parameters.FormatPlaceholder(routineName, routineType, parameterName, parameterValue);
    
    /// <summary>
    /// Resolves the required DbType for the enum value.
    /// </summary>
    /// <param name="e">The enum value.</param>
    /// <returns>The DbType value.</returns>
    protected virtual DbType ResolveEnumType(Enum e)
        => Options.Conventions.Enums.ResolveType(e);

    /// <summary>
    /// Resolves the enum value based on the conventions set for this repository.
    /// </summary>
    /// <param name="e">The original enum value.</param>
    /// <returns>The final enum value.</returns>
    protected virtual object ResolveEnumValue(Enum e)
        => Options.Conventions.Enums.ResolveValue(e);
}