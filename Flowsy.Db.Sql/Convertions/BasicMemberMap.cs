using System.Reflection;
using Dapper;

namespace Flowsy.Db.Sql.Convertions;

public class BasicMemberMap : SqlMapper.IMemberMap
{
    public BasicMemberMap(string columnName, Type memberType, ParameterInfo? parameter)
    {
        ColumnName = columnName;
        MemberType = memberType;
        Parameter = parameter;
    }

    public BasicMemberMap(string columnName, Type memberType, FieldInfo? field)
    {
        ColumnName = columnName;
        MemberType = memberType;
        Field = field;
    }

    public BasicMemberMap(string columnName, Type memberType, PropertyInfo? property)
    {
        ColumnName = columnName;
        MemberType = memberType;
        Property = property;
    }

    public BasicMemberMap(string columnName, Type memberType, PropertyInfo? property, FieldInfo? field, ParameterInfo? parameter)
    {
        ColumnName = columnName;
        MemberType = memberType;
        Property = property;
        Field = field;
        Parameter = parameter;
    }

    public string ColumnName { get; }

    public Type MemberType { get; }

    public PropertyInfo? Property { get; }

    public FieldInfo? Field { get; }

    public ParameterInfo? Parameter { get; }
}