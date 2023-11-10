namespace Flowsy.Db.Sql.Conventions;

/// <summary>
/// Formats a parameter placeholder to be used in a query.
/// </summary>
/// <param name="routineName">The name of the routine to be executed.</param>
/// <param name="routineType">The type of routine to be executed.</param>
/// <param name="parameterName">The name of the parameter.</param>
/// <param name="parameterValue">The value of the parameter.</param>
/// <returns>The formatted parameter placeholder.</returns>
public delegate string FormatRoutineParameterPlaceholder(
    string routineName,
    DbRoutineType routineType,
    string parameterName,
    object? parameterValue
    );