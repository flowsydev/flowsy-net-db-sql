using Dapper;

namespace Flowsy.Db.Sql;

public sealed class DbRoutinePreExecutionContext
{
    public DbRoutinePreExecutionContext(string routineSimpleName, string routineFullName, DbRoutineType routineType, DynamicParameters parameters)
    {
        RoutineSimpleName = routineSimpleName;
        RoutineFullName = routineFullName;
        RoutineType = routineType;
        Parameters = parameters;
    }

    public string RoutineSimpleName { get; }
    public string RoutineFullName { get; }
    public DbRoutineType RoutineType { get; set; }
    public DynamicParameters Parameters { get; }
}