// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Logging
{
    public class RuleManager : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleManager"/> class.
        /// </summary>
        /// <param name="store">The store that the <see cref="RuleManager" /> operates against.</param>
        public RuleManager(IRuleStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            Store = store;
        }

        /// <summary>
        /// The store that the <see cref="RuleManager" /> operates against
        /// </summary>
        protected IRuleStore Store { get; set; }

        /// <summary>
        /// Inserts a rule.
        /// </summary>
        /// <param name="rule">The rule to insert.</param>
        /// <param name="user">The user who owns the rule.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// rule
        /// or
        /// </exception>
        public virtual Task<ValidationResult> CreateAsync(RuleItem rule, AccountItem user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Store.CreateAsync(rule, user, cancellationToken);
        }

        /// <summary>
        /// Updates a rule.
        /// </summary>
        /// <param name="rule">The rule to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ValidationResult> UpdateAsync(RuleItem rule, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }
            return Store.UpdateAsync(rule, cancellationToken);
        }

        /// <summary>
        /// Deletes a rule.
        /// </summary>
        /// <param name="rule">The rule to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ValidationResult> DeleteAsync(RuleItem rule, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }
            return Store.DeleteAsync(rule, cancellationToken);
        }

        /// <summary>
        /// Finds a rule with the given primary key value.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<RuleItem> FindByIdAsync(int id, RuleField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Store.FindByIdAsync(id, fields, cancellationToken);
        }

        /// <summary>
        /// Finds a collection of rules with the given values.
        /// </summary>
        /// <param name="userId">The user identifier for the item to be found.</param>
        /// <param name="fields">The related fields to include in the query results.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ListResult<RuleItem>> FindAllAsync(int userId, RuleField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Store.FindAllAsync(userId, fields, cancellationToken);
        }

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException" /> if the context has already been disposed
        /// </summary>
        /// <exception cref="System.ObjectDisposedException" />
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// If disposing, calls dispose on the Context. Always nulls out the Context
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Store != null)
            {
                // Store.Dispose();
            }
            _disposed = true;
            Store = null;
        }
    }
}
