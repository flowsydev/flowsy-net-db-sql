using System.Data;
using System.Data.Common;
using EvolveDb;
using Flowsy.Db.Abstractions;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Sql;

public sealed class DbManager
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<DbManager>? _logger;
    

    public DbManager(IDbConnectionFactory dbConnectionFactory, ILogger<DbManager>? logger = null)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }

    /// <summary>
    /// Runs database migrations for all the connections associated with the underlying IDbConnectionFactory instance.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    public Task MigrateAsync(CancellationToken cancellationToken)
        => MigrateAsync(Array.Empty<string>(), cancellationToken);

    /// <summary>
    /// Runs database migrations for the specified connection keys.
    /// If no connection key is provided, it will run migrations for all connections associated with the underlying IDbConnectionFactory instance.
    /// </summary>
    /// <param name="connectionKeys">A list of connection keys.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    public Task<IEnumerable<DbMigrationResult>> MigrateAsync(IEnumerable<string> connectionKeys, CancellationToken cancellationToken)
        => Task.Run<IEnumerable<DbMigrationResult>>(() =>
        {
            var keys = connectionKeys.ToArray();
            var configurations = _dbConnectionFactory
                .Configurations
                .Where(c => !keys.Any() || keys.Contains(c.Key))
                .ToArray();

            if (!configurations.Any())
                throw new InvalidOperationException("No database configuration was found.");
            
            var results = new List<DbMigrationResult>();
            
            foreach (var configuration in configurations)
            {
                try
                {
                    if (configuration.Migration is null)
                    {
                        _logger?.LogError(
                            "No migration configuration was provided for connection with key {ConnectionKey}",
                            configuration.Key
                        );
                        continue;
                    }

                    using var connection = _dbConnectionFactory.GetConnection(configuration.Key);
                    if (connection is not DbConnection dbConnection)
                    {
                        _logger?.LogError(
                            "Connection with key {ConnectionKey} is not of type DbConnection",
                            configuration.Key
                        );
                        continue;
                    }

                    connection.Open();

                    var evolve = new Evolve(
                        dbConnection,
                        message => _logger?.LogInformation("Database migration: {Message}", message)
                    )
                    {
                        Locations = new[]
                        {
                            configuration.Migration.SourceDirectory
                        },
                        MetadataTableSchema = configuration.Migration.MetadataSchema ?? string.Empty,
                        MetadataTableName = configuration.Migration.MetadataTable
                    };

                    evolve.Migrate();

                    if (configuration.Migration.InitializationStatement is not null)
                    {
                        using var command = connection.CreateCommand();
                        command.CommandType = CommandType.Text;
                        command.CommandText = configuration.Migration.InitializationStatement;
                        command.ExecuteNonQuery();
                    }
                    
                    results.Add(new DbMigrationResult(configuration));
                }
                catch (Exception exception)
                {
                    _logger?.LogError(exception, "Database migration failed for connection with key {ConnectionKey}", configuration.Key);
                    results.Add(new DbMigrationResult(configuration, exception));
                }
            }

            return results;
        }, cancellationToken);
}