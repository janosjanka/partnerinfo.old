// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Logging.EntityFramework
{
    public class RuleStore : IRuleStore
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleStore" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public RuleStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            Context = context;
        }

        /// <summary>
        /// If true will call SaveChanges after Create/Update/Delete.
        /// </summary>
        public bool AutoSaveChanges { get; set; } = true;

        /// <summary>
        /// Gets the context for the store.
        /// </summary>
        public DbContext Context { get; private set; }

        /// <summary>
        /// Inserts a rule.
        /// </summary>
        /// <param name="rule">The rule to insert.</param>
        /// <param name="user">The user who owns the rule.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> CreateAsync(RuleItem rule, AccountItem user, CancellationToken cancellationToken)
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
            var ruleEntity = new LoggingRule
            {
                UserId = user.Id,
                Options = LoggingRuleOptionsHelpers.Serialize(new LoggingRuleOptions { Conditions = rule.Conditions, Actions = rule.Actions })
            };
            Context.Add(ruleEntity);
            await SaveChangesAsync(cancellationToken);
            rule.Id = ruleEntity.Id;
            return ValidationResult.Success;
        }

        /// <summary>
        /// Updates a rule.
        /// </summary>
        /// <param name="rule">The rule to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> UpdateAsync(RuleItem rule, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }
            var ruleEntity = await Rules.FindAsync(cancellationToken, rule.Id);
            if (ruleEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, LoggingResources.RuleNotFound, rule.Id));
            }
            ruleEntity.Options = LoggingRuleOptionsHelpers.Serialize(new LoggingRuleOptions { Conditions = rule.Conditions, Actions = rule.Actions });
            Context.Patch(ruleEntity, rule);
            await SaveChangesAsync(cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Deletes a rule.
        /// </summary>
        /// <param name="rule">The rule to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> DeleteAsync(RuleItem rule, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }
            var ruleEntity = await Rules.FindAsync(cancellationToken, rule.Id);
            if (ruleEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, LoggingResources.RuleNotFound, rule.Id));
            }
            Context.Remove(ruleEntity);
            await SaveChangesAsync(cancellationToken);
            return ValidationResult.Success;
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
        public virtual async Task<RuleItem> FindByIdAsync(int id, RuleField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var ruleEntity = await Rules.FindAsync(cancellationToken, id);
            if (ruleEntity == null)
            {
                return null;
            }
            return LoggingQueries.ToRuleItem(ruleEntity);
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
        public virtual async Task<ListResult<RuleItem>> FindAllAsync(int userId, RuleField fields, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var rules = await Rules.Where(userId).ToArrayAsync(cancellationToken);
            var list = new List<RuleItem>();
            foreach (var rule in rules)
            {
                list.Add(LoggingQueries.ToRuleItem(rule));
            }
            return ListResult.Create(list);
        }

        /// <summary>
        /// The DbSet for access to events in the context.
        /// </summary>
        private DbSet<LoggingRule> Rules
        {
            get { return Context.Set<LoggingRule>(); }
        }

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException" /> if the context has already been disposed.
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
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        private Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.FromResult(0);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// If disposing, calls dispose on the Context.  Always nulls out the Context
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Do not dispose the context here because it must be performed by the caller
            // An ASP.NET application, which uses dependency injection with lifetime configuration, 
            // can be crashed if the context is disposed prematurely

            _disposed = true;
        }
    }
}
