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
    protected virtual Task<int> ExecuteStatementAsync(string sql, CommandType commandType, dynamic param, CancellationToken cancellationToken)
        => ExecuteCommandAsync(new CommandDefinition(
            sql,
            ToDynamicParameters((object) param),
            transaction: Transaction,
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
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(string sql, CommandType commandType, dynamic param, CancellationToken cancellationToken)
        => GetManyAsync<T>(new CommandDefinition(
            sql,
            ToDynamicParameters((object) param),
            transaction: Transaction,
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
    protected virtual Task<T> GetFirstAsync<T>(string sql, CommandType commandType, dynamic param, CancellationToken cancellationToken)
        => GetFirstAsync<T>(new CommandDefinition(
            sql,
            ToDynamicParameters((object) param),
            transaction: Transaction,
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
    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(string sql, CommandType commandType, dynamic param, CancellationToken cancellationToken)
        => GetFirstOrDefaultAsync<T>(new CommandDefinition(
            sql,
            ToDynamicParameters((object) param),
            transaction: Transaction,
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
    /// <returns>A single result. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T> GetSingleAsync<T>(string sql, CommandType commandType, dynamic param, CancellationToken cancellationToken)
        => GetSingleAsync<T>(new CommandDefinition(
            sql,
            ToDynamicParameters((object) param),
            transaction: Transaction,
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
    /// <returns>The single result or the default value for T. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(string sql, CommandType commandType, dynamic param, CancellationToken cancellationToken)
        => GetSingleOrDefaultAsync<T>(new CommandDefinition(
            sql,
            ToDynamicParameters((object) param),
            transaction: Transaction,
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
    protected virtual Task<SqlMapper.GridReader> GetMultipleAsync(
        string sql,
        CommandType commandType,
        dynamic param,
        CancellationToken cancellationToken
        )
        => GetMultipleAsync(new CommandDefinition(
            sql,
            ToDynamicParameters((object) param),
            transaction: Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
            ));
}