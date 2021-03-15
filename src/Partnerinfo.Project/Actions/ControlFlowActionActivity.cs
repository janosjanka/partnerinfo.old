// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Partnerinfo.Project.Actions
{
    public sealed class ControlFlowActionActivity : IActionActivity
    {
        private readonly ImmutableDictionary<ActionType, ActionDescriptor> _actions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlFlowActionActivity" /> class.
        /// </summary>
        /// <param name="actions">A collection of action activities.</param>
        /// <exception cref="System.ArgumentNullException">actions</exception>
        public ControlFlowActionActivity(IEnumerable<Lazy<IActionActivity, IDictionary<string, object>>> actions)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            object value;
            var dictionary = new Dictionary<ActionType, ActionDescriptor>();
            foreach (var action in actions)
            {
                if (action.Metadata.TryGetValue("Action", out value))
                {
                    var metadata = value as ActionActivityMetadata;
                    if (metadata != null)
                    {
                        dictionary[metadata.Type] = new ActionDescriptor(action.Value, metadata);
                    }
                }
            }
            _actions = ImmutableDictionary.ToImmutableDictionary(dictionary);
        }

        /// <summary>
        /// Called by the workflow runtime to execute an activity.
        /// </summary>
        /// <param name="context">The <see cref="ActionActivityContext"/> to associate with this activity and execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="ActionActivityResult"/> of the run task, which determines whether the activity remains in the executing state, or transitions to the closed state.
        /// </returns>
        public async Task<ActionActivityResult> ExecuteAsync(ActionActivityContext context, CancellationToken cancellationToken)
        {
            if (context.Contact != null)
            {
                var contact = await FindContactAsync(context, cancellationToken);
                if (contact != null)
                {
                    PatchContact(contact, context.Contact);
                    context.Contact = contact;
                }
            }

            var result = await ExecuteNodeAsync(context, cancellationToken);
            if (result == null)
            {
                throw new InvalidOperationException("Action result is required.");
            }

            // Preserve backward-compatibility and log each event received from old actions
            // without having an effect on the action result
            if (!result.HasLogAction)
            {
                await ExecuteNodeAsync(context.Clone(new ActionItem
                {
                    Type = ActionType.Log,
                    Enabled = true,
                    Options = JObject.FromObject(new { Anonymous = true })
                }),
                cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Merges contact data with DB contact data
        /// </summary>
        private static async Task<ContactItem> FindContactAsync(ActionActivityContext context, CancellationToken cancellationToken)
        {
            var projectManager = context.Resolve<ProjectManager>();
            var contact = default(ContactItem);

            if (context.Contact.Id > 0)
            {
                contact = await projectManager.GetContactByIdAsync(context.Contact.Id, ContactField.None, cancellationToken);
            }

            if (contact == null && context.Contact.FacebookId != null)
            {
                contact = await projectManager.GetContactByFacebookIdAsync(context.Project, (long)context.Contact.FacebookId, ContactField.None, cancellationToken);
            }

            if (contact == null && context.Contact.Email != null && context.Contact.Email.Address != null)
            {
                contact = await projectManager.GetContactByMailAsync(context.Project, context.Contact.Email.Address, ContactField.None, cancellationToken);
            }

            return contact;
        }

        private static void PatchContact(ContactItem contact, ContactItem contactData)
        {
            if (contact.Sponsor == null)
            {
                contact.Sponsor = contactData.Sponsor ?? contact.Sponsor;
            }

            contact.Email = contact.Email ?? MailAddressItem.None;
            contact.NickName = contactData.NickName ?? contact.NickName;
            contact.FirstName = contactData.FirstName ?? contact.FirstName;
            contact.LastName = contactData.LastName ?? contact.LastName;
            contact.Gender = contactData.Gender != PersonGender.Unknown ? contactData.Gender : contact.Gender;
            contact.Birthday = contactData.Birthday ?? contact.Birthday;
            contact.Phones = contact.Phones ?? PhoneGroupItem.Empty;
            contact.Comment = contactData.Comment ?? contact.Comment;

            if (contactData.Email != null)
            {
                contact.Email = MailAddressItem.Create(
                    address: contactData.Email.Address ?? contact.Email.Address,
                    name: contactData.Email.Name ?? contact.Email.Name);
            }

            if (contactData.Phones != null)
            {
                contact.Phones = PhoneGroupItem.Create(
                    personal: contactData.Phones.Personal ?? contact.Phones.Personal,
                    business: contactData.Phones.Business ?? contact.Phones.Business,
                    mobile: contactData.Phones.Mobile ?? contact.Phones.Mobile,
                    other: contactData.Phones.Other ?? contact.Phones.Other);
            }
        }

        /// <summary>
        /// Invokes an activity.
        /// </summary>
        /// <param name="context">The <see cref="ActionActivityContext" /> to associate with this activity and execution.</param>
        /// <returns>
        /// The <see cref="ActionActivityResult" /> of the run task, which determines whether the activity remains in the executing state, or transitions to the closed state.
        /// </returns>
        private async Task<ActionActivityResult> ExecuteNodeAsync(ActionActivityContext context, CancellationToken cancellationToken)
        {
            Debug.Assert(context != null);

            if (context.Action.Type == ActionType.Log)
            {
                context.HasLogAction = true;
            }
            if (!context.Action.Enabled)
            {
                return context.CreateResult(ActionActivityStatusCode.Failed);
            }

            var descriptor = FindActionDescriptor(context.Action.Type);
            if (descriptor == null)
            {
                return context.CreateResult(ActionActivityStatusCode.Failed);
            }
            var status = await descriptor.Activity.ExecuteAsync(context, cancellationToken);
            if (status.StatusCode == ActionActivityStatusCode.Success && context.Action.Children.Count > 0)
            {
                status = await ExecuteNodeListAsync(context, context.Action.Children.OfType<ActionItem>(), cancellationToken);
            }
            return status;
        }

        /// <summary>
        /// Invokes an activity.
        /// </summary>
        /// <param name="context">The <see cref="ActionActivityContext" /> to associate with this activity and execution.</param>
        /// <param name="actions">The actions.</param>
        /// <returns>
        /// The <see cref="ActionActivityResult" /> of the run task, which determines whether the activity remains in the executing state, or transitions to the closed state.
        /// </returns>
        private async Task<ActionActivityResult> ExecuteNodeListAsync(ActionActivityContext context, IEnumerable<ActionItem> actions, CancellationToken cancellationToken)
        {
            Debug.Assert(context != null);
            Debug.Assert(actions != null);

            ActionActivityResult status = null;
            foreach (var action in actions)
            {
                context = context.Clone(action);
                status = await ExecuteNodeAsync(context, cancellationToken);
                if (status.StatusCode == ActionActivityStatusCode.Success && status.ReturnUrl != null ||
                    status.StatusCode == ActionActivityStatusCode.Forbidden)
                {
                    return status;
                }
            }
            return status ?? context.CreateResult(ActionActivityStatusCode.Failed);
        }

        /// <summary>
        /// Finds an action for the given type. If the given type is not registered, then it returns null.
        /// </summary>
        /// <param name="type">The type of the action.</param>
        /// <returns>
        /// The descriptor object.
        /// </returns>
        private ActionDescriptor FindActionDescriptor(ActionType type)
        {
            if (type != ActionType.Unknown)
            {
                ActionDescriptor descriptor = null;
                _actions.TryGetValue(type, out descriptor);
                return descriptor;
            }
            return null;
        }
    }
}
