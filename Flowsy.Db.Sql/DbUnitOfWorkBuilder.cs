using Flowsy.Db.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Db.Sql;

public sealed class DbUnitOfWorkBuilder
{
    private readonly IServiceCollection _services;
    private readonly DbUnitOfWorkOptions _defaultOptions;

    internal DbUnitOfWorkBuilder(IServiceCollection services, DbUnitOfWorkOptions defaultOptions)
    {
        _services = services;
        _defaultOptions = defaultOptions;
    }
    
    /// <summary>
    /// Registers a new type of unit of work factory.
    /// </summary>
    /// <typeparam name="TService">The type of unit of work factory abstraction.</typeparam>
    /// <typeparam name="TImplementation">The type of unit of work factory implementation.</typeparam>
    public DbUnitOfWorkBuilder UsingFactory<TService, TImplementation>()
        where TService : class, IUnitOfWorkFactory
        where TImplementation : class, TService
    {
        _services.AddScoped<TService, TImplementation>();
        return this;
    }

    /// <summary>
    /// Registers a new type of unit of work factory.
    /// </summary>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <typeparam name="TService">The type of unit of work factory abstraction.</typeparam>
    /// <returns></returns>
    public DbUnitOfWorkBuilder UsingFactory<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class, IUnitOfWorkFactory
    {
        _services.AddScoped(implementationFactory);
        return this;
    }

    /// <summary>
    /// Registers options for a specific type of unit of work.
    /// </summary>
    /// <param name="configure">A function to configure options for the specific type of unit of work.</param>
    /// <typeparam name="TUnitOfWork">The type of unit of work.</typeparam>
    public DbUnitOfWorkBuilder WithUnit<TUnitOfWork>(Action<DbUnitOfWorkOptions>? configure = null)
        where TUnitOfWork : DbUnitOfWork
    {
        var options = _defaultOptions.Clone();
        configure?.Invoke(options);
        DbUnitOfWorkOptions.Register<TUnitOfWork>(options);
        return this;
    }

    /// <summary>
    /// Registers options for a specific type of unit of work.
    /// </summary>
    /// <param name="unitOfWorkType">The type of unit of work.</param>
    /// <param name="configure">A function to configure options for the specific type of unit of work.</param>
    public DbUnitOfWorkBuilder WithUnit(Type unitOfWorkType, Action<DbUnitOfWorkOptions>? configure = null)
    {
        var options = _defaultOptions.Clone();
        configure?.Invoke(options);
        DbUnitOfWorkOptions.Register(unitOfWorkType, options);
        return this;
    }
    
    /// <summary>
    /// Configures default options for all repositories.
    /// </summary>
    /// <param name="configureDefaults">A function to configure default options for repositories.</param>
    public DbRepositoryBuilder AddRepositories(Action<DbRepositoryOptions> configureDefaults)
    {
        var defaults = new DbRepositoryOptions();
        configureDefaults(defaults);
        return new DbRepositoryBuilder(defaults);
    }
}