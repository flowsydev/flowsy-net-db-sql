using System.Data;
using System.Data.Common;
using EvolveDb;
using Flowsy.Db.Sql.Migrations;
using Flowsy.Db.Sql.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Sql;

public sealed class DbManager
{
    private readonly DbConnectionFactory? _dbConnectionFactory;
    private readonly IEnumerable<DbConnectionConfiguration>? _connectionConfigurations;
    private readonly ILogger<DbManager>? _logger;
    private readonly Action<string>? _logAction;
    
    public DbManager(DbConnectionFactory dbConnectionFactory, ILogger<DbManager>? logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }
    
    public DbManager(DbConnectionFactory dbConnectionFactory, Action<string>? logAction)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _logAction = logAction;
    }

    public DbManager(IEnumerable<DbConnectionConfiguration>? connectionConfigurations, ILogger<DbManager>? logger)
    {
        _connectionConfigurations = connectionConfigurations;
        _logger = logger;
    }

    public DbManager(IEnumerable<DbConnectionConfiguration>? connectionConfigurations, Action<string>? logAction)
    {
        _connectionConfigurations = connectionConfigurations;
        _logAction = logAction;
    }

    private void LogInformation(string message)
    {
        if (_logger is not null)
            _logger.LogInformation(message);
        else if (_logAction is not null)
            _logAction.Invoke(message);
    }

    private void LogError(Exception exception, string message)
    {
        if (_logger is not null)
            _logger.LogError(exception, message);
        else if (_logAction is not null)
            _logAction.Invoke($"{message}{Environment.NewLine}{exception}");
    }

    private IEnumerable<DbConnectionConfiguration> GetConnectionConfigurations()
        => _dbConnectionFactory?.Configurations ??
           _connectionConfigurations ??
           throw new InvalidOperationException(Strings.ConnectionFactoryOrConfigurationListWasExpected); 

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
            var configurations = GetConnectionConfigurations()
                .Where(c => !keys.Any() || keys.Contains(c.Key))
                .ToArray();

            if (!configurations.Any())
                throw new InvalidOperationException(Strings.NoConnectionConfigurationProvided);
            
            var results = new List<DbMigrationResult>();
            
            foreach (var configuration in configurations)
            {
                try
                {
                    if (configuration.Migration is null)
                    {
                        throw new InvalidOperationException(
                            $"{Strings.NoMigrationConfigurationProvidedForConnectionWithKey} {configuration.Key}"
                            );
                    }

                    var factory = DbProviderFactories.GetFactory(configuration.ProviderInvariantName);
                    using var connection = factory.CreateConnection();
                    if (connection is null)
                    {
                        throw new InvalidOperationException(
                            $"{Strings.CouldNotCreateDatabaseConnectionForConfigurationWithKey} {configuration.Key}"
                            );
                    }

                    connection.ConnectionString = configuration.ConnectionString;
                    connection.Open();

                    var evolve = new Evolve(connection, LogInformation)
                    {
                        Locations = new[]
                        {
                            configuration.Migration.SourceDirectory
                        },
                        MetadataTableSchema = configuration.Migration.MetadataTableSchema,
                        MetadataTableName = configuration.Migration.MetadataTableName
                    };

                    evolve.Migrate();

                    if (configuration.Migration.InitializationStatement is not null)
                    {
                        using var command = connection.CreateCommand();
                        command.CommandType = CommandType.Text;
                        command.CommandText = configuration.Migration.InitializationStatement;
                        command.ExecuteNonQuery();
                        
                        LogInformation($"{Strings.InitializationStatementExecuted}: {configuration.Migration.InitializationStatement}");
                    }
                    
                    results.Add(new DbMigrationResult(configuration));
                }
                catch (Exception exception)
                {
                    LogError(exception, $"{Strings.DatabaseMigrationFailedForConnectionWithKey} {configuration.Key}");
                    results.Add(new DbMigrationResult(configuration, exception));
                }
            }

            return results;
        }, cancellationToken);
}