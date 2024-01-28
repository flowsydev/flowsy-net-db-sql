using System.Text;
using Flowsy.Core;

namespace Flowsy.Db.Sql.Conventions;

public abstract class DbConvention
{
    protected DbConvention()
        : this(null, null, null)
    {
    }

    protected DbConvention(
        string? prefix,
        string? suffix,
        CaseConvention? casing
        )
    {
        Prefix = prefix;
        Suffix = suffix;
        Casing = casing;
    }
    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
    public CaseConvention? Casing { get; set; }

    public virtual string Format(string simpleName)
        => Format(simpleName, null);
    
    public virtual string Format(string simpleName, string? schema)
    {
        var builder = new StringBuilder();
        
        if (!string.IsNullOrEmpty(schema))
            builder.AppendFormat("{0}.", schema);

        if (!string.IsNullOrEmpty(Prefix))
            builder.Append(Prefix);

        builder.Append(simpleName.ApplyConvention(Casing));
        
        if (!string.IsNullOrEmpty(Suffix))
            builder.Append(Suffix);

        return builder.ToString();
    }

    public abstract DbConvention Clone();
}