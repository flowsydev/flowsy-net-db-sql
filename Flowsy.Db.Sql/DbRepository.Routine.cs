using Dapper;

namespace Flowsy.Db.Sql;

public abstract partial class DbRepository
{
    /// <summary>
    /// Executes a stored routine and returns the number of affected rows.
    /// </summary>
    /// <param name="simpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <returns>The number of affected records.</returns>
    protected virtual Task<int> ExecuteRoutineAsync(
        string simpleName,
        dynamic param,
        CancellationToken cancellationToken
    )
        => ExecuteRoutineAsync(simpleName, ResolveRoutineType(), (object) param, cancellationToken);
    
    /// <summary>
    /// Executes a stored routine and returns the number of affected records.
    /// </summary>
    /// <param name="simpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine (StoredProcedure, StoredFunction).</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <returns>The number of affected records.</returns>
    protected virtual Task<int> ExecuteRoutineAsync(
        string simpleName,
        DbRoutineType? routineType,
        dynamic param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = ToDynamicParameters((object) param);
        return ExecuteCommandAsync(new CommandDefinition(
            ResolveRoutineStatement(simpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
            ));
    }

    /// <summary>
    /// Executes a stored routine and returns a list of results.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The list of results.</returns>
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(
        string routineSimpleName,
        dynamic param,
        CancellationToken cancellationToken
        )
        => GetManyAsync<T>(routineSimpleName, ResolveRoutineType(), (object) param, cancellationToken);
    
    /// <summary>
    /// Executes a stored routine and returns a list of results.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The list of results.</returns>
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = ToDynamicParameters((object) param);
        return GetManyAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
            ));
    }

    /// <summary>
    /// Executes a SQL statement and returns the first result.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual Task<T> GetFirstAsync<T>(
        string routineSimpleName,
        dynamic param,
        CancellationToken cancellationToken
        )
        => GetFirstAsync<T>(routineSimpleName, ResolveRoutineType(), (object) param, cancellationToken);

    /// <summary>
    /// Executes a SQL statement and returns the first result.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual Task<T> GetFirstAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic param,
        CancellationToken cancellationToken
        )
    {
        var dynamicParameters = ToDynamicParameters((object) param);
        return GetFirstAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
            ));
    }

    /// <summary>
    /// Executes a SQL statement and returns a single result.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>A single result. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T> GetSingleAsync<T>(
        string routineSimpleName,
        dynamic param,
        CancellationToken cancellationToken
        )
        => GetSingleAsync<T>(routineSimpleName, ResolveRoutineType(), (object) param, cancellationToken);

    /// <summary>
    /// Executes a SQL statement and returns a single result.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>A single result. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T> GetSingleAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic param,
        CancellationToken cancellationToken
        )
    {
        var dynamicParameters = ToDynamicParameters((object) param);
        return GetSingleAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
        ));
    }

    /// <summary>
    /// Executes a SQL statement and returns a single result or a default value if no results were found.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The single result or the default value for T. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(
        string routineSimpleName,
        dynamic param,
        CancellationToken cancellationToken
        )
        => GetSingleOrDefaultAsync<T>(routineSimpleName, ResolveRoutineType(), (object) param, cancellationToken);
    
    /// <summary>
    /// Executes a SQL statement and returns a single result or a default value if no results were found.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The single result or the default value for T. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic param,
        CancellationToken cancellationToken
        )
    {
        var dynamicParameters = ToDynamicParameters((object) param);
        return GetSingleOrDefaultAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
            ));
    }

    /// <summary>
    /// Executes a SQL statement and returns multiple result sets.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The result sets as a GridReader.</returns>
    protected virtual Task<SqlMapper.GridReader> GetMultipleAsync(
        string routineSimpleName,
        dynamic param,
        CancellationToken cancellationToken
        )
        => GetMultipleAsync(routineSimpleName, ResolveRoutineType(), (object) param, cancellationToken);

    /// <summary>
    /// Executes a SQL statement and returns multiple result sets.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The result sets as a GridReader.</returns>
    protected virtual Task<SqlMapper.GridReader> GetMultipleAsync(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic param,
        CancellationToken cancellationToken
        )
    {
        var dynamicParameters = ToDynamicParameters((object) param);
        return GetMultipleAsync(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
            ));
    }
}