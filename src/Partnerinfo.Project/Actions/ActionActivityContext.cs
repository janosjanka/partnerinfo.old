// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using Partnerinfo.Logging;

namespace Partnerinfo.Project.Actions
{
    public sealed class ActionActivityContext : ICloneable
    {
        private ObjectState _contactState;

        private ActionActivityContext() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionActivityContext" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public ActionActivityContext(
            ProjectItem project,
            ActionItem action,
            AuthTicket authTicket = null,
            ContactItem contact = null,
            ObjectState contactState = ObjectState.Unchanged,
            PropertyDictionary properties = null,
            EventItem eventItem = null)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Project = project;
            RootAction = Action = action;
            AuthTicket = authTicket;
            Event = eventItem ?? new EventItem();
            Event.ObjectType = ObjectType.Action;
            Event.ObjectId = action.Id;
            Event.Project = project;
            AnonymId = Event.AnonymId;
            Contact = contact;
            ContactState = contactState;
            Properties = properties == null ? new PropertyDictionary() : new PropertyDictionary(properties);
            Errors = new List<string>();
        }

        /// <summary>
        /// Dependency Resolver
        /// </summary>
        public IServiceResolver Resolver { get; set; }

        /// <summary>
        /// Project
        /// </summary>
        public ProjectItem Project { get; private set; }

        /// <summary>
        /// Root action
        /// </summary>
        public ActionItem RootAction { get; private set; }

        /// <summary>
        /// Action will be executed
        /// </summary>
        public ActionItem Action { get; private set; }

        /// <summary>
        /// Access token
        /// </summary>
        public AuthTicket AuthTicket { get; set; }

        /// <summary>
        /// User ID which identifiers an anonymous user
        /// </summary>
        public Guid? AnonymId { get; set; }

        /// <summary>
        /// Contact data
        /// </summary>
        public ContactItem Contact { get; set; }

        /// <summary>
        /// Contact state
        /// </summary>
        public ObjectState ContactState
        {
            get
            {
                return _contactState;
            }

            set
            {
                // An added/deleted entity cannot be modified
                if (value == ObjectState.Modified && (
                    _contactState == ObjectState.Added ||
                    _contactState == ObjectState.Deleted))
                {
                    return;
                }

                _contactState = value;
            }
        }

        /// <summary>
        /// Returns true if the contact exists
        /// </summary>
        public bool ContactExists => Contact?.Id > 0;

        /// <summary>
        /// Other properties that can be injected to the pipeline
        /// </summary>
        public PropertyDictionary Properties { get; private set; }

        /// <summary>
        /// Event object
        /// </summary>
        public EventItem Event { get; private set; }

        /// <summary>
        /// Error messages
        /// </summary>
        public IList<string> Errors { get; private set; }

        /// <summary>
        /// Returns true if the contact is authenticated
        /// </summary>
        public bool IsAuthenticated => AuthTicket != null;

        /// <summary>
        /// Returns true if the action tree contains at least one log action (required for backward-compatibility)
        /// </summary>
        internal bool HasLogAction { get; set; }

        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <returns>
        /// The requested service or object.
        /// </returns>
        public T Resolve<T>() where T : class
        {
            if (Resolver == null)
            {
                throw new InvalidOperationException("A dependency resolver must be registered for the acitvity context.");
            }
            var resolver = Resolver.Resolve(typeof(T)) as T;
            if (resolver == null)
            {
                throw new InvalidOperationException($"Type is not registered: {typeof(T).Name}.");
            }
            return resolver;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return Clone(null);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public ActionActivityContext Clone(ActionItem current)
        {
            return new ActionActivityContext
            {
                Resolver = Resolver,
                AnonymId = AnonymId,
                Event = Event,
                Project = Project,
                RootAction = RootAction ?? current,
                Action = current ?? Action,
                AuthTicket = AuthTicket,
                Contact = Contact,
                ContactState = ContactState,
                Properties = Properties,
                Errors = Errors,
                HasLogAction = HasLogAction
            };
        }

        /// <summary>
        /// Creates an activity result passing the current contact state to the result object.
        /// </summary>
        /// <param name="statusCode">The new status code.</param>
        /// <returns>
        /// The result object.
        /// </returns>
        public ActionActivityResult CreateResult(ActionActivityStatusCode statusCode)
        {
            return new ActionActivityResult
            {
                StatusCode = statusCode,
                AnonymId = AnonymId,
                Event = Event,
                Ticket = AuthTicket,
                Contact = Contact,
                ContactState = ContactState,
                Errors = Errors,
                HasLogAction = HasLogAction
            };
        }
    }
}
