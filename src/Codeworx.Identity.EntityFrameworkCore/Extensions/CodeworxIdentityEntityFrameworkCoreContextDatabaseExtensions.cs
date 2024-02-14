using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityEntityFrameworkCoreContextDatabaseExtensions
    {
        public static async Task<IDbContextTransaction> EnsureTransactionAsync(this DatabaseFacade database, CancellationToken token = default)
        {
            if (database.CurrentTransaction != null)
            {
                return new NullTransaction(database.CurrentTransaction);
            }

            return await database.BeginTransactionAsync(token);
        }

        private class NullTransaction : IDbContextTransaction
        {
            private IDbContextTransaction _currentTransaction;

            public NullTransaction(IDbContextTransaction currentTransaction)
            {
                _currentTransaction = currentTransaction;
            }

            public Guid TransactionId => _currentTransaction.TransactionId;

            public void Commit()
            {
                // do nothing;
            }

            public Task CommitAsync(CancellationToken cancellationToken = default)
            {
                // do nothing;
                return Task.CompletedTask;
            }

            public void Dispose()
            {
                // do nothing;
            }

            public ValueTask DisposeAsync()
            {
                // do nothing;
                return ValueTask.CompletedTask;
            }

            public void Rollback()
            {
                // do nothing;
            }

            public Task RollbackAsync(CancellationToken cancellationToken = default)
            {
                // do nothing;
                return Task.CompletedTask;
            }
        }
    }
}