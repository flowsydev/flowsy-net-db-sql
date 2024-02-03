using System.Data;
using System.Data.Common;
using Flowsy.Db.Abstractions;
using Flowsy.Db.Sql.Resources;

namespace Flowsy.Db.Sql;

/// <summary>
/// Implements a unit of work wrapping a database transaction.
/// </summary>
public abstract class DbUnitOfWork : IUnitOfWork
{
    private readonly DbConnectionFactory _connectionFactory;
    private IDbConnection? _connection;
    private bool _disposed;

    public event EventHandler? WorkBegun;
    public event EventHandler? WorkSaved;
    public event EventHandler? WorkDiscarded;

    protected DbUnitOfWork(DbConnectionFactory connectionFactory, DbExceptionHandler? exceptionHandler = null)
    {
        _connectionFactory = connectionFactory;
        ExceptionHandler = exceptionHandler;
    }

    ~DbUnitOfWork()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            TryRollbackTransaction();
            _connection?.Dispose();
            _connection = null;
        }

        _disposed = true;
    }
    
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            await TryRollbackTransactionAsync(CancellationToken.None);
            
            if (_connection is DbConnection dbConnection)
                await dbConnection.DisposeAsync();
            else 
                _connection?.Dispose();
            
            _connection = null;
        }
        
        _disposed = true;
    }

    protected virtual void TryRollbackTransaction()
    {
        if (Transaction is null)
            return;
        
        Transaction.Rollback();
        Transaction = null;
    }

    protected virtual async Task TryRollbackTransactionAsync(CancellationToken cancellationToken)
    {
        if (Transaction is null)
            return;
        
        if (Transaction is DbTransaction dbTransaction)
            await dbTransaction.RollbackAsync(cancellationToken);
        else
            Transaction!.Rollback();
        
        Transaction = null;
    }

    protected DbUnitOfWorkOptions Options => DbUnitOfWorkOptions.Resolve(GetType());

    protected internal IDbConnection Connection
    {
        get
        {
            if (_connection is not null)
                return _connection;

            _connection = _connectionFactory.GetConnection(Options.ConnectionKey) ??
                          throw new InvalidOperationException(Strings.CouldNotGetConnection);
            
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            return _connection;
        }
    }

    protected internal IDbTransaction? Transaction { get; private set; }
    
    protected DbExceptionHandler? ExceptionHandler { get; }

    private void ValidateTransactionalState()
    {
        if (_connection is null || Transaction is null)
        {
            throw new InvalidOperationException(
                string.Format(Strings.MustBeginWorkByInvokingMethodX, nameof(BeginWork))
            );
        }
    }

    protected virtual void OnWorkBegun(EventArgs e)
    {
        WorkBegun?.Invoke(this, e);
    }
    
    public virtual void BeginWork()
    {
        Transaction = Connection.BeginTransaction();
        OnWorkBegun(EventArgs.Empty);
    }

    protected virtual void OnWorkSaved(EventArgs e)
    {
        WorkSaved?.Invoke(this, e);
    }

    public virtual void SaveWork()
    {
        ValidateTransactionalState(); 
        
        Transaction!.Commit();
        
        OnWorkSaved(EventArgs.Empty);
    }

    public virtual async Task SaveWorkAsync(CancellationToken cancellationToken)
    {
        ValidateTransactionalState();
        
        if (Transaction is DbTransaction dbTransaction)
            await dbTransaction.CommitAsync(cancellationToken);
        else
            Transaction!.Commit();
        
        OnWorkSaved(EventArgs.Empty);
    }

    protected virtual void OnWorkDiscarded(EventArgs e)
    {
        WorkDiscarded?.Invoke(this, e);
    }
    
    public virtual void DiscardWork()
    {
        ValidateTransactionalState();
        TryRollbackTransaction();
        OnWorkDiscarded(EventArgs.Empty);
    }

    public virtual async Task DiscardWorkAsync(CancellationToken cancellationToken)
    {
        ValidateTransactionalState();
        await TryRollbackTransactionAsync(cancellationToken);
        OnWorkDiscarded(EventArgs.Empty);
    }
}