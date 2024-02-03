using System.Reflection;
using Dapper;
using Flowsy.Core;

namespace Flowsy.Db.Sql.Convertions;

public sealed class CaseConventionTypeMap : SqlMapper.ITypeMap
{
    private readonly Type _type;
    private readonly CaseConvention _caseConvention;
    
    private readonly Dictionary<ConstructorInfo, IDictionary<string, SqlMapper.IMemberMap>> _constructorMappings = new ();
    private readonly Dictionary<string, SqlMapper.IMemberMap> _memberMappings = new ();

    public CaseConventionTypeMap(Type type, CaseConvention caseConvention)
    {
        _type = type;
        _caseConvention = caseConvention;
    }

    private bool IsParameterMatch(ParameterInfo parameter, string columnName, Type columnType)
        => parameter.Name is not null &&
           columnName == parameter.Name.ApplyConvention(_caseConvention) &&
           parameter.ParameterType.IsAssignableFrom(columnType);

    private bool IsConstructorMatch(ConstructorInfo constructor, string[] names, Type[] types)
    {
        var parameters = constructor.GetParameters();
        if (parameters.Length != names.Length && parameters.Length != types.Length)
            return false;
        
        for (var index = 0; index <= parameters.Length; index++)
            if (!IsParameterMatch(parameters[index], names[index], types[index]))
                return false;

        return true;
    }

    public ConstructorInfo? FindConstructor(string[] names, Type[] types)
        => _type
            .GetConstructors()
            .FirstOrDefault(c => IsConstructorMatch(c, names, types));

    public ConstructorInfo? FindExplicitConstructor()
        => _type
            .GetConstructors()
            .OrderBy(c => c.GetParameters().Length)
            .FirstOrDefault();
    
    public SqlMapper.IMemberMap? GetConstructorParameter(ConstructorInfo constructor, string columnName)
    {
        if (!_constructorMappings.TryGetValue(constructor, out var mapping))
        {
            mapping = new Dictionary<string, SqlMapper.IMemberMap>();
            _constructorMappings[constructor] = mapping;
        }

        if (mapping.TryGetValue(columnName, out var memberMap))
            return memberMap;

        var parameter = constructor
            .GetParameters()
            .FirstOrDefault(p => p.Name?.ApplyConvention(_caseConvention) == columnName);

        if (parameter is null)
            return null;

        memberMap = new BasicMemberMap(columnName, parameter.ParameterType, parameter);
        mapping[columnName] = memberMap;
        return memberMap;
    }

    public SqlMapper.IMemberMap? GetMember(string columnName)
    {
        if (_memberMappings.TryGetValue(columnName, out var memberMap))
            return memberMap;

        var property = _type
            .GetRuntimeProperties()
            .FirstOrDefault(p => p.Name.ApplyConvention(_caseConvention) == columnName);

        if (property is not null)
        {
            memberMap = new BasicMemberMap(columnName, property.PropertyType, property);
            _memberMappings[columnName] = memberMap;
            return memberMap;
        }
        
        var field = _type
            .GetRuntimeFields()
            .FirstOrDefault(f => f.Name.ApplyConvention(_caseConvention) == columnName);

        if (field is null)
            return null;

        memberMap = new BasicMemberMap(columnName, field.FieldType, field);
        _memberMappings[columnName] = memberMap;
        return memberMap;
    }
}