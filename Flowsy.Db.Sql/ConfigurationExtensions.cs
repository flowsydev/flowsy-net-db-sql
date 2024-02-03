using Flowsy.Db.Sql.Migrations;
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
                var metadataTableSchema = migrationSection["MetadataTableSchema"];
                var metadataTableName = migrationSection["MetadataTableName"];
                var initializationStatement = migrationSection["InitializationStatement"];
                
                var migration = sourceDirectory is not null && metadataTableName is not null
                    ? new DbMigrationConfiguration(sourceDirectory, metadataTableSchema, metadataTableName, initializationStatement)
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