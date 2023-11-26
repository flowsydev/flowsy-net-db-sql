using Flowsy.Db.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Flowsy.Db.Sql;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Creates a list of DbConnectionConfiguration objects from a IConfiguration object.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns>A list of DbConnectionConfiguration objects.</returns>
    public static IEnumerable<DbConnectionConfiguration> GetConnectionConfigurations(
        this IConfiguration configuration,
        string? sectionName = null
        )
    {
        var section = !string.IsNullOrEmpty(sectionName) ? configuration.GetSection(sectionName) : configuration;
        return section
            .GetChildren()
            .Where(c => c["ProviderInvariantName"] is not null && c["ConnectionString"] is not null)
            .Select(c =>
            {
                var migrationSection = c.GetSection("Migration");
                var sourceDirectory = migrationSection["SourceDirectory"];
                var metadataSchema = migrationSection["MetadataSchema"];
                var metadataTable = migrationSection["MetadataTable"];
                var initializationStatement = migrationSection["InitializationStatement"];
                var migration = sourceDirectory is not null && metadataTable is not null
                    ? new DbMigrationConfiguration(sourceDirectory, metadataSchema, metadataTable, initializationStatement)
                    : null;

                return new DbConnectionConfiguration
                {
                    Key = c.Key,
                    ProviderInvariantName = c["ProviderInvariantName"]!,
                    ConnectionString = c["ConnectionString"]!,
                    Migration = migration
                };
            });
    }
}