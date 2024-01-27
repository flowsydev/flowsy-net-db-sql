using System.Data;
using Dapper;

namespace Flowsy.Db.Sql;

public abstract partial class DbRepository
{
    /// <summary>
    /// Executes a SQL statement and returns the number of affected rows.
    /// </summary>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The number of affected rows.</returns>
    protected virtual async Task<int> ExecuteStatementAsync(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
        )
    {
        var connection = GetConnection();
        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            
            return await ExecuteStatementAsync(
                connection,
                Transaction,
                sql,
                commandType,
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
    /// Executes a SQL statement and returns the number of affected rows.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The number of affected rows.</returns>
    protected virtual Task<int> ExecuteStatementAsync(
        IDbConnection connection,
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => ExecuteStatementAsync(
            connection,
            null,
            sql,
            commandType,
            param as object,
            cancellationToken
            );
    
    /// <summary>
    /// Executes a SQL statement and returns the number of affected rows.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The number of affected rows.</returns>
    protected virtual Task<int> ExecuteStatementAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => connection.ExecuteAsync(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters(param as object) : null,
            transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
            ));
    
    /// <summary>
    /// Executes a SQL statement and returns a list of results.
    /// </summary>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected results.</typeparam>
    /// <returns>A list of results.</returns>
    protected virtual async Task<IEnumerable<T>> GetManyAsync<T>(
        string sql,
        CommandType commandType,
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
                sql,
                commandType,
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
    /// Executes a SQL statement and returns a list of results.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected results.</typeparam>
    /// <returns>A list of results.</returns>
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(
        IDbConnection connection,
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => GetManyAsync<T>(
            connection,
            null,
            sql,
            commandType,
            param as object,
            cancellationToken
            );
    
    /// <summary>
    /// Executes a SQL statement and returns a list of results.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected results.</typeparam>
    /// <returns>A list of results.</returns>
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => connection.QueryAsync<T>(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters((object) param) : null,
            transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
            ));

    /// <summary>
    /// Executes a SQL statement and returns the first result.
    /// </summary>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual async Task<T> GetFirstAsync<T>(
        string sql,
        CommandType commandType,
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
                sql,
                commandType,
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
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual Task<T> GetFirstAsync<T>(
        IDbConnection connection,
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetFirstAsync<T>(
            connection,
            null,
            sql,
            commandType,
            param as object,
            cancellationToken
            );
    
    /// <summary>
    /// Executes a SQL statement and returns the first result.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The first element of the query results. An exception is thrown if no results are found.</returns>
    protected virtual Task<T> GetFirstAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => connection.QueryFirstAsync<T>(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters((object) param) : null,
            transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
            ));

    /// <summary>
    /// Executes a SQL statement and returns the first result or a default value if no results were found.
    /// </summary>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>An instance of T or its default value.</returns>
    protected virtual async Task<T?> GetFirstOrDefaultAsync<T>(
        string sql,
        CommandType commandType,
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
                sql,
                commandType,
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
    /// Executes a SQL statement and returns the first result or a default value if no results were found.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>An instance of T or its default value.</returns>
    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(
        IDbConnection connection,
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetFirstOrDefaultAsync<T>(
            connection,
            null,
            sql,
            commandType,
            param as object,
            cancellationToken
            );
    
    /// <summary>
    /// Executes a SQL statement and returns the first result or a default value if no results were found.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>An instance of T or its default value.</returns>
    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters((object) param) : null,
            transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
            ));
    
    /// <summary>  
    /// Executes a SQL statement and returns a single result.  
    /// </summary>  
    /// <param name="sql">The SQL statement.</param>  
    /// <param name="commandType">The type of command.</param>  
    /// <param name="param">The statement parameters.</param>  
    /// <param name="cancellationToken">The cancellation token for the operation.</param>  
    /// <typeparam name="T">The type of the expected result.</typeparam>  
    /// <returns>A single result. An exception is thrown if more than one result is found.</returns>  
    protected virtual async Task<T> GetSingleAsync<T>(  
        string sql,  
        CommandType commandType,  
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
                sql,  
                commandType,  
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
    /// <param name="sql">The SQL statement.</param>  
    /// <param name="commandType">The type of command.</param>  
    /// <param name="param">The statement parameters.</param>  
    /// <param name="cancellationToken">The cancellation token for the operation.</param>  
    /// <typeparam name="T">The type of the expected result.</typeparam>  
    /// <returns>A single result. An exception is thrown if more than one result is found.</returns>  
    protected virtual Task<T> GetSingleAsync<T>(  
        IDbConnection connection,  
        string sql,  
        CommandType commandType,  
        dynamic? param,  
        CancellationToken cancellationToken  
    )  
        => GetSingleAsync<T>(  
            connection,  
            null,  
            sql,  
            commandType,  
            param as object,  
            cancellationToken  
            );
    
    /// <summary>
    /// Executes a SQL statement and returns a single result.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>A single result. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T> GetSingleAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => connection.QuerySingleAsync<T>(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters((object) param) : null,
            transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
            ));
    
    /// <summary>  
    /// Executes a SQL statement and returns a single result or a default value if no results were found.  
    /// </summary>  
    /// <param name="sql">The SQL statement.</param>  
    /// <param name="commandType">The type of command.</param>  
    /// <param name="param">The statement parameters.</param>  
    /// <param name="cancellationToken">The cancellation token for the operation.</param>  
    /// <typeparam name="T">The type of the expected result.</typeparam>  
    /// <returns>The single result or the default value for T. An exception is thrown if more than one result is found.</returns>  
    protected virtual async Task<T?> GetSingleOrDefaultAsync<T>(  
        string sql,  
        CommandType commandType,  
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
                sql,  
                commandType,  
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
    /// <param name="sql">The SQL statement.</param>  
    /// <param name="commandType">The type of command.</param>  
    /// <param name="param">The statement parameters.</param>  
    /// <param name="cancellationToken">The cancellation token for the operation.</param>  
    /// <typeparam name="T">The type of the expected result.</typeparam>  
    /// <returns>The single result or the default value for T. An exception is thrown if more than one result is found.</returns>  
    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(  
        IDbConnection connection,  
        string sql,  
        CommandType commandType,  
        dynamic? param,  
        CancellationToken cancellationToken  
        )  
        => GetSingleOrDefaultAsync<T>(  
            connection,  
            null,  
            sql,  
            commandType,  
            param as object,  
            cancellationToken  
            );

    
    /// <summary>
    /// Executes a SQL statement and returns a single result or a default value if no results were found.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The single result or the default value for T. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(
        IDbConnection connection,
        IDbTransaction? transaction,
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters((object) param) : null,
            transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
            ));
    
    /// <summary>  
    /// Executes a SQL statement and returns multiple result sets.  
    /// </summary>  
    /// <param name="sql">The SQL statement.</param>  
    /// <param name="commandType">The type of command.</param>  
    /// <param name="param">The statement parameters.</param>  
    /// <param name="cancellationToken">The cancellation token for the operation.</param>  
    /// <returns>The result sets as a GridReader.</returns>  
    protected virtual async Task<SqlMapper.GridReader> GetMultipleAsync(  
        string sql,  
        CommandType commandType,  
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
                sql,  
                commandType,  
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
    /// <param name="sql">The SQL statement.</param>  
    /// <param name="commandType">The type of command.</param>  
    /// <param name="param">The statement parameters.</param>  
    /// <param name="cancellationToken">The cancellation token for the operation.</param>  
    /// <returns>The result sets as a GridReader.</returns>  
    protected virtual Task<SqlMapper.GridReader> GetMultipleAsync(  
        IDbConnection connection,  
        string sql,  
        CommandType commandType,  
        dynamic? param,  
        CancellationToken cancellationToken  
    )  
        => GetMultipleAsync(  
            connection,  
            null,  
            sql,  
            commandType,  
            param as object,  
            cancellationToken  
            );
    
    /// <summary>
    /// Executes a SQL statement and returns multiple result sets.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">An optional database transaction.</param>
    /// <param name="sql">The SQL statement.</param>
    /// <param name="commandType">The type of command.</param>
    /// <param name="param">The statement parameters.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The result sets as a GridReader.</returns>
    protected virtual Task<SqlMapper.GridReader> GetMultipleAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
        )
        => connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters((object) param) : null,
            transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
            ));
}