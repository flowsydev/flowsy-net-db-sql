using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Db.Sql;

public static class DependencyInjection
{
    public static IServiceCollection AddDbConnectionFactory(
        this IServiceCollection services,
        params DbConnectionConfiguration[] connectionConfigurations
        )
    {
        services.AddSingleton(new DbConnectionFactory(connectionConfigurations));
        return services;
    }
    
    /// <summary>
    /// Registers a database connection factory.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    public static IServiceCollection AddDbConnectionFactory(
        this IServiceCollection services,
        Func<IServiceProvider, DbConnectionFactory> implementationFactory
        )
    {
        services.AddSingleton(implementationFactory);
        return services;
    }
    
    /// <summary>
    /// Registers and configures unit of work services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureDefaults">A function to configure default options for all unit of work types.</param>
    public static DbUnitOfWorkBuilder AddUnitOfWork(
        this IServiceCollection services,
        Action<DbUnitOfWorkOptions>? configureDefaults
    )
    {
        var defaults = new DbUnitOfWorkOptions();
        configureDefaults?.Invoke(defaults);
        return new DbUnitOfWorkBuilder(services, defaults);
    }
}