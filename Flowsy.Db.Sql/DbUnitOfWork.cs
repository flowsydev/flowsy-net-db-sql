using System.Data;
using System.Data.Common;
using Flowsy.Db.Abstractions;

namespace Flowsy.Db.Sql;

/// <summary>
/// Implements a unit of work wrapping a database transaction.
/// </summary>
public abstract class DbUnitOfWork : IUnitOfWork
{
    protected DbUnitOfWork(IDbConnection connection)
    {
        Connection = connection;
        Connection.Open();
        Transaction = Connection.BeginTransaction();
    }

    ~DbUnitOfWork()
    {
        Dispose(false);
    }

    /// <summary>
    /// The object representing the underlying transaction
    /// </summary>
    object IUnitOfWork.Transaction => Transaction;
    
    /// <summary>
    /// The object representing the underlying transaction
    /// </summary>
    protected IDbTransaction Transaction { get; }

    /// <summary>
    /// The database connection.
    /// </summary>
    protected IDbConnection Connection { get; }

    private bool _disposed;

    /// <summary>
    /// Persists all the changes made during the unit of work by committing the transaction to the underlying database.
    /// </summary>
    public void Save()
    {
        Transaction.Commit();
        OnSaved();
    }

    
    /// <summary>
    /// Asynchronously persists all the changes made during the unit of work by committing the transaction to the underlying database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        if (Transaction is DbTransaction dbTransaction)
            await dbTransaction.CommitAsync(cancellationToken);
        else
            Transaction.Commit();

        OnSaved();
    }

    /// <summary>
    /// Event raised when all the changes were saved successfully.
    /// </summary>
    public event EventHandler? Saved;
    
    /// <summary>
    /// Raises the Saved event.
    /// </summary>
    protected virtual void OnSaved()
    {
        Saved?.Invoke(this, EventArgs.Empty);
    }

    public void Undo()
    {
        Transaction.Dispose();
        Connection.Dispose();
        OnUndone();
    }

    public async Task UndoAsync(CancellationToken cancellationToken)
    {
        if (Transaction is DbTransaction dbTransaction)
            await dbTransaction.DisposeAsync();
        else 
            Transaction.Dispose();

        if (Connection is DbConnection dbConnection)
            await dbConnection.DisposeAsync();
        else
            Connection.Dispose();
        
        OnUndone();
    }
    
    /// <summary>
    /// Event raised when the changes were rolled back.
    /// </summary>
    public event EventHandler? Undone;
    
    /// <summary>
    /// Raises the Undone event.
    /// </summary>
    protected virtual void OnUndone()
    {
        Undone?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Triggers the disposal of the underlying connection which rolls back the uncommitted transaction. 
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronously triggers the disposal of the underlying connection which rolls back the uncommitted transaction. 
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the underlying connection which rolls back the uncommitted transaction.
    /// </summary>
    /// <param name="disposing">Indicates whether the object is being disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            Undo();
        }

        _disposed = true;
    }
    
    /// <summary>
    /// Asynchronously disposes the underlying connection which rolls back the uncommitted transaction.
    /// </summary>
    /// <param name="disposing">Indicates whether the object is being disposed.</param>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            await UndoAsync(CancellationToken.None);
        }

        _disposed = true;
    }
}