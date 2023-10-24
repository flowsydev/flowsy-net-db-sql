using System.Data;
using System.Data.Common;
using Flowsy.Db.Abstractions;

namespace Flowsy.Db.Sql;

/// <summary>
/// Obtains database connections from a list of DbConnectionConfiguration objects.
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly Dictionary<string, DbConnectionConfiguration> _connectionConfigurations;
    
    public DbConnectionFactory(params DbConnectionConfiguration[] connectionConfigurations)
    {
        if (connectionConfigurations.Length == 0)
            throw new ArgumentException(Resources.Strings.NoConnectionConfigurationProvided, nameof(connectionConfigurations));
        
        _connectionConfigurations = new Dictionary<string, DbConnectionConfiguration>();
        foreach (var c in connectionConfigurations)
            _connectionConfigurations[c.Key] = c;
    }

    /// <summary>
    /// A list of keys known by the connection factory.
    /// Each key identifies a database connection that can be obtained by the connection factory.
    /// </summary>
    public IEnumerable<string> ConnectionKeys => _connectionConfigurations.Keys;
    
    /// <summary>
    /// Obtains a database connection using the DbConnectionConfiguration identified by the provided key.
    /// </summary>
    /// <param name="key">The connection identifier. If omitted, the first DbConnectionConfiguration in the list will be used.</param>
    /// <returns>A database connection</returns>
    public IDbConnection GetConnection(string? key = null)
    {
        var k = key ?? _connectionConfigurations.Keys.First();
        if (!_connectionConfigurations.ContainsKey(k))
            throw new InvalidOperationException(string.Format(Resources.Strings.InvalidConnectionKey, k));

        var connectionConfiguration = _connectionConfigurations[k];
     
        var connection = DbProviderFactories
            .GetFactory(connectionConfiguration.ProviderInvariantName)
            .CreateConnection() ?? throw new InvalidOperationException();
        
        connection.ConnectionString = connectionConfiguration.ConnectionString;
        
        return connection;
    }
}