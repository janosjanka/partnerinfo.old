// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Security;

namespace Partnerinfo.Logging.EntityFramework
{
    /// <summary>
    /// Provides facilities for querying and working with system data as objects.
    /// </summary>
    public class EventStore :
        IEventStore,
        IEventCategoryStore,
        IEventBulkOperations
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStore" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public EventStore(DbContext context)
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

        //
        // IEventStore
        //

        /// <summary>
        /// Creates new web events and adds there to the database in a bulk operation.
        /// </summary>
        /// <param name="eventItem">The event descriptor object.</param>
        /// <param name="users">The users.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Returns the unique identifier (GUID) for a group of events.
        /// </returns>
        public virtual async Task<ValidationResult> CreateAsync(EventItem eventItem, IEnumerable<AccountItem> users, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }
            if (users == null)
            {
                throw new ArgumentNullException(nameof(users));
            }

            var eventEntity = Context.Add(new LoggingEvent
            {
                ObjectType = eventItem.ObjectType,
                ObjectId = eventItem.ObjectId,
                ContactId = eventItem.Contact?.Id,
                ContactState = eventItem.ContactState,
                ProjectId = eventItem.Project?.Id,
                BrowserBrand = eventItem.BrowserBrand,
                BrowserVersion = eventItem.BrowserVersion,
                MobileDevice = eventItem.MobileDevice,
                AnonymId = eventItem.AnonymId,
                ClientId = eventItem.ClientId,
                CustomUri = eventItem.CustomUri,
                ReferrerUrl = eventItem.ReferrerUrl,
                Message = eventItem.Message
            });

            foreach (var user in users)
            {
                eventEntity.Users.Add(new LoggingEventSharing { UserId = user.Id });
            }

            await SaveChangesAsync(cancellationToken);
            eventItem.Id = eventEntity.Id;
            return ValidationResult.Success;
        }

        /// <summary>
        /// Updates an event.
        /// </summary>
        /// <param name="eventItem">The event to update.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> UpdateAsync(EventItem eventItem, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }
            var eventEntity = await Events.FindAsync(cancellationToken, eventItem.Id);
            if (eventEntity == null)
            {
                throw new InvalidOperationException("");
            }
            await SaveChangesAsync(cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Deletes an event.
        /// </summary>
        /// <param name="eventItem">The event to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ValidationResult> DeleteAsync(EventItem eventItem, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }
            var eventEntity = await Events.Include(e => e.Users).FirstOrDefaultAsync(e => e.Id == eventItem.Id, cancellationToken);
            if (eventEntity == null)
            {
                throw new InvalidOperationException("");
            }
            foreach (var user in eventEntity.Users)
            {
                Context.Remove(user);
            }
            Context.Remove(eventEntity);
            await SaveChangesAsync(cancellationToken);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Deletes a group of events.
        /// </summary>
        /// <param name="userId">The User ID (Foreign Key).</param>
        /// <param name="eventIds">A collection of primary keys of events.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task BulkDeleteAsync(int userId, IEnumerable<int> eventIds, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (eventIds == null)
            {
                throw new ArgumentNullException("eventIds");
            }
            return Context.Database.ExecuteSqlCommandAsync(
                "Logging.DeleteEvents @UserId, @EventIds",
                cancellationToken,
                DbParameters.Value("UserId", userId),
                DbParameters.IdList("EventIds", eventIds));
        }

        /// <summary>
        /// Gets the number of unread web events.
        /// </summary>
        /// <param name="ownerId">The unqiue identifier of the <see cref="IdentityUser" />.</param>
        /// <returns>
        /// The number of unread web events.
        /// </returns>
        public virtual Task<int> GetUnreadCountAsync(int ownerId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Context.Database.SqlQuery<int>(
                "DECLARE @ReturnValue int;" +
                "EXEC @ReturnValue = Logging.GetUnreadEventCount @OwnerId;" +
                "SELECT @ReturnValue",
                DbParameters.Value("OwnerId", ownerId))
                .FirstAsync(cancellationToken);
        }

        /// <summary>
        /// Sets the event that correlates to the given event.
        /// </summary>
        /// <param name="id">The Event ID (Primary Key).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task SetCorrelationAsync(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Context.Database.ExecuteSqlCommandAsync(
                "Logging.SetEventCorrelation @Id",
                cancellationToken,
                DbParameters.Value("Id", id));
        }

        /// <summary>
        /// Finishes an event.
        /// </summary>
        /// <param name="id">The unique identifier for the HTTP request.</param>
        public virtual Task SetFinishDateAsync(int id, DateTime? finishDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Context.Database.ExecuteSqlCommandAsync(
                "Logging.SetEventFinishDate @Id, @FinishDate",
                cancellationToken,
                DbParameters.Value("Id", id),
                DbParameters.Value("FinishDate", finishDate));
        }

        /// <summary>
        /// Marks events as read or unread.
        /// </summary>
        public virtual Task SetLastReadDateAsync(int userId, DateTime lastReadDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Context.Database.ExecuteSqlCommandAsync(
                "Logging.SetEventLastReadDate @UserId, @LastReadDate",
                cancellationToken,
                DbParameters.Value("UserId", userId),
                DbParameters.Value("LastReadDate", lastReadDate));
        }

        /// <summary>
        /// Finds a collection of events with the given values
        /// </summary>
        /// <param name="ownerId">Owner ID (Foreign Key)</param>
        /// <param name="categoryId">Category ID (Foreign Key)</param>
        /// <param name="projectId">Project ID (Foreign Key)</param>
        /// <param name="actionId">Action ID (Foreign Key)</param>
        /// <param name="pageId">Page ID (Foreign Key)</param>
        /// <param name="contactState">Contact state flag</param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="emails">A collection of contact emails.</param>
        /// <param name="clients">A collection of Client IDs.</param>
        /// <param name="pageIndex">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<ListResult<EventResult>> FindAllAsync(
            int userId,
            int? categoryId,
            DateTime? dateFrom,
            DateTime? dateTo,
            ObjectType? objectType,
            int? objectId,
            int? contactId,
            ObjectState? contactState,
            int? projectId,
            string customUri,
            IEnumerable<string> emails,
            IEnumerable<string> clients,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var listBuilder = ImmutableArray.CreateBuilder<EventResult>();
            var total = 0;
            var connection = Context.Database.Connection;
            try
            {
                await connection.OpenAsync(cancellationToken);

                // This solution has a considerable effect on performance avoiding EF Code First
                // object materialization. Log queries are performance critical. Do not be lazy :-)
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Logging.GetEvents";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(DbParameters.Value("UserId", userId));
                    command.Parameters.Add(DbParameters.Value("CategoryId", categoryId));
                    command.Parameters.Add(DbParameters.Value("DateFrom", dateFrom));
                    command.Parameters.Add(DbParameters.Value("DateTo", dateTo));
                    command.Parameters.Add(DbParameters.Value("ObjectTypeId", objectType));
                    command.Parameters.Add(DbParameters.Value("ObjectId", objectId));
                    command.Parameters.Add(DbParameters.Value("ContactId", contactId));
                    command.Parameters.Add(DbParameters.Value("ContactStateId", contactState));
                    command.Parameters.Add(DbParameters.Value("ProjectId", projectId));
                    command.Parameters.Add(DbParameters.Value("CustomUri", customUri));
                    command.Parameters.Add(DbParameters.EmailList("Emails", emails));
                    command.Parameters.Add(DbParameters.ClientList("Clients", clients));
                    command.Parameters.Add(DbParameters.Value("PageIndex", pageIndex));
                    command.Parameters.Add(DbParameters.Value("PageSize", pageSize));
                    command.Parameters.Add(DbParameters.Value("TotalItemCount", 50000));

                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        var mapper = new LoggingEventResultMapper(reader);

                        // Read the first record to get the total number of events before paging is applied.
                        // This stored procedure uses an optimized CTE query, however, a side effect of
                        // this solution is that the total number of records is presented in each row.
                        if (!reader.Read() || (total = mapper.GetTotal()) <= 0)
                        {
                            return ListResult<EventResult>.Empty;
                        }
                        do
                        {
                            listBuilder.Add(mapper.Translate());
                        }
                        while (reader.Read());
                    }
                }
            }
            finally
            {
                connection.Close();
            }
            return ListResult.Create(listBuilder.ToImmutable(), total);
        }

        /// <summary>
        /// Finds a collection of events with the given primary key value.
        /// </summary>
        /// <param name="id">Event ID (Primary Key).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<IList<EventResult>> FindAllByIdAsync(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var list = new List<EventResult>();
            var connection = Context.Database.Connection;
            try
            {
                await connection.OpenAsync(cancellationToken);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Logging.GetEventsById";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(DbParameters.Value("Id", id));

                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        var mapper = new LoggingEventResultMapper(reader);
                        while (reader.Read())
                        {
                            list.Add(mapper.Translate());
                        }
                    }
                }
            }
            finally
            {
                connection.Close();
            }

            return list;
        }

        /// <summary>
        /// Gets a collection of event notifications with the given values.
        /// </summary>
        /// <param name="userId">The site where the events occured.</param>
        /// <param name="clientId">The client who is the actor.</param>
        /// <param name="offset">The index of the page of results to return. Use 1 to indicate the first page.</param>
        /// <param name="limit">The size of the page of results to return. <paramref name="pageIndex" /> is non-zero-based.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<IList<MessageResult>> FindAllMessagesAsync(int projectId, string clientId, int offset, int limit, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await (
                from e in Events
                where
                   e.StartDate >= DbFunctions.AddDays(DateTime.UtcNow, -14)
                   && e.ObjectType == ObjectType.Message
                   && e.ProjectId == projectId
                   && e.ClientId == clientId
                orderby e.StartDate descending
                select new MessageResult
                {
                    Id = e.Id,
                    Contact = e.Contact == null ? null : new AccountItem
                    {
                        Id = e.Contact.Id,
                        Email = e.Contact.Email,
                        FirstName = e.Contact.FirstName,
                        LastName = e.Contact.LastName,
                        NickName = e.Contact.NickName,
                        Gender = e.Contact.Gender,
                        Birthday = e.Contact.Birthday
                    },
                    StartDate = e.StartDate,
                    ClientId = e.ClientId,
                    CustomUri = e.CustomUri,
                    Message = e.Message
                }
            )
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
        }

        //
        // IEventAceStore
        //

        /// <summary>
        /// Adds the specified <paramref name="acl"/> for the specified <paramref name="eventItem"/>.
        /// </summary>
        /// <param name="eventItem">The project to set the specified <paramref name="acl"/>.</param>
        /// <param name="acl">A list of <see cref="AccessRuleItem"/>s to be specified on the project.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public virtual async Task AddAclAsync(EventItem eventItem, IEnumerable<AccessRuleItem> acl, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }
            var eventEntity = await Events.FindAsync(cancellationToken, eventItem.Id);
            if (eventEntity == null)
            {
                throw new InvalidOperationException();
            }
            foreach (var ace in acl)
            {
                if (ace.User != null)
                {
                    eventEntity.Users.Add(new LoggingEventSharing { UserId = ace.User.Id });
                }
            }
        }

        /// <summary>
        /// Add a the specified <paramref name="eventItem"/> to the named category, as an asynchronous operation.
        /// </summary>
        /// <param name="eventItem">The event to add to the named category.</param>
        /// <param name="categoryName">The name of the category to add the event to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public virtual async Task AddToCategoryAsync(EventItem eventItem, string categoryName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }
            if (categoryName == null)
            {
                throw new ArgumentNullException("categoryName");
            }
            var categoryEntity = await Categories.SingleOrDefaultAsync(c => c.Name == categoryName, cancellationToken);
            if (categoryEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, LoggingResources.CategoryNotFound, categoryName));
            }
            categoryEntity.Events.Add(new LoggingEventSharing { EventId = eventItem.Id, CategoryId = categoryEntity.Id });
        }

        /// <summary>
        /// Add a the specified <paramref name="eventItem"/> from the named category, as an asynchronous operation.
        /// </summary>
        /// <param name="eventItem">The event to remove the named category from.</param>
        /// <param name="categoryName">The name of the category to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public virtual Task RemoveFromCategoryAsync(EventItem eventItem, string categoryName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }
            if (categoryName == null)
            {
                throw new ArgumentNullException("categoryName");
            }
            return Task.FromResult(0);
        }

        //
        // IEventBulkOperations
        //

        /// <summary>
        /// Gets a group of primary keys of events with the given values.
        /// </summary>
        /// <param name="ownerId">Owner ID (Foreign Key)</param>
        /// <param name="categoryId">Category ID (Foreign Key)</param>
        /// <param name="projectId">Project ID (Foreign Key)</param>
        /// <param name="actionId">Action ID (Foreign Key)</param>
        /// <param name="pageId">Page ID (Foreign Key)</param>
        /// <param name="contactState">Contact state flag</param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="emails">A collection of contact emails.</param>
        /// <param name="clients">A collection of Client IDs.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<IList<int>> GetIdsAsync(
            int userId,
            int? categoryId,
            DateTime? dateFrom,
            DateTime? dateTo,
            ObjectType? objectType,
            int? objectId,
            int? contactId,
            ObjectState? contactState,
            int? projectId,
            string customUri,
            IEnumerable<string> emails,
            IEnumerable<string> clients,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await Context.Database.SqlQuery<int>(
                "SELECT Id FROM Logging.GetEventIds(@UserId, @CategoryId, @DateFrom, @DateTo, @ObjectTypeId, @ObjectId, @ContactId, @ContactState, @ProjectId, @CustomUri, @Emails, @Clients)",
                DbParameters.Value("UserId", userId),
                DbParameters.Value("CategoryId", categoryId),
                DbParameters.Value("DateFrom", dateFrom),
                DbParameters.Value("DateTo", dateTo),
                DbParameters.Value("ObjectTypeId", objectType),
                DbParameters.Value("ObjectId", objectId),
                DbParameters.Value("ContactId", contactId),
                DbParameters.Value("ContactState", contactState),
                DbParameters.Value("ProjectId", projectId),
                DbParameters.Value("CustomUri", customUri),
                DbParameters.EmailList("Emails", emails),
                DbParameters.ClientList("Clients", clients))
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Sets the given category for a group of events.
        /// </summary>
        /// <param name="userId">The User ID (Foreign Key).</param>
        /// <param name="eventIds">A collection of primary keys of events.</param>
        /// <param name="categoryId">The Category ID (Foreign Key).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task BulkSetCategoriesAsync(int userId, IEnumerable<int> eventIds, int? categoryId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return Context.Database.ExecuteSqlCommandAsync(
                "Logging.SetEventCategories @UserId, @EventIds, @CategoryId",
                cancellationToken,
                DbParameters.Value("UserId", userId),
                DbParameters.IdList("EventIds", eventIds),
                DbParameters.Value("CategoryId", categoryId));
        }

        /// <summary>
        /// Sets the given contact for all the events.
        /// </summary>
        /// <param name="anonymId">The Anonymous User ID to identify the event(s).</param>
        /// <param name="contactId">The Contact ID (Foreign Key) to associate with the event(s).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task BulkSetContactsAsync(Guid anonymId, int contactId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return Context.Database.ExecuteSqlCommandAsync(
                "Logging.SetEventContacts @AnonymId, @ContactId",
                cancellationToken,
                DbParameters.Value("AnonymId", anonymId),
                DbParameters.Value("ContactId", contactId));
        }

        /// <summary>
        /// The DbSet for access to categories in the context.
        /// </summary>
        private DbSet<LoggingCategory> Categories
        {
            get { return Context.Set<LoggingCategory>(); }
        }

        /// <summary>
        /// The DbSet for access to events in the context.
        /// </summary>
        private DbSet<LoggingEvent> Events
        {
            get { return Context.Set<LoggingEvent>(); }
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
