using System.Text;
using Flowsy.Core;

namespace Flowsy.Db.Sql.Conventions;

public abstract class DbConvention
{
    protected DbConvention() : this(null, null, null)
    {
    }

    protected DbConvention(string? prefix, string? suffix, CaseConvention? casing)
    {
        Prefix = prefix;
        Suffix = suffix;
        Casing = casing;
    }
    
    /// <summary>
    /// Prefix to apply to the name.
    /// </summary>
    public string? Prefix { get; set; }
    
    /// <summary>
    /// Suffix to apply to the name.
    /// </summary>
    public string? Suffix { get; set; }
    
    /// <summary>
    /// Casing convention to apply to the name.
    /// </summary>
    public CaseConvention? Casing { get; set; }

    /// <summary>
    /// Formats the provided simple name using the conventions set for this instance.
    /// </summary>
    /// <param name="simpleName">The name without prefix or suffix.</param>
    /// <returns>The formatted name.</returns>
    public virtual string Format(string simpleName)
    {
        var builder = new StringBuilder();
        
        if (!string.IsNullOrEmpty(Prefix))
            builder.Append(Prefix);

        if (Casing.HasValue && !simpleName.SatisfiesConvention(Casing.Value))
            builder.Append(simpleName.ApplyConvention(Casing));
        else
            builder.Append(simpleName);
        
        if (!string.IsNullOrEmpty(Suffix))
            builder.Append(Suffix);

        return builder.ToString();
    }

    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <returns>A new instance with the same values as the current instance.</returns>
    public abstract DbConvention Clone();
}