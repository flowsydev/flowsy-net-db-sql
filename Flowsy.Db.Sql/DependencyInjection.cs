using Flowsy.Db.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Db.Sql;

public static class DependencyInjection
{
    public static IServiceCollection AddDbConnectionFactory(
        this IServiceCollection services,
        params DbConnectionConfiguration[] connectionConfigurations
        )
    {
        services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(connectionConfigurations));
        return services;
    }
    
    /// <summary>
    /// Registers a database connection factory.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    public static IServiceCollection AddDbConnectionFactory(
        this IServiceCollection services,
        Func<IServiceProvider, IDbConnectionFactory> implementationFactory
        )
    {
        services.AddSingleton(implementationFactory);
        return services;
    }

    /// <summary>
    /// Registers and configures repository services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureDefaults">Action to configure default options for all repository types.</param>
    public static DbRepositoryBuilder AddRepositories(
        this IServiceCollection services,
        Action<DbRepositoryOptions> configureDefaults
        )
    {
        var defaults = new DbRepositoryOptions();
        configureDefaults(defaults);
        return new DbRepositoryBuilder(services, defaults);
    }
}