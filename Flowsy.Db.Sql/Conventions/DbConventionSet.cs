namespace Flowsy.Db.Sql.Conventions;

public class DbConventionSet
{
    public DbConventionSet()
        : this(
            new DbTableConvention(),
            new DbColumnConvention(),
            new DbRoutineConvention(),
            new DbParameterConvention(),
            new DbEnumConvention(),
            DbDateTimeOffsetFormat.Utc
            )
    {
    }

    public DbConventionSet(
        DbTableConvention tables,
        DbColumnConvention columns,
        DbRoutineConvention routines,
        DbParameterConvention parameters,
        DbEnumConvention enums,
        DbDateTimeOffsetFormat dateTimeOffsetFormat
        )
    {
        Tables = tables;
        Columns = columns;
        Routines = routines;
        Parameters = parameters;
        Enums = enums;
        DateTimeOffsetFormat = dateTimeOffsetFormat;
    }

    public DbTableConvention Tables { get; set; }
    public DbColumnConvention Columns { get; set; }
    public DbRoutineConvention Routines { get; set; }
    public DbParameterConvention Parameters { get; set; }
    public DbEnumConvention Enums { get; set; }
    public DbDateTimeOffsetFormat DateTimeOffsetFormat { get; set; }

    public DbConventionSet Clone()
        => new(Tables, Columns, Routines, Parameters, Enums, DateTimeOffsetFormat);
}