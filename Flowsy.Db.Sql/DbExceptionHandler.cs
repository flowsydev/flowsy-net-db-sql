namespace Flowsy.Db.Sql;

public abstract class DbExceptionHandler
{
    public abstract Exception? Handle(Exception exception);
}