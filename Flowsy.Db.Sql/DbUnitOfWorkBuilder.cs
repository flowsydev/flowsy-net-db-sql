using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Db.Sql;

public sealed class DbUnitOfWorkBuilder
{
    private readonly IServiceCollection _services;
    private readonly DbUnitOfWorkOptions _defaultOptions;

    public DbUnitOfWorkBuilder(IServiceCollection services, DbUnitOfWorkOptions defaultOptions)
    {
        _services = services;
        _defaultOptions = defaultOptions;
    }

    public DbUnitOfWorkBuilder Using<TService, TImplementation>(Action<DbUnitOfWorkOptions>? configure = null)
        => Using(typeof(TService), typeof(TImplementation), configure);

    public DbUnitOfWorkBuilder Using<TService>(
        Func<IServiceProvider, TService> implementationFactory,
        Action<DbUnitOfWorkOptions>? configure = null
    )
        where TService : DbUnitOfWork
    {
        var options = _defaultOptions.Clone();
        configure?.Invoke(options);
        DbUnitOfWorkOptions.Register<TService>(options);
        _services.AddTransient(implementationFactory);
        return this;
    }

    public DbUnitOfWorkBuilder Using(Type serviceType, Type implementationType, Action<DbUnitOfWorkOptions>? configure = null)
    {
        var options = _defaultOptions.Clone();
        configure?.Invoke(options);
        DbUnitOfWorkOptions.Register(implementationType, options);
        _services.AddTransient(serviceType, implementationType);
        return this;
    }
}