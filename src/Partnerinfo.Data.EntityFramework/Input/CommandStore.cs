// Copyright (c) János Janka. All rights reserved.

using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Input.EntityFramework
{
    public class CommandStore : ICommandStore
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandStore" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CommandStore(DbContext context)
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
        /// Finds a command with the given primary key value.
        /// </summary>
        /// <param name="id">The primary key for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<CommandItem> FindByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Commands
                .Select(c => new CommandItem
                {
                    Id = c.Id,
                    Uri = c.Uri,
                    CreatedDate = c.CreatedDate,
                    Line = c.Line,
                    Data = c.Data
                })
                .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        /// <summary>
        /// Finds a command with the given primary key value.
        /// </summary>
        /// <param name="uri">The primary key for the item to be found.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<CommandItem> FindByUriAsync(string uri, CancellationToken cancellationToken)
        {
            return Commands
                .Select(c => new CommandItem
                {
                    Id = c.Id,
                    Uri = c.Uri,
                    CreatedDate = c.CreatedDate,
                    Line = c.Line,
                    Data = c.Data
                })
                .SingleOrDefaultAsync(c => c.Uri == uri, cancellationToken);
        }

        /// <summary>
        /// Inserts a command.
        /// </summary>
        /// <param name="command">The command to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> CreateAsync(CommandItem command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var commandEntity = Context.Add(new CommandEntity
            {
                Uri = command.Uri,
                CreatedDate = command.CreatedDate,
                Line = command.Line,
                Data = command.Data
            });
            await SaveChangesAsync(cancellationToken);
            command.Id = commandEntity.Id;
            return ValidationResult.Success;
        }

        /// <summary>
        /// Updates a command.
        /// </summary>
        /// <param name="command">The command to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> UpdateAsync(CommandItem command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var commandEntity = await Commands.FindAsync(cancellationToken, command.Id);
            if (commandEntity == null)
            {
                throw new InvalidOperationException($"Command was not found: {command.Id}");
            }
            Context.Patch(commandEntity, command);
            await SaveChangesAsync(cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Deletes a command.
        /// </summary>
        /// <param name="command">The command to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> DeleteAsync(CommandItem command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var commandEntity = await Commands.FindAsync(cancellationToken, command.Id);
            if (commandEntity == null)
            {
                throw new InvalidOperationException($"Command was not found: {command.Id}");
            }
            Context.Remove(commandEntity);
            await SaveChangesAsync(cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Removes all the expired commands.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> CleanAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            await Context.Database.ExecuteSqlCommandAsync("[Input].[CleanCommands]", cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// The DbSet for access to commands in the context.
        /// </summary>
        private DbSet<CommandEntity> Commands
        {
            get { return Context.Set<CommandEntity>(); }
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
        private async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (AutoSaveChanges)
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
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
