using System.Text;
using Flowsy.Core;

namespace Flowsy.Db.Sql.Conventions;

public abstract class DbSchemaChildConvention : DbConvention
{
    protected DbSchemaChildConvention() : this(null, null, null)
    {
    }

    protected DbSchemaChildConvention(string? prefix, string? suffix, CaseConvention? casing) : base(prefix, suffix, casing)
    {
    }

    /// <summary>
    /// Formats the name of an object contained directly under a database schema.
    /// </summary>
    /// <param name="simpleName">The object name without prefix or suffix.</param>
    /// <param name="schema">The schema name.</param>
    /// <returns>The formatted name.</returns>
    public virtual string Format(string simpleName, string? schema)
    {
        var builder = new StringBuilder();
        
        if (!string.IsNullOrEmpty(schema))
            builder.AppendFormat("{0}.", schema);

        builder.Append(Format(simpleName));

        return builder.ToString();
    }
}