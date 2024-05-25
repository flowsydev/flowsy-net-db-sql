using System.Data;
using Dapper;

namespace Flowsy.Db.Sql;

public sealed class DbStatementPreExecutionContext
{
    public DbStatementPreExecutionContext(string commandText, CommandType commandType, DynamicParameters parameters)
    {
        CommandText = commandText;
        CommandType = commandType;
        Parameters = parameters;
    }

    public string CommandText { get; }
    public CommandType CommandType { get; }
    public DynamicParameters Parameters { get; }
}