using System.Data;
using Dapper;

namespace Flowsy.Db.Sql;

public abstract partial class DbRepository
{
    protected virtual async Task<int> ExecuteStatementAsync(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var preExecutionContext = new DbStatementPreExecutionContext(
            sql,
            commandType,
            param is not null ? ToDynamicParameters(param as object) : new DynamicParameters()
        );
        OnExecutingStatement(preExecutionContext);
        
        LogCommandExecution(CommandType.Text, sql, preExecutionContext.Parameters);
        
        var result = await Connection.ExecuteAsync(new CommandDefinition(
            sql,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnStatementExecuted(new DbStatementPostExecutionContext<int>(
            preExecutionContext.CommandText,
            preExecutionContext.CommandType,
            preExecutionContext.Parameters,
            result
        ));
    }

    protected virtual async Task<IEnumerable<T>> GetManyAsync<T>(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var preExecutionContext = new DbStatementPreExecutionContext(
            sql,
            commandType,
            param is not null ? ToDynamicParameters(param as object) : new DynamicParameters()
        );
        OnExecutingStatement(preExecutionContext);
        
        LogCommandExecution(commandType, sql, preExecutionContext.Parameters);
        
        var result = await Connection.QueryAsync<T>(new CommandDefinition(
            sql,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnStatementExecuted(new DbStatementPostExecutionContext<IEnumerable<T>>(
            preExecutionContext.CommandText,
            preExecutionContext.CommandType,
            preExecutionContext.Parameters,
            result
        ));
    }

    protected virtual async Task<T> GetFirstAsync<T>(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var preExecutionContext = new DbStatementPreExecutionContext(
            sql,
            commandType,
            param is not null ? ToDynamicParameters(param as object) : new DynamicParameters()
        );
        OnExecutingStatement(preExecutionContext);
        
        LogCommandExecution(commandType, sql, preExecutionContext.Parameters);
        
        var result = await Connection.QueryFirstAsync<T>(new CommandDefinition(
            sql,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnStatementExecuted(new DbStatementPostExecutionContext<T>(
            preExecutionContext.CommandText,
            preExecutionContext.CommandType,
            preExecutionContext.Parameters,
            result
        ));
    }

    protected virtual async Task<T?> GetFirstOrDefaultAsync<T>(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var preExecutionContext = new DbStatementPreExecutionContext(
            sql,
            commandType,
            param is not null ? ToDynamicParameters(param as object) : new DynamicParameters()
        );
        OnExecutingStatement(preExecutionContext);
        
        LogCommandExecution(commandType, sql, preExecutionContext.Parameters);
        
        var result = await Connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
            sql,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnStatementExecuted(new DbStatementPostExecutionContext<T?>(
            preExecutionContext.CommandText,
            preExecutionContext.CommandType,
            preExecutionContext.Parameters,
            result
        ));
    }

    protected virtual async Task<T> GetSingleAsync<T>(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var preExecutionContext = new DbStatementPreExecutionContext(
            sql,
            commandType,
            param is not null ? ToDynamicParameters(param as object) : new DynamicParameters()
        );
        OnExecutingStatement(preExecutionContext);
        
        LogCommandExecution(commandType, sql, preExecutionContext.Parameters);
        
        var result = await Connection.QuerySingleAsync<T>(new CommandDefinition(
            sql,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnStatementExecuted(new DbStatementPostExecutionContext<T>(
            preExecutionContext.CommandText,
            preExecutionContext.CommandType,
            preExecutionContext.Parameters,
            result
        ));
    }

    protected virtual async Task<T?> GetSingleOrDefaultAsync<T>(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var preExecutionContext = new DbStatementPreExecutionContext(
            sql,
            commandType,
            param is not null ? ToDynamicParameters(param as object) : new DynamicParameters()
        );
        OnExecutingStatement(preExecutionContext);
        
        LogCommandExecution(commandType, sql, preExecutionContext.Parameters);
        
        var result = await Connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(
            sql,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnStatementExecuted(new DbStatementPostExecutionContext<T?>(
            preExecutionContext.CommandText,
            preExecutionContext.CommandType,
            preExecutionContext.Parameters,
            result
        ));
    }

    protected virtual async Task<SqlMapper.GridReader> GetMultipleAsync(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var preExecutionContext = new DbStatementPreExecutionContext(
            sql,
            commandType,
            param is not null ? ToDynamicParameters(param as object) : new DynamicParameters()
        );
        OnExecutingStatement(preExecutionContext);
        
        LogCommandExecution(commandType, sql, preExecutionContext.Parameters);
        
        var result = await Connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnStatementExecuted(new DbStatementPostExecutionContext<SqlMapper.GridReader>(
            preExecutionContext.CommandText,
            preExecutionContext.CommandType,
            preExecutionContext.Parameters,
            result
        ));
    }
}