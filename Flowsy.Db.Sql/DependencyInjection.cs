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
    
    public static IServiceCollection AddDbConnectionFactory(
        this IServiceCollection services,
        Func<IServiceProvider, IDbConnectionFactory> implementationFactory
        )
    {
        services.AddSingleton(implementationFactory);
        return services;
    }

    public static DbRepositoryBuilder AddRepositories(
        this IServiceCollection services,
        Action<DbRepositoryOptions> configureDefaults
        )
    {
        var defaults = new DbRepositoryOptions();
        configureDefaults?.Invoke(defaults);
        return new DbRepositoryBuilder(services, defaults);
    }
}