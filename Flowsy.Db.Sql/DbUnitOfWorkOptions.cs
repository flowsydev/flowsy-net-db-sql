using Flowsy.Db.Sql.Resources;

namespace Flowsy.Db.Sql;

public sealed class DbUnitOfWorkOptions
{
    public string? ConnectionKey { get; set; }
    
    private static readonly IDictionary<Type, DbUnitOfWorkOptions> OptionsMap 
        = new Dictionary<Type, DbUnitOfWorkOptions>();

    public static void Register<T>(DbUnitOfWorkOptions options) where T : DbUnitOfWork
        => Register(typeof(T), options);
    
    public static void Register(Type unitOfWorkType, DbUnitOfWorkOptions options)
        => OptionsMap[unitOfWorkType] = options;

    public static DbUnitOfWorkOptions Resolve<T>() where T : DbUnitOfWork
        => Resolve(typeof(T));

    public static DbUnitOfWorkOptions Resolve(Type unitOfWorkType)
    {
        if (OptionsMap.TryGetValue(unitOfWorkType, out var options))
            return options;

        throw new InvalidOperationException(string.Format(Strings.NoOptionsRegisteredForX, unitOfWorkType.Name));
    }

    public DbUnitOfWorkOptions Clone() => new ()
    {
        ConnectionKey = ConnectionKey
    };
}