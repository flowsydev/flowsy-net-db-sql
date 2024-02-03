using System.Data;
using Dapper;

namespace Flowsy.Db.Sql;

public abstract partial class DbRepository
{
    protected virtual Task<int> ExecuteStatementAsync(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => Connection.ExecuteAsync(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters(param as object) : null,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
    
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => Connection.QueryAsync<T>(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters((object) param) : null,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));

    protected virtual Task<T> GetFirstAsync<T>(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => Connection.QueryFirstAsync<T>(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters((object) param) : null,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));

    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => Connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters((object) param) : null,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
    
    protected virtual Task<T> GetSingleAsync<T>(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => Connection.QuerySingleAsync<T>(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters((object) param) : null,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
    
    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => Connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters((object) param) : null,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
    
    protected virtual Task<SqlMapper.GridReader> GetMultipleAsync(
        string sql,
        CommandType commandType,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => Connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            param is not null ? ToDynamicParameters((object) param) : null,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
}