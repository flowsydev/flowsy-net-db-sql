using System.Text.Json;
using Flowsy.Db.Sql.Conventions;
using Flowsy.Db.Sql.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Sql;

public class DbRepositoryOptions
{
    public DbRepositoryOptions()
        : this(null, new DbConventionSet(), new JsonSerializerOptions(), LogLevel.Debug)
    {
    }

    public DbRepositoryOptions(string? schema, DbConventionSet conventions)
        : this(schema, conventions, new JsonSerializerOptions(), LogLevel.Debug)
    {
    }

    public DbRepositoryOptions(string? schema, DbConventionSet conventions, JsonSerializerOptions jsonSerialization, LogLevel logLevel)
    {
        Schema = schema;
        Conventions = conventions;
        JsonSerialization = jsonSerialization;
        LogLevel = logLevel;
    }

    public string? Schema { get; set; }
    public DbConventionSet Conventions { get; set; }
    public JsonSerializerOptions JsonSerialization { get; set; }
    public LogLevel LogLevel { get; set; }

    private static readonly IDictionary<Type, DbRepositoryOptions> OptionsMap 
        = new Dictionary<Type, DbRepositoryOptions>();

    public static void Register<T>(DbRepositoryOptions options) where T : DbRepository
        => Register(typeof(T), options);
    
    public static void Register(Type repositoryType, DbRepositoryOptions options)
        => OptionsMap[repositoryType] = options;

    public static DbRepositoryOptions Resolve<T>() where T : DbRepository
        => Resolve(typeof(T));

    public static DbRepositoryOptions Resolve(Type repositoryType)
    {
        if (OptionsMap.TryGetValue(repositoryType, out var options))
            return options;

        throw new InvalidOperationException(string.Format(Strings.NoOptionsRegisteredForX, repositoryType.Name));
    }

    public DbRepositoryOptions Clone()
        => new(Schema, Conventions.Clone(), JsonSerialization, LogLevel);
}