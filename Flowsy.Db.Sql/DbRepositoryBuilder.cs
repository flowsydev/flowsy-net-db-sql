using System.Reflection;
using Dapper;
using Flowsy.Core;
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

    /// <summary>
    /// Configures conventions for a repository type and adds it to the dependency injection system.
    /// </summary>
    /// <param name="configure">Action to execute to configure repository options.</param>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    public DbRepositoryBuilder Using<TService, TImplementation>(Action<DbRepositoryOptions>? configure = null) 
        where TImplementation : DbRepository, TService
        => Using(typeof(TService), typeof(TImplementation), configure);

    /// <summary>
    /// Configures conventions for a repository type and adds it to the dependency injection system.
    /// </summary>
    /// <param name="abstractionType">The type of the abstract interface.</param>
    /// <param name="implementationType">The type of the concrete implementation.</param>
    /// <param name="configure">Action to execute to configure repository options.</param>
    public DbRepositoryBuilder Using(Type abstractionType, Type implementationType, Action<DbRepositoryOptions>? configure = null)
    {
        var options = _defaultOptions.Clone();
        configure?.Invoke(options);
        DbRepositoryOptions.Register(implementationType, options);
        _services.AddTransient(abstractionType, implementationType);
        return this;
    }
    
    /// <summary>
    /// Configures conventions for a repository type and adds it to the dependency injection system.
    /// </summary>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="configure">Action to execute to configure repository options.</param>
    /// <typeparam name="TService">The service type.</typeparam>
    public DbRepositoryBuilder Using<TService>(Func<IServiceProvider, TService> implementationFactory, Action<DbRepositoryOptions>? configure = null) 
        where TService : DbRepository
    {        
        var options = _defaultOptions.Clone();
        configure?.Invoke(options);
        DbRepositoryOptions.Register<TService>(options);
        _services.AddTransient(implementationFactory);
        return this;
    }

    /// <summary>
    /// Configures column mappings for query results based on a case convention.
    /// </summary>
    /// <param name="columnCaseConvention">The case convention to use.</param>
    /// <param name="typeSelector">Type selector based on the target type taken from the provided assemblies.</param>
    /// <param name="assemblies">The assemblies to scan for types.</param>
    public DbRepositoryBuilder WithColumnMapping(
        CaseConvention columnCaseConvention, 
        Func<Type, bool> typeSelector,
        params Assembly[] assemblies
        )
        => WithColumnMapping(
            columnCaseConvention,
            assemblies
                .SelectMany(a => a.GetTypes().Where(typeSelector))
                .ToArray()
            );

    /// <summary>
    /// Configures column mappings for query results based on a property selector.
    /// </summary>
    /// <param name="typeSelector">Type selector based on the target type taken from the provided assemblies.</param>
    /// <param name="propertySelector">The selector based on the target type and column name.</param>
    /// <param name="assemblies">The assemblies to scan for types.</param>
    public DbRepositoryBuilder WithColumnMapping(
        Func<Type, bool> typeSelector,
        Func<Type, string, PropertyInfo> propertySelector,
        params Assembly[] assemblies
        )
        => WithColumnMapping(
            propertySelector,
            assemblies
                .SelectMany(a => a.GetTypes().Where(typeSelector))
                .ToArray()
            );

    /// <summary>
    /// Configures column mappings for query results based on a case convention.
    /// </summary>
    /// <param name="columnCaseConvention">The case convention to use.</param>
    /// <param name="types">The types to map.</param>
    public DbRepositoryBuilder WithColumnMapping(CaseConvention columnCaseConvention, params Type[] types)
        => WithColumnMapping(
            (entityType, columnName) => 
                entityType
                    .GetRuntimeProperties()
                    .FirstOrDefault(p => p.Name.ApplyConvention(columnCaseConvention) == columnName),
            types
            );
    
    /// <summary>
    /// Configures column mappings for query results based on a case convention.
    /// </summary>
    /// <param name="propertySelector">The selector based on the target type and column name.</param>
    /// <param name="types">The types to map.</param>
    public DbRepositoryBuilder WithColumnMapping(Func<Type, string, PropertyInfo> propertySelector, params Type[] types)
    {
        foreach (var type in types)
        {
            SqlMapper.RemoveTypeMap(type);
            SqlMapper.SetTypeMap(type, new CustomPropertyTypeMap(type, propertySelector));
        }

        return this;
    }

    /// <summary>
    /// Configures a type handler for custom formatting and parsing of database values.
    /// </summary>
    /// <param name="typeHandler">The custom type handler.</param>
    /// <typeparam name="T">The type of the target values.</typeparam>
    public DbRepositoryBuilder WithTypeHandler<T>(DbTypeHandler<T> typeHandler)
    {
        SqlMapper.AddTypeHandler(typeHandler);
        return this;
    }
}