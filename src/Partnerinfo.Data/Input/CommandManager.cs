// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Input
{
    public class CommandManager : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManager" /> class
        /// </summary>
        /// <param name="store">The project store.</param>
        public CommandManager(ICommandStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            Store = store;
        }

        /// <summary>
        /// The store that the <see cref="CommandManager" /> operates against.
        /// </summary>
        protected ICommandStore Store { get; set; }

        /// <summary>
        /// URI Provider
        /// </summary>
        public ICommandUriProvider UriProvider { get; set; } = CommandDefaultUriProvider.Default;

        /// <summary>
        /// URI Provider
        /// </summary>
        public ICommandMailService MailService { get; set; } = CommandDefaultMailService.Default;

        /// <summary>
        /// Command invoker
        /// </summary>
        public ICommandInvoker Invoker { get; set; }

        /// <summary>
        /// Returns true if a command invoker is specified.
        /// </summary>
        public virtual bool SupportsInvoker
        {
            get
            {
                ThrowIfDisposed();
                return Invoker != null;
            }
        }

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
            ThrowIfDisposed();
            return Store.FindByIdAsync(id, cancellationToken);
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
            ThrowIfDisposed();
            return Store.FindByUriAsync(uri, cancellationToken);
        }

        /// <summary>
        /// Inserts a command.
        /// </summary>
        /// <param name="command">The command to insert.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ValidationResult> CreateAsync(CommandItem command, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            command.Uri = UriProvider.Generate(command.Uri);
            return Store.CreateAsync(command, cancellationToken);
        }

        /// <summary>
        /// Updates a command.
        /// </summary>
        /// <param name="command">The command to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ValidationResult> UpdateAsync(CommandItem command, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            return Store.UpdateAsync(command, cancellationToken);
        }

        /// <summary>
        /// Deletes a command.
        /// </summary>
        /// <param name="command">The command to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ValidationResult> DeleteAsync(CommandItem command, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            return Store.DeleteAsync(command, cancellationToken);
        }

        /// <summary>
        /// Removes all the expired commands.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ValidationResult> CleanAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return Store.CleanAsync(cancellationToken);
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
                Store.Dispose();
            }
            _disposed = true;
            Store = null;
        }
    }
}
