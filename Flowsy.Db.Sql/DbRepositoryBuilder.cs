using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Db.Sql;

public class DbRepositoryBuilder
{
    private readonly IServiceCollection _services;
    private readonly DbRepositoryOptions _defaultOptions;

    internal DbRepositoryBuilder(IServiceCollection services, DbRepositoryOptions defaultOptions)
    {
        _services = services;
        _defaultOptions = defaultOptions;
    }

    public DbRepositoryBuilder Using<TService, TImplementation>(Action<DbRepositoryOptions>? configure = null) 
        where TImplementation : DbRepository, TService
        => Using(typeof(TImplementation), typeof(TImplementation), configure);

    public DbRepositoryBuilder Using(Type abstractionType, Type implementationType, Action<DbRepositoryOptions>? configure = null)
    {
        var options = _defaultOptions.Clone();
        configure?.Invoke(options);
        _services.AddTransient(abstractionType, implementationType);
        return this;
    }
    
    public DbRepositoryBuilder Using<TService>(Func<IServiceProvider, TService> implementationFactory, Action<DbRepositoryOptions>? configure = null) 
        where TService : class
    {
        var options = _defaultOptions.Clone();
        configure?.Invoke(options);
        _services.AddTransient(implementationFactory);
        return this;
    }
}