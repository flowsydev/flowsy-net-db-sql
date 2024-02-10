using System.Reflection;
using Dapper;
using Flowsy.Core;
using Flowsy.Db.Sql.Convertions;

namespace Flowsy.Db.Sql;

public sealed class DbRepositoryBuilder
{
    private readonly DbRepositoryOptions _defaultOptions;

    internal DbRepositoryBuilder(DbRepositoryOptions defaultOptions)
    {
        _defaultOptions = defaultOptions;
    }
    
    /// <summary>
    /// Configures conventions for a spefici type of repository.
    /// </summary>
    /// <param name="configure">Function to configure options for a specific type of repository.</param>
    /// <typeparam name="TRepository">The type of repository.</typeparam>
    public DbRepositoryBuilder WithRepository<TRepository>(Action<DbRepositoryOptions>? configure = null) 
        where TRepository : DbRepository
    {        
        var options = _defaultOptions.Clone();
        configure?.Invoke(options);
        DbRepositoryOptions.Register<TRepository>(options);
        return this;
    }

    /// <summary>
    /// Configures conventions for a spefici type of repository.
    /// </summary>
    /// <param name="repositoryType">The type of repository.</param>
    /// <param name="configure">Function to configure options for a specific type of repository.</param>
    public DbRepositoryBuilder WithRepository(Type repositoryType, Action<DbRepositoryOptions>? configure = null)
    {
        var options = _defaultOptions.Clone();
        configure?.Invoke(options);
        DbRepositoryOptions.Register(repositoryType, options);
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
    /// Configures column mappings for query results based on a case convention.
    /// </summary>
    /// <param name="columnCaseConvention">The case convention to use.</param>
    /// <param name="types">The types to map.</param>
    public DbRepositoryBuilder WithColumnMapping(CaseConvention columnCaseConvention, params Type[] types)
    {
        foreach (var type in types)
        {
            SqlMapper.RemoveTypeMap(type);
            SqlMapper.SetTypeMap(type, new CaseConventionTypeMap(type, columnCaseConvention));
        }

        return this;
    }
    
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