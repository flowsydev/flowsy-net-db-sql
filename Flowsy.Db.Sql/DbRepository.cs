using System.Data;
using Dapper;
using Flowsy.Core;

namespace Flowsy.Db.Sql;

/// <summary>
/// Provides basic functionality to implement entity repositories with an SQL database as their underlying data store.
/// </summary>
public abstract partial class DbRepository
{
    private readonly DbUnitOfWork _unitOfWork;
    
    protected DbRepository(DbUnitOfWork unitOfWork, DbExceptionHandler? exceptionHandler = null)
    {
        _unitOfWork = unitOfWork;
        ExceptionHandler = exceptionHandler;
    }
    
    /// <summary>
    /// A set of options to customize the behavior of the repository.
    /// </summary>
    protected DbRepositoryOptions Options => DbRepositoryOptions.Resolve(GetType());
    
    /// <summary>
    /// A service to handle exceptions and possibly translate them to domain layer exceptions.
    /// </summary>
    protected DbExceptionHandler? ExceptionHandler { get; }

    /// <summary>
    /// A database connection to execute queries.
    /// </summary>
    protected IDbConnection Connection => _unitOfWork.Connection;

    /// <summary>
    /// A database transaction to execute queries.
    /// </summary>
    protected IDbTransaction? Transaction => _unitOfWork.Transaction;

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
}