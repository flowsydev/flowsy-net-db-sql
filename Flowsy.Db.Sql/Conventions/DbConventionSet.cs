namespace Flowsy.Db.Sql.Conventions;

public class DbConventionSet
{
    public DbConventionSet()
        : this(
            new DbRoutineConvention(),
            new DbParameterConvention(),
            new DbEnumConvention(),
            DbDateTimeOffsetFormat.Utc
            )
    {
    }

    public DbConventionSet(
        DbRoutineConvention routines,
        DbParameterConvention parameters,
        DbEnumConvention enums,
        DbDateTimeOffsetFormat dateTimeOffsetFormat
        )
    {
        Routines = routines;
        Parameters = parameters;
        Enums = enums;
        DateTimeOffsetFormat = dateTimeOffsetFormat;
    }
    public DbRoutineConvention Routines { get; set; }
    public DbParameterConvention Parameters { get; set; }
    public DbEnumConvention Enums { get; set; }
    public DbDateTimeOffsetFormat DateTimeOffsetFormat { get; set; }

    public DbConventionSet Clone()
        => new(Routines, Parameters, Enums, DateTimeOffsetFormat);
}