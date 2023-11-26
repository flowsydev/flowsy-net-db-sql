using Flowsy.Db.Abstractions;

namespace Flowsy.Db.Sql;

public sealed class DbMigrationResult
{
    internal DbMigrationResult(DbConnectionConfiguration connectionConfiguration, Exception? exception = null)
    {
        ConnectionConfiguration = connectionConfiguration;
        Exception = exception;
    }

    public DbConnectionConfiguration ConnectionConfiguration { get; }
    public bool IsSuccess => Exception is null;
    public bool IsFail => Exception is not null;
    public Exception? Exception { get; }
}