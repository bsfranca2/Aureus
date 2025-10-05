using System.Data;

using Bsfranca2.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Aureus.Infrastructure.Data.Repositories;

public sealed class UnitOfWork(DatabaseContext context) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;
    private bool _disposed;
    private bool _transactionFinished;
    
    public bool HasActiveTransaction => _transaction != null;

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (_transaction != null || _disposed || _transactionFinished)
        {
            throw new InvalidOperationException("Transaction already started, finished, or UnitOfWork is disposed.");
        }

        _transaction = await context.Database.BeginTransactionAsync(isolationLevel);
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction == null || _disposed || _transactionFinished)
        {
            throw new InvalidOperationException(
                "Cannot commit. Transaction not started, finished, or UnitOfWork is disposed.");
        }

        try
        {
            await context.SaveChangesAsync();
            await _transaction.CommitAsync();
            _transactionFinished = true;
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null || _disposed || _transactionFinished)
        {
            return;
        }

        await _transaction.RollbackAsync();
        _transactionFinished = true;
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (!_transactionFinished && _transaction != null)
                {
                    try { _transaction.Rollback(); }
                    catch
                    {
                        // ignored
                    }
                }

                _transaction?.Dispose();

                context.Dispose();
            }

            _disposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (!_transactionFinished && _transaction != null)
            {
                try { await _transaction.RollbackAsync(); }
                catch
                {
                    // ignored
                }
            }

            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }

            await context.DisposeAsync();
            _disposed = true;
        }
    }
}