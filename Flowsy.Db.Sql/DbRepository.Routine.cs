using Dapper;

namespace Flowsy.Db.Sql;

public abstract partial class DbRepository
{
    protected virtual Task<T> ExecuteRoutineReturningSingleAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetSingleAsync<T>(routineSimpleName, param as object, cancellationToken);
   
    protected virtual Task<IEnumerable<T>> ExecuteRoutineReturningManyAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetManyAsync<T>(routineSimpleName, param as object, cancellationToken);
    
    protected virtual Task<int> ExecuteRoutineAsync(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => ExecuteRoutineAsync(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);

    protected virtual async Task<int> ExecuteRoutineAsync(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : new DynamicParameters();
        var routineFullName = ResolveRoutineName(routineSimpleName);
        var preExecutionContext = new DbRoutinePreExecutionContext(
            routineSimpleName,
            routineFullName,
            routineType ?? Options.Conventions.Routines.DefaultType,
            dynamicParameters
        ); 
        OnExecutingRoutine(preExecutionContext);
        
        var commandText = ResolveRoutineStatement(routineSimpleName, preExecutionContext.Parameters, preExecutionContext.RoutineType);
        var commandType = ResolveRoutineCommandType(preExecutionContext.RoutineType);
        
        LogCommandExecution(commandType, commandText, preExecutionContext.Parameters);
        
        var result = await Connection.ExecuteAsync(new CommandDefinition(
            commandText,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnRoutineExecuted(new DbRoutinePostExecutionContext<int>(
            preExecutionContext.RoutineSimpleName,
            preExecutionContext.RoutineFullName,
            preExecutionContext.RoutineType,
            preExecutionContext.Parameters,
            result
            ));
    }
    
    protected virtual Task<IEnumerable<T>> GetManyAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetManyAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);
    
    protected virtual async Task<IEnumerable<T>> GetManyAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : new DynamicParameters();
        var routineFullName = ResolveRoutineName(routineSimpleName);
        var preExecutionContext = new DbRoutinePreExecutionContext(
            routineSimpleName,
            routineFullName,
            routineType ?? Options.Conventions.Routines.DefaultType,
            dynamicParameters
        ); 
        OnExecutingRoutine(preExecutionContext);
        
        var commandText = ResolveRoutineStatement(routineSimpleName, preExecutionContext.Parameters, preExecutionContext.RoutineType);
        var commandType = ResolveRoutineCommandType(preExecutionContext.RoutineType);
        
        LogCommandExecution(commandType, commandText, preExecutionContext.Parameters);
        
        var result = await Connection.QueryAsync<T>(new CommandDefinition(
            commandText,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnRoutineExecuted(new DbRoutinePostExecutionContext<IEnumerable<T>>(
            preExecutionContext.RoutineSimpleName,
            preExecutionContext.RoutineFullName,
            preExecutionContext.RoutineType,
            preExecutionContext.Parameters,
            result
        ));
    }

    protected virtual Task<T> GetFirstAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetFirstAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);
    
    protected virtual async Task<T> GetFirstAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : new DynamicParameters();
        var routineFullName = ResolveRoutineName(routineSimpleName);
        var preExecutionContext = new DbRoutinePreExecutionContext(
            routineSimpleName,
            routineFullName,
            routineType ?? Options.Conventions.Routines.DefaultType,
            dynamicParameters
        ); 
        OnExecutingRoutine(preExecutionContext);
        
        var commandText = ResolveRoutineStatement(routineSimpleName, preExecutionContext.Parameters, preExecutionContext.RoutineType);
        var commandType = ResolveRoutineCommandType(preExecutionContext.RoutineType);
        
        LogCommandExecution(commandType, commandText, preExecutionContext.Parameters);
        
        var result = await Connection.QueryFirstAsync<T>(new CommandDefinition(
            ResolveRoutineStatement(routineSimpleName, preExecutionContext.Parameters, preExecutionContext.RoutineType),
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnRoutineExecuted(new DbRoutinePostExecutionContext<T>(
            preExecutionContext.RoutineSimpleName,
            preExecutionContext.RoutineFullName,
            preExecutionContext.RoutineType,
            preExecutionContext.Parameters,
            result
        ));
    }

    protected virtual Task<T?> GetFirstOrDefaultAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetFirstOrDefaultAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);

    protected virtual async Task<T?> GetFirstOrDefaultAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : new DynamicParameters();
        var routineFullName = ResolveRoutineName(routineSimpleName);
        var preExecutionContext = new DbRoutinePreExecutionContext(
            routineSimpleName,
            routineFullName,
            routineType ?? Options.Conventions.Routines.DefaultType,
            dynamicParameters
        );
        OnExecutingRoutine(preExecutionContext);
        
        var commandText = ResolveRoutineStatement(routineSimpleName, preExecutionContext.Parameters, preExecutionContext.RoutineType);
        var commandType = ResolveRoutineCommandType(preExecutionContext.RoutineType);
        
        LogCommandExecution(commandType, commandText, preExecutionContext.Parameters);
        
        var result = await Connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
            commandText,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnRoutineExecuted(new DbRoutinePostExecutionContext<T?>(
            preExecutionContext.RoutineSimpleName,
            preExecutionContext.RoutineFullName,
            preExecutionContext.RoutineType,
            preExecutionContext.Parameters,
            result
        ));
    }

    protected virtual Task<T> GetSingleAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetSingleAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);

    protected virtual async Task<T> GetSingleAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : new DynamicParameters();
        var routineFullName = ResolveRoutineName(routineSimpleName);
        var preExecutionContext = new DbRoutinePreExecutionContext(
            routineSimpleName,
            routineFullName,
            routineType ?? Options.Conventions.Routines.DefaultType,
            dynamicParameters
        );
        OnExecutingRoutine(preExecutionContext);
        
        var commandText = ResolveRoutineStatement(routineSimpleName, preExecutionContext.Parameters, preExecutionContext.RoutineType);
        var commandType = ResolveRoutineCommandType(preExecutionContext.RoutineType);
        
        LogCommandExecution(commandType, commandText, preExecutionContext.Parameters);
        
        var result = await Connection.QuerySingleAsync<T>(new CommandDefinition(
            commandText,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnRoutineExecuted(new DbRoutinePostExecutionContext<T>(
            preExecutionContext.RoutineSimpleName,
            preExecutionContext.RoutineFullName,
            preExecutionContext.RoutineType,
            preExecutionContext.Parameters,
            result
        ));
    }

    protected virtual Task<T?> GetSingleOrDefaultAsync<T>(
        string routineSimpleName,
        dynamic? param,
        CancellationToken cancellationToken
    )
        => GetSingleOrDefaultAsync<T>(routineSimpleName, ResolveRoutineType(), param as object, cancellationToken);
    
    protected virtual async Task<T?> GetSingleOrDefaultAsync<T>(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : new DynamicParameters();
        var routineFullName = ResolveRoutineName(routineSimpleName);
        var preExecutionContext = new DbRoutinePreExecutionContext(
            routineSimpleName,
            routineFullName,
            routineType ?? Options.Conventions.Routines.DefaultType,
            dynamicParameters
        );
        OnExecutingRoutine(preExecutionContext);
        
        var commandText = ResolveRoutineStatement(routineSimpleName, preExecutionContext.Parameters, preExecutionContext.RoutineType);
        var commandType = ResolveRoutineCommandType(preExecutionContext.RoutineType);
        
        LogCommandExecution(commandType, commandText, preExecutionContext.Parameters);
        
        var result = await Connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(
            commandText,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnRoutineExecuted(new DbRoutinePostExecutionContext<T?>(
            preExecutionContext.RoutineSimpleName,
            preExecutionContext.RoutineFullName,
            preExecutionContext.RoutineType,
            preExecutionContext.Parameters,
            result
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

    protected virtual async Task<SqlMapper.GridReader> GetMultipleAsync(
        string routineSimpleName,
        DbRoutineType? routineType,
        dynamic? param,
        CancellationToken cancellationToken
    )
    {
        var dynamicParameters = param is not null ? ToDynamicParameters((object) param) : new DynamicParameters();
        var routineFullName = ResolveRoutineName(routineSimpleName);
        var preExecutionContext = new DbRoutinePreExecutionContext(
            routineSimpleName,
            routineFullName,
            routineType ?? Options.Conventions.Routines.DefaultType,
            dynamicParameters
        );
        OnExecutingRoutine(preExecutionContext);
        
        var commandText = ResolveRoutineStatement(routineSimpleName, preExecutionContext.Parameters, preExecutionContext.RoutineType);
        var commandType = ResolveRoutineCommandType(preExecutionContext.RoutineType);
        
        LogCommandExecution(commandType, commandText, preExecutionContext.Parameters);
        
        var result = await Connection.QueryMultipleAsync(new CommandDefinition(
            commandText,
            preExecutionContext.Parameters,
            Transaction,
            commandType: commandType,
            cancellationToken: cancellationToken
        ));
        
        return OnRoutineExecuted(new DbRoutinePostExecutionContext<SqlMapper.GridReader>(
            preExecutionContext.RoutineSimpleName,
            preExecutionContext.RoutineFullName,
            preExecutionContext.RoutineType,
            preExecutionContext.Parameters,
            result
        ));
    }
}