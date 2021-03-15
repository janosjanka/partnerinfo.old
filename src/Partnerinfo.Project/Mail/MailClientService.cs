// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Partnerinfo.Project.Templating;
using Partnerinfo.Security;

namespace Partnerinfo.Project.Mail
{
    public class MailClientService : IMailClientService
    {
        private ImmutableArray<int> _userIds;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailClientService" /> class.
        /// </summary>
        public MailClientService(SecurityManager securityManager, ProjectManager projectManager)
        {
            if (securityManager == null)
            {
                throw new ArgumentNullException(nameof(securityManager));
            }
            if (projectManager == null)
            {
                throw new ArgumentNullException(nameof(projectManager));
            }
            SecurityManager = securityManager;
            ProjectManager = projectManager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailClientService" /> class.
        /// </summary>
        /// <param name="projectManager">The service that managers projects.</param>
        /// <param name="stringTemplate">The service that interpolates template strings.</param>
        public MailClientService(SecurityManager securityManager, ProjectManager projectManager, StringTemplate stringTemplate)
            : this(securityManager, projectManager)
        {
            if (stringTemplate == null)
            {
                throw new ArgumentNullException(nameof(stringTemplate));
            }
            StringTemplate = stringTemplate;
        }

        /// <summary>
        /// Gets or sets the security manager that that <see cref="MailClientService" /> operates against.
        /// </summary>
        /// <value>
        /// The security manager.
        /// </value>
        protected SecurityManager SecurityManager { get; set; }

        /// <summary>
        /// The service that managers projects
        /// </summary>
        protected ProjectManager ProjectManager { get; set; }

        /// <summary>
        /// The service that interpolates template strings
        /// </summary>
        protected StringTemplate StringTemplate { get; set; }

        /// <summary>
        /// The users who will be notified
        /// </summary>
        protected ImmutableArray<AccountItem> Users { get; set; }

        /// <summary>
        /// Mail body template
        /// </summary>
        public string BodyTemplate { get; set; } = MailResources.Message;

        /// <summary>
        /// The maximum number of concurrent tasks.
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount * 5;

        /// <summary>
        /// Enqueues emails asynchronously.
        /// </summary>
        /// <param name="project">The project that contains the given contacts.</param>
        /// <param name="mailHeader">The mail header that defines contacts and filter criteria.</param>
        /// <param name="mailMessage">The mail message to send.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The task object representing the number of the recipients.
        /// </returns>
        public virtual async Task<ValidationResult> SendAsync(ProjectItem project, MailMessageHeader mailHeader, MailMessageItem mailMessage, CancellationToken cancellationToken)
        {
            var validationResult = await ValidateAsync(project, mailHeader, mailMessage, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            var from = await GetSenderAsync(project, mailHeader, cancellationToken);
            if (from == null)
            {
                throw new InvalidOperationException("The sender of the mail message is not specified.");
            }
            var to = await GetRecipientsAsync(project, mailHeader, cancellationToken);
            if (to.Any())
            {
                Users = await GetUsersAsync(project, cancellationToken);
                _userIds = Users.Select(u => u.Id).ToImmutableArray();
                await EqueueAllAsync(project, from, to, mailMessage, mailHeader.Placeholders, cancellationToken);
            }
            return ValidationResult.Success;
        }

        /// <summary>
        /// Validates the specified <paramref name="mailHeader"/> and <paramref name="mailMessage"/> as an asynchronous operation.
        /// </summary>
        /// <param name="project">The project that contains the given contacts.</param>
        /// <param name="mailHeader">The mail header that defines contacts and filter criteria.</param>
        /// <param name="mailMessage">The mail message to send.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The task object representing the number of the recipients.
        /// </returns>
        public virtual Task<ValidationResult> ValidateAsync(ProjectItem project, MailMessageHeader mailHeader, MailMessageItem mailMessage, CancellationToken cancellationToken)
        {
            if (project == null)
            {
                return Task.FromResult(ValidationResult.Failed(string.Format(CultureInfo.CurrentCulture, MailResources.MailProjectRequired)));
            }
            if (mailHeader == null)
            {
                return Task.FromResult(ValidationResult.Failed(string.Format(CultureInfo.CurrentCulture, MailResources.MailHeaderRequired)));
            }
            if (mailHeader.To.Count == 0 && mailHeader.IncludeWithTags.Count == 0 && mailHeader.ExcludeWithTags.Count == 0)
            {
                return Task.FromResult(ValidationResult.Failed(string.Format(CultureInfo.CurrentCulture, MailResources.MailFilterPredicateRequired)));
            }
            if (mailMessage == null)
            {
                return Task.FromResult(ValidationResult.Failed(string.Format(CultureInfo.CurrentCulture, MailResources.MailMessageRequired)));
            }
            if (string.IsNullOrWhiteSpace(mailMessage.Subject))
            {
                return Task.FromResult(ValidationResult.Failed(string.Format(CultureInfo.CurrentCulture, MailResources.MailMessageSubjectRequired)));
            }
            return Task.FromResult(ValidationResult.Success);
        }

        /// <summary>
        /// Gets all the users that satisfy the given criteria.
        /// </summary>
        /// <param name="project">The project that contains the given contacts.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        protected virtual async Task<ImmutableArray<AccountItem>> GetUsersAsync(ProjectItem project, CancellationToken cancellationToken)
        {
            Debug.Assert(project != null);

            return (await SecurityManager.GetAccessRulesAsync(project, cancellationToken))
                .Data
                .Where(ace => !ace.Anyone)
                .Select(ace => ace.User)
                .ToImmutableArray();
        }

        /// <summary>
        /// Gets all the contacts that satisfy the given criteria.
        /// </summary>
        /// <param name="project">The project that contains the given contacts.</param>
        /// <param name="mailHeader">The message header.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        protected virtual Task<ContactItem> GetSenderAsync(ProjectItem project, MailMessageHeader mailHeader, CancellationToken cancellationToken)
        {
            Debug.Assert(project != null);
            Debug.Assert(mailHeader != null);

            if (mailHeader.From != null)
            {
                return Task.FromResult(mailHeader.From);
            }
            return Task.FromResult(new ContactItem { Email = project.Sender });
        }

        /// <summary>
        /// Gets all the contacts that satisfy the given criteria.
        /// </summary>
        /// <param name="project">The project that contains the given contacts.</param>
        /// <param name="mailHeader">The message header.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        protected virtual async Task<ImmutableArray<ContactItem>> GetRecipientsAsync(ProjectItem project, MailMessageHeader mailHeader, CancellationToken cancellationToken)
        {
            Debug.Assert(project != null);
            Debug.Assert(mailHeader != null);

            var contacts = new HashSet<ContactItem>(UniqueItemEqualityComparer.Default);

            if (mailHeader.To.Any())
            {
                contacts.UnionWith(
                    await ProjectManager.GetContactsAsync(project,
                        mailHeader.To, ContactSortOrder.None, ContactField.None, cancellationToken));
            }

            if (mailHeader.IncludeWithTags.Any() || mailHeader.ExcludeWithTags.Any())
            {
                contacts.UnionWith(
                    (await ProjectManager.GetContactsAsync(project,
                        null, mailHeader.IncludeWithTags, mailHeader.ExcludeWithTags, ContactSortOrder.None, 0, int.MaxValue, ContactField.None, cancellationToken)).Data);
            }

            return contacts.Where(c => c.Email?.Address != null).ToImmutableArray();
        }

        /// <summary>
        /// Asynchronously enqueues a mail message.
        /// </summary>
        /// <param name="from">The sender of the mail message.</param>
        /// <param name="to">The recipients of the mail message.</param>
        /// <param name="mailMessage">The mail message to enqueue.</param>
        /// <param name="placeholders">A map of key/value pairs that can be used to interpolate the content of the mail message.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        protected virtual Task EqueueAllAsync(ProjectItem project, ContactItem from, ImmutableArray<ContactItem> to, MailMessageItem mailMessage, PropertyDictionary placeholders, CancellationToken cancellationToken)
        {
            Debug.Assert(project != null);
            Debug.Assert(from != null);
            Debug.Assert(to != null);
            Debug.Assert(mailMessage != null);

            return Task.WhenAll(
                from partition in Partitioner.Create(to).GetPartitions(MaxDegreeOfParallelism)
                select Task.Run(async () =>
                {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await EnqueueAsync(project, @from, partition.Current, mailMessage, placeholders, cancellationToken);
                        }
                    }
                },
                cancellationToken));
        }

        /// <summary>
        /// Enqueues a mail message.
        /// </summary>
        protected virtual async Task EnqueueAsync(ProjectItem project, ContactItem from, ContactItem to, MailMessageItem mailMessage, PropertyDictionary placeholders, CancellationToken cancellationToken)
        {
            Debug.Assert(project != null);
            Debug.Assert(from != null);
            Debug.Assert(to != null);
            Debug.Assert(mailMessage != null);
            Debug.Assert(placeholders != null);

            bool hasSubject = !string.IsNullOrEmpty(mailMessage.Subject);
            bool hasContent = !string.IsNullOrEmpty(mailMessage.Body);
            string subject = null;
            string content = null;

            if (hasSubject || hasContent)
            {
                placeholders = new PropertyDictionary(placeholders);
                placeholders.AddIfNotExists(StringTemplate.IdentityIdProperty, to.Id);
                placeholders.AddIfNotExists(MailResources.MailHeaderFrom, from);
                placeholders.AddIfNotExists(MailResources.MailHeaderTo, to);

                var immutablePlaceholders = placeholders.ToImmutableDictionary();

                if (hasSubject)
                {
                    subject = await StringTemplate.InterpolateAsync(mailMessage.Subject, immutablePlaceholders, cancellationToken);
                }
                if (hasContent)
                {
                    content = await StringTemplate.InterpolateAsync(mailMessage.Body, immutablePlaceholders, cancellationToken);
                }
            }

            BackgroundJob.Enqueue<MailJobClient>(job => job.Send(
                new MailJobMessage
                {
                    MessageId = mailMessage.Id,
                    ProjectId = project.Id,
                    From = new MailJobAddress(from.Id, from.Email.Address, from.Email.Name),
                    To = new MailJobAddress(to.Id, to.Email.Address, to.Email.Name),
                    Subject = subject,
                    Body = content,
                    Users = _userIds
                }));
        }
    }
}
