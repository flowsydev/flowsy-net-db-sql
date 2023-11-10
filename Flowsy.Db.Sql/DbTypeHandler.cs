using System.Data;
using Dapper;

namespace Flowsy.Db.Sql;

public abstract class DbTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    protected DbTypeHandler(DbType parameterType)
    {
        ParameterType = parameterType;
    }

    public DbType ParameterType { get; protected set; }

    public override void SetValue(IDbDataParameter parameter, T? value)
    {
        throw new NotSupportedException(string.Format(
            Resources.Strings.CouldNotSetValueXForParameterX,
            value,
            parameter.ParameterName
            ));
    }

    public override T? Parse(object value)
    {
        throw new NotSupportedException(string.Format(Resources.Strings.CouldNotParseValueX, value));
    }
}