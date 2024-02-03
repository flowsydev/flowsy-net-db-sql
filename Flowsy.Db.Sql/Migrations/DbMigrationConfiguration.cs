namespace Flowsy.Db.Sql.Migrations;

/// <summary>
/// Represents a set of configurations for running database migrations.
/// </summary>
public sealed class DbMigrationConfiguration
{
    public DbMigrationConfiguration()
        : this(string.Empty, null, string.Empty, null)
    {
    }

    public DbMigrationConfiguration(string sourceDirectory, string? metadataTableSchema, string metadataTableName, string? initializationStatement)
    {
        SourceDirectory = sourceDirectory;
        MetadataTableSchema = metadataTableSchema;
        MetadataTableName = metadataTableName;
        InitializationStatement = initializationStatement;
    }

    /// <summary>
    /// The directory containing migration scripts.
    /// </summary>
    public string SourceDirectory { get; set; }
    
    /// <summary>
    /// The database schema containing the table to store migration metadata. 
    /// </summary>
    public string? MetadataTableSchema { get; set; }
    
    /// <summary>
    /// The name of the table to store migration metadata.
    /// </summary>
    public string MetadataTableName { get; set; }
    
    /// <summary>
    /// An optional statement to be executed after running migrations.
    /// </summary>
    public string? InitializationStatement { get; set; }
}