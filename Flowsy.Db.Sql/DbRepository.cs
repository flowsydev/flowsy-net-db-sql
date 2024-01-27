using System.Data;
using System.Data.Common;
using Dapper;
using Flowsy.Core;
using Flowsy.Db.Abstractions;

namespace Flowsy.Db.Sql;

/// <summary>
/// Provides basic functionality to implement entity repositories with an SQL database as their underlying data store.
/// </summary>
public abstract partial class DbRepository
{
    protected DbRepository(
        IDbConnectionFactory connectionFactory,
        DbExceptionHandler? exceptionHandler = null
        )
    {
        ConnectionFactory = connectionFactory;
        ExceptionHandler = exceptionHandler;
    }
    
    protected DbRepository(IDbConnection connection, DbExceptionHandler? exceptionHandler = null)
    {
        Connection = connection;
        ExceptionHandler = exceptionHandler;
    }
    
    protected DbRepository(IDbTransaction transaction, DbExceptionHandler? exceptionHandler = null)
    {
        Transaction = transaction;
        ExceptionHandler = exceptionHandler;
    }

    /// <summary>
    /// A factory to get database connections from.
    /// </summary>
    protected IDbConnectionFactory? ConnectionFactory { get; }
    
    /// <summary>
    /// A database connection to execute queries.
    /// </summary>
    protected IDbConnection? Connection { get; }
    
    /// <summary>
    /// A database transaction to execute queries.
    /// </summary>
    protected IDbTransaction? Transaction { get; }
    
    /// <summary>
    /// A service to handle exceptions and possibly translate them to domain layer exceptions.
    /// </summary>
    protected DbExceptionHandler? ExceptionHandler { get; }

    /// <summary>
    /// A set of options to customize the behavior of the repository.
    /// </summary>
    protected DbRepositoryOptions Options => DbRepositoryOptions.Resolve(GetType());

    #region Connection Management 
    /// <summary>
    /// Gets the connection associated with the Transaction property.
    /// If there is no transaction available, this method gets the value of the Connection property,
    /// otherwise it gets a new connection from the object hold by the ConnectionFactory property. 
    /// </summary>
    /// <param name="key">An optional string value corresponding to the key of a connection configuration of the IDbConnectionFactory.</param>
    /// <returns>A database connection.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected IDbConnection GetConnection(string? key = null)
        => Transaction?.Connection ??
           Connection ??
           ConnectionFactory?.GetConnection(key ?? Options.ConnectionKey) ??
           throw new InvalidOperationException(Resources.Strings.CouldNotGetConnection);

    /// <summary>
    /// Disposes a database connection only if it's not equal to the value of
    /// the Connection property nor the connection from the Transaction property.
    /// </summary>
    /// <param name="connection">The connection to dispose.</param>
    protected void TryDisposeConnection(IDbConnection connection)
    {
        if (connection == Connection || connection == Transaction?.Connection)
            return;

        connection.Dispose();
    }

    /// <summary>
    /// Asynchronously disposes a database connection only if it's not equal to the value of
    /// the Connection property nor the connection from the Transaction property.
    /// </summary>
    /// <param name="connection">The connection to dispose.</param>
    protected async Task TryDisposeConnectionAsync(IDbConnection connection)
    {
        if (connection == Connection || connection == Transaction?.Connection)
            return;
        
        if (connection is DbConnection c)
            await c.DisposeAsync();
        else
            connection.Dispose();
    }
    #endregion

    #region Parameter Resolution 
    /// <summary>
    /// Creates a DynamicParameters instance from the properties of the given object.
    /// </summary>
    /// <param name="obj">The object to read the properties from.</param>
    /// <returns>An instance of DynamicParameters.</returns>
    protected virtual DynamicParameters ToDynamicParameters(object obj)
        => ToDynamicParameters(obj.ToReadonlyDictionary());
    
    /// <summary>
    /// Creates a DynamicParameters instance from the values of the given dictionary.
    /// </summary>
    /// <param name="properties">The property names and values of an object.</param>
    /// <returns>An instance of DynamicParameters.</returns>
    protected virtual DynamicParameters ToDynamicParameters(IReadOnlyDictionary<string, object?> properties)
    {
        var parameters = new DynamicParameters();

        foreach (var (key, value) in properties)
        {
            var parameter = BuildParameter(key, value);
            parameters.Add(parameter.Name, parameter.Value, parameter.Type, parameter.Direction, parameter.Size);
        }
        
        return parameters;
    }
    
    /// <summary>
    /// Builds a database parameter from a property name and value read from an object.
    /// </summary>
    /// <param name="sourcePropertyName">The name of property.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>An instance of DbParameterInfo.</returns>
    protected virtual DbParameterInfo BuildParameter(string sourcePropertyName, object? value)
    {
        var parameterName = Options.Conventions.Parameters.Format(sourcePropertyName);
        
        if (value is null)
            return new DbParameterInfo(parameterName, null, null, null, null);

        return value switch
        {
            DateTime => new DbParameterInfo(parameterName, DbType.DateTime2, null, null, value),
            DateTimeOffset dateTimeOffset => new DbParameterInfo(parameterName, DbType.DateTimeOffset, null, null, Options.Conventions.DateTimeOffsetFormat == DbDateTimeOffsetFormat.Utc ? dateTimeOffset.UtcDateTime : dateTimeOffset.LocalDateTime),
            Enum e => new DbParameterInfo(parameterName, ResolveEnumType(e), null, null, ResolveEnumValue(e)),
            IEnumerable<sbyte> enumerable => new DbParameterInfo(parameterName, null, null, null, enumerable.ToArray()),
            IEnumerable<byte> enumerable => new DbParameterInfo(parameterName, null, null, null, enumerable.ToArray()),
            IEnumerable<short> enumerable => new DbParameterInfo(parameterName, null, null, null, enumerable.ToArray()),
            IEnumerable<int> enumerable => new DbParameterInfo(parameterName, null, null, null, enumerable.ToArray()),
            IEnumerable<long> enumerable => new DbParameterInfo(parameterName, null, null, null, enumerable.ToArray()),
            IEnumerable<float> enumerable => new DbParameterInfo(parameterName, null, null, null, enumerable.ToArray()),
            IEnumerable<double> enumerable => new DbParameterInfo(parameterName, null, null, null, enumerable.ToArray()),
            IEnumerable<decimal> enumerable => new DbParameterInfo(parameterName, null, null, null, enumerable.ToArray()),
            IEnumerable<bool> enumerable => new DbParameterInfo(parameterName, null, null, null, enumerable.ToArray()),
            IEnumerable<string> enumerable => new DbParameterInfo(parameterName, null, null, null, enumerable.ToArray()),
            _ => new DbParameterInfo(parameterName, null, null, null, value)
        };
    }
    #endregion

    #region Database Action

    /// <summary>
    /// Executes an action on a database connection resolved from the properties of this respository.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="action">The action to execute.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The result of the action.</returns>
    protected virtual async Task<T> ExecuteAsync<T>(IDbConnection connection, Func<IDbConnection, Task<T>> action)
    {
        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            
            return await action(connection);
        }
        catch (Exception exception)
        {
            if (ExceptionHandler is null)
                throw;
            
            var newException = ExceptionHandler?.Handle(exception);
            if (newException is not null)
                throw newException;

            throw;
        }
    }
    
    /// <summary>
    /// Executes an action on a database connection resolved from the properties of this respository.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <typeparam name="T">The type of the expected result.</typeparam>
    /// <returns>The result of the action.</returns>
    protected virtual async Task<T> ExecuteAsync<T>(Func<IDbConnection, Task<T>> action)
    {
        var connection = GetConnection();
        try
        {
            return await ExecuteAsync(connection, action);
        }
        finally
        {
            await TryDisposeConnectionAsync(connection);
        }
    }

    /// <summary>
    /// Executes an action on a database connection resolved from the properties of this respository.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The result of the action.</returns>
    protected virtual async Task ExecuteAsync(IDbConnection connection, Func<IDbConnection, Task> action)
    {
        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            
            await action(connection);
        }
        catch (Exception exception)
        {
            if (ExceptionHandler is null)
                throw;
            
            var newException = ExceptionHandler?.Handle(exception);
            if (newException is not null)
                throw newException;

            throw;
        }
    }
    
    /// <summary>
    /// Executes an action on a database connection resolved from the properties of this respository.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The result of the action.</returns>
    protected virtual async Task ExecuteAsync(Func<IDbConnection, Task> action)
    {
        var connection = GetConnection();
        try
        {
            await ExecuteAsync(connection, action);
        }
        finally
        {
            await TryDisposeConnectionAsync(connection);
        }
    }
    #endregion
}