using System.Data;
using Dapper;

namespace Flowsy.Db.Sql;

public sealed class DbStatementPostExecutionContext<TResult>
{
    public DbStatementPostExecutionContext(string commandText, CommandType commandType, DynamicParameters parameters, TResult result)
    {
        CommandText = commandText;
        CommandType = commandType;
        Parameters = parameters;
        Result = result;
    }

    public string CommandText { get; }
    public CommandType CommandType { get; }
    public DynamicParameters Parameters { get; }
    public TResult Result { get; }
}