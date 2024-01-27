using Dapper;

namespace Flowsy.Db.Sql;

public abstract partial class DbRepository
{
    /// <summary>
    /// Executes a statement from a CommandDefinition and returns the number of affected rows.
    /// </summary>
    /// <param name="command">The command definition.</param>
    /// <returns>The number of affected rows.</returns>
    protected virtual Task<int> ExecuteCommandAsync(CommandDefinition command)
        => ExecuteAsync(c => c.ExecuteAsync(command));
    
    /// <summary>
    /// Executes a query from a CommandDefinition and returns a list of results.
    /// </summary>
    /// <param name="command">The command definition.</param>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <returns>The first result or the default value for T.</returns>
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(CommandDefinition command)
        => ExecuteAsync(c => c.QueryAsync<T>(command));
    
    /// <summary>
    /// Executes a query from a CommandDefinition and returns the first result.
    /// </summary>
    /// <param name="command">The command definition.</param>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <returns>The first element of the query results.</returns>
    protected virtual Task<T> GetFirstAsync<T>(CommandDefinition command)
        => ExecuteAsync(c => c.QueryFirstAsync<T>(command));
    
    /// <summary>
    /// Executes a query from a CommandDefinition and returns the first result or a default value if no results were found.
    /// </summary>
    /// <param name="command">The command definition.</param>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <returns>The first result or the default value for T.</returns>
    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(CommandDefinition command)
        => ExecuteAsync(c => c.QueryFirstOrDefaultAsync<T>(command));

    /// <summary>
    /// Executes a query from a CommandDefinition and returns a single result.
    /// </summary>
    /// <param name="command">The command definition.</param>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <returns>A single result. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T> GetSingleAsync<T>(CommandDefinition command)
        => ExecuteAsync(c => c.QuerySingleAsync<T>(command));

    /// <summary>
    /// Executes a query from a CommandDefinition and returns a single result or a default value if no results were found.
    /// </summary>
    /// <param name="command">The command definition.</param>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <returns>The single result or the default value for T. An exception is thrown if more than one result are found.</returns>
    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(CommandDefinition command)
        => ExecuteAsync(c => c.QuerySingleOrDefaultAsync<T>(command));

    /// <summary>
    /// Executes a query from a CommandDefinition and returns multiple result sets.
    /// </summary>
    /// <param name="command">The command definition.</param>
    /// <returns>The result sets as a GridReader.</returns>
    protected virtual Task<SqlMapper.GridReader> GetMultipleAsync(CommandDefinition command)
        => ExecuteAsync(c => c.QueryMultipleAsync(command));
}