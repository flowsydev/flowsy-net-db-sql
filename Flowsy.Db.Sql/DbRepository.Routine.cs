using Dapper;

namespace Flowsy.Db.Sql;

public abstract partial class DbRepository
{
    protected virtual Task<int> ExecuteRoutineAsync(
        string simpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => ExecuteRoutineAsync(simpleName, ResolveRoutineType(), param as object, cancellationToken);

    protected virtual Task<int> ExecuteRoutineAsync(
        string simpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return Connection.ExecuteAsync(new CommandDefinition(
            ResolveRoutineStatement(simpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
        ));
    }
    
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetManyAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);
    
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return Connection.QueryAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
        ));
    }

    protected virtual Task<T> GetFirstAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetFirstAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);
    
    protected virtual Task<T> GetFirstAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return Connection.QueryFirstAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
        ));
    }

    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetFirstOrDefaultAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);

    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return Connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
        ));
    }

    protected virtual Task<T> GetSingleAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetSingleAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);

    protected virtual Task<T> GetSingleAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return Connection.QuerySingleAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
        ));
    }

    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetSingleOrDefaultAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);
    
    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return Connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
        ));
    }

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

    protected virtual Task<SqlMapper.GridReader> GetMultipleAsync(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : null;
        return Connection.QueryMultipleAsync(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, dynamicParameters, routineType),
            dynamicParameters,
            Transaction,
            commandType: ResolveRoutineCommandType(routineType),
            cancellationToken: cancellationToken
        ));
    }
}