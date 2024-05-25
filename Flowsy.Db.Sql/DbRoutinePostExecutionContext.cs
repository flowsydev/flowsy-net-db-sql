using Dapper;

namespace Flowsy.Db.Sql;

public sealed class DbRoutinePostExecutionContext<TResult>
{
    public DbRoutinePostExecutionContext(string routineSimpleName, string routineFullName, DbRoutineType routineType, DynamicParameters parameters, TResult result)
    {
        RoutineSimpleName = routineSimpleName;
        RoutineFullName = routineFullName;
        RoutineType = routineType;
        Parameters = parameters;
        Result = result;
    }

    public string RoutineSimpleName { get; }
    public string RoutineFullName { get; }
    public DbRoutineType RoutineType { get; }
    public DynamicParameters Parameters { get; }
    public TResult Result { get; }
}