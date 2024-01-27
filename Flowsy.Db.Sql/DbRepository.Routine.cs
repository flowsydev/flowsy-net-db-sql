using System.Data;
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
        dynamic? param,
        CancellationToken cancellationToken
    )
        => ExecuteRoutineAsync(simpleName, ResolveRoutineType(), param as object, cancellationToken);
    
    /// <summary>
    /// Executes a stored routine and returns the number of affected rows.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="simpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <returns>The number of affected records.</returns>
    protected virtual Task<int> ExecuteRoutineAsync(
        IDbConnection connection,
        string simpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => ExecuteRoutineAsync(connection, null, simpleName, ResolveRoutineType(), param as object, cancellationToken);
    
    /// <summary>
    /// Executes a stored routine and returns the number of affected rows.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="simpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <returns>The number of affected records.</returns>
    protected virtual Task<int> ExecuteRoutineAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        string simpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => ExecuteRoutineAsync(connection, transaction, simpleName, ResolveRoutineType(), param as object, cancellationToken);

    /// <summary>
    /// Executes a stored routine and returns the number of affected records.
    /// </summary>
    /// <param name="simpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine (StoredProcedure, StoredFunction).</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <returns>The number of affected records.</returns>
    protected virtual async Task<int> ExecuteRoutineAsync(
        string simpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var connection = GetConnection();
        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            
            return await ExecuteRoutineAsync(
                connection,
                Transaction,
                simpleName,
                routineType,
                param as object,
                cancellationToken
                );
        }
        catch (Exception exception)
        {
            if (ExceptionHandler is null)
                throw;
            
            var newException = ExceptionHandler?.Handle(exception);
            if (newException is not null)
                throw newException;

            throw;
        }
        finally
        {
            await TryDisposeConnectionAsync(connection);
        }
    }

    /// <summary>
    /// Executes a stored routine and returns the number of affected records.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="simpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine (StoredProcedure, StoredFunction).</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <returns>The number of affected records.</returns>
    protected virtual Task<int> ExecuteRoutineAsync(
        IDbConnection connection,
        string simpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => ExecuteRoutineAsync(
            connection,
            null,
            simpleName,
            routineType,
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a stored routine and returns the number of affected records.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="simpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine (StoredProcedure, StoredFunction).</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <returns>The number of affected records.</returns>
    protected virtual Task<int> ExecuteRoutineAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        string simpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return connection.ExecuteAsync(new CommandDefinition(
            ResolveRoutineStatement(simpleName, dynamicParameters, routineType),
            dynamicParameters,
            transaction,
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
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetManyAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);
    
    /// <summary>
    /// Executes a stored routine and returns a list of results.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The list of results.</returns>
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(
        IDbConnection connection,
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetManyAsync<T>(connection, null, routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);
    
    /// <summary>
    /// Executes a stored routine and returns a list of results.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The list of results.</returns>
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetManyAsync<T>(connection, transaction, routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);

    /// <summary>
    /// Executes a stored routine and returns a list of results.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The list of results.</returns>
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(
        IDbConnection connection,
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetManyAsync<T>(
            connection,
            null,
            routineSimpleName,
            routineType,
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a stored routine and returns a list of results.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The list of results.</returns>
    protected virtual async Task<IEnumerable<T>> GetManyAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var connection = GetConnection();
        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            
            return await GetManyAsync<T>(
                connection,
                Transaction,
                routineSimpleName, 
                routineType,
                param as object,
                cancellationToken
                );
        }
        catch (Exception exception)
        {
            if (ExceptionHandler is null)
                throw;
            
            var newException = ExceptionHandler?.Handle(exception);
            if (newException is not null)
                throw newException;

            throw;
        }
        finally
        {
            await TryDisposeConnectionAsync(connection);
        }
    }

    /// <summary>
    /// Executes a stored routine and returns a list of results.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The parameters for the routine.</param>
    /// <param name="cancellationToken">The cancellation token for the query.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The list of results.</returns>
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return connection.QueryAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            transaction,
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
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetFirstAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);

    /// <summary>
    /// Executes a SQL statement and returns the first result.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual Task<T> GetFirstAsync<T>(
        IDbConnection connection,
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetFirstAsync<T>(
            connection,
            null,
            routineSimpleName,
            ResolveRoutineType(),
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns the first result.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual Task<T> GetFirstAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetFirstAsync<T>(
            connection,
            transaction,
            routineSimpleName,
            ResolveRoutineType(),
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns the first result.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual async Task<T> GetFirstAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var connection = GetConnection();
        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            
            return await GetFirstAsync<T>(
                connection,
                Transaction,
                routineSimpleName,
                routineType,
                param as object,
                cancellationToken
                );
        }
        catch (Exception exception)
        {
            if (ExceptionHandler is null)
                throw;
            
            var newException = ExceptionHandler?.Handle(exception);
            if (newException is not null)
                throw newException;

            throw;
        }
        finally
        {
            await TryDisposeConnectionAsync(connection);
        }
    }

    /// <summary>
    /// Executes a SQL statement and returns the first result.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual Task<T> GetFirstAsync<T>(
        IDbConnection connection,
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetFirstAsync<T>(
            connection,
            null,
            routineSimpleName,
            routineType,
            param as object,
            cancellationToken
            );
    
    /// <summary>
    /// Executes a SQL statement and returns the first result.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual Task<T> GetFirstAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return connection.QueryFirstAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
            ));
    }

    /// <summary>
    /// Executes a SQL statement and returns the first result or a default value if no results were found.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetFirstOrDefaultAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);

    /// <summary>
    /// Executes a SQL statement and returns the first result or a default value if no results were found.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(
        IDbConnection connection,
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetFirstOrDefaultAsync<T>(
            connection,
            null,
            routineSimpleName,
            ResolveRoutineType(),
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns the first result or a default value if no results were found.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetFirstOrDefaultAsync<T>(
            connection,
            transaction,
            routineSimpleName,
            ResolveRoutineType(),
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns the first result.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results or a default value if no results are found.</returns>
    protected virtual async Task<T?> GetFirstOrDefaultAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var connection = GetConnection();
        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            
            return await GetFirstOrDefaultAsync<T>(
                connection,
                Transaction,
                routineSimpleName,
                routineType,
                param as object,
                cancellationToken
                );
        }
        catch (Exception exception)
        {
            if (ExceptionHandler is null)
                throw;
            
            var newException = ExceptionHandler?.Handle(exception);
            if (newException is not null)
                throw newException;

            throw;
        }
        finally
        {
            await TryDisposeConnectionAsync(connection);
        }
    }

    /// <summary>
    /// Executes a SQL statement and returns the first result.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results or a default value if no results are found.</returns>
    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(
        IDbConnection connection,
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetFirstOrDefaultAsync<T>(
            connection, 
            null,
            routineSimpleName,
            routineType,
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns the first result.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results or a default value if no results are found.</returns>
    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            transaction,
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
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetSingleAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);

    /// <summary>
    /// Executes a SQL statement and returns a single result.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>A single result. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T> GetSingleAsync<T>(
        IDbConnection connection,
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetSingleAsync<T>(
            connection,
            null,
            routineSimpleName,
            ResolveRoutineType(),
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns a single result.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>A single result. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T> GetSingleAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetSingleAsync<T>(
            connection,
            transaction,
            routineSimpleName,
            ResolveRoutineType(),
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns a single result.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>A single result. An exception is thrown if more than one result are found.</returns>
    protected virtual async Task<T> GetSingleAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var connection = GetConnection();
        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            
            return await GetSingleAsync<T>(
                connection,
                Transaction,
                routineSimpleName,
                routineType,
                param as object,
                cancellationToken
                );
        }
        catch (Exception exception)
        {
            if (ExceptionHandler is null)
                throw;
            
            var newException = ExceptionHandler?.Handle(exception);
            if (newException is not null)
                throw newException;

            throw;
        }
        finally
        {
            await TryDisposeConnectionAsync(connection);
        }
    }

    /// <summary>
    /// Executes a SQL statement and returns a single result.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>A single result. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T> GetSingleAsync<T>(
        IDbConnection connection,
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetSingleAsync<T>(
            connection, 
            null,
            routineSimpleName,
            routineType,
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns a single result.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>A single result. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T> GetSingleAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return connection.QuerySingleAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            transaction,
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
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetSingleOrDefaultAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);

    /// <summary>
    /// Executes a SQL statement and returns a single result or a default value if no results were found.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The single result or the default value for T. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(
        IDbConnection connection,
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetSingleOrDefaultAsync<T>(
            connection,
            null,
            routineSimpleName,
            ResolveRoutineType(),
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns a single result or a default value if no results were found.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The single result or the default value for T. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetSingleOrDefaultAsync<T>(
            connection,
            transaction,
            routineSimpleName,
            ResolveRoutineType(),
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns a single result or a default value if no results were found.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The single result or the default value for T. An exception is thrown if more than one result are found.</returns>
    protected virtual async Task<T?> GetSingleOrDefaultAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var connection = GetConnection();
        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            
            return await GetSingleOrDefaultAsync<T>(
                connection, 
                Transaction,
                routineSimpleName,
                routineType,
                param as object,
                cancellationToken
                );
        }
        catch (Exception exception)
        {
            if (ExceptionHandler is null)
                throw;
            
            var newException = ExceptionHandler?.Handle(exception);
            if (newException is not null)
                throw newException;

            throw;
        }
        finally
        {
            await TryDisposeConnectionAsync(connection);
        }
    }

    /// <summary>
    /// Executes a SQL statement and returns a single result or a default value if no results were found.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The single result or the default value for T. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(
        IDbConnection connection,
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetSingleOrDefaultAsync<T>(
            connection, 
            null,
            routineSimpleName,
            routineType,
            param as object,
            cancellationToken
            );
    
    /// <summary>
    /// Executes a SQL statement and returns a single result or a default value if no results were found.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The single result or the default value for T. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            transaction,
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
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetMultipleAsync(
            routineSimpleName,
            ResolveRoutineType(),
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns multiple result sets.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The result sets as a GridReader.</returns>
    protected virtual Task<SqlMapper.GridReader> GetMultipleAsync(
        IDbConnection connection,
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetMultipleAsync(
            connection,
            null,
            routineSimpleName,
            ResolveRoutineType(),
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns multiple result sets.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The result sets as a GridReader.</returns>
    protected virtual Task<SqlMapper.GridReader> GetMultipleAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetMultipleAsync(
            connection,
            transaction,
            routineSimpleName,
            ResolveRoutineType(),
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns multiple result sets.
    /// </summary>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The result sets as a GridReader.</returns>
    protected virtual async Task<SqlMapper.GridReader> GetMultipleAsync(
        string routineSimpleName,
        DbRoutineType routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var connection = GetConnection();
        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            
            return await GetMultipleAsync(
                connection,
                Transaction,
                routineSimpleName,
                routineType,
                param as object,
                cancellationToken
                );
        }
        catch (Exception exception)
        {
            if (ExceptionHandler is null)
                throw;
            
            var newException = ExceptionHandler?.Handle(exception);
            if (newException is not null)
                throw newException;

            throw;
        }
        finally
        {
            await TryDisposeConnectionAsync(connection);
        }
    }

    /// <summary>
    /// Executes a SQL statement and returns multiple result sets.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The result sets as a GridReader.</returns>
    protected virtual Task<SqlMapper.GridReader> GetMultipleAsync(
        IDbConnection connection,
        string routineSimpleName,
        DbRoutineType routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetMultipleAsync(
            connection,
            null,
            routineSimpleName,
            routineType,
            param as object,
            cancellationToken
            );

    /// <summary>
    /// Executes a SQL statement and returns multiple result sets.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="routineSimpleName">The routine simple name (UserCreate, UserPatchEmail, UserDelete, etc.)</param>
    /// <param name="routineType">The type of routine.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The result sets as a GridReader.</returns>
    protected virtual Task<SqlMapper.GridReader> GetMultipleAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return connection.QueryMultipleAsync(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
            ));
    }
}