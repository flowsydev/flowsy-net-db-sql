using Flowsy.Db.Sql.Conventions;

namespace Flowsy.Db.Sql;

public class DbRepositoryOptions
{
    public DbRepositoryOptions()
        : this(null, new DbConventionSet())
    {
    }

    public DbRepositoryOptions(string? schema, DbConventionSet conventions)
    {
        Schema = schema;
        Conventions = conventions;
    }

    public string? Schema { get; set; }
    public DbConventionSet Conventions { get; set; }

    private static readonly IDictionary<Type, DbRepositoryOptions> OptionsMap 
        = new Dictionary<Type, DbRepositoryOptions>();

    public static void Register<T>(DbRepositoryOptions options) where T : DbRepository
        => Register(typeof(T), options);
    
    public static void Register(Type repositoryType, DbRepositoryOptions options)
        => OptionsMap[repositoryType] = options;

    public static DbRepositoryOptions Resolve<T>() where T : DbRepository
        => Resolve(typeof(T));
    
    public static DbRepositoryOptions Resolve(Type repositoryType)
        => OptionsMap[repositoryType];

    public DbRepositoryOptions Clone()
        => new(Schema, Conventions.Clone());
}