// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Partnerinfo.Logging;

namespace Partnerinfo.Project.Actions
{
    public sealed class ConditionActionActivity : IActionActivity
    {
        public sealed class Options
        {
            [JsonConverter(typeof(ListJsonConverter<ConditionItem>))]
            public IList<ConditionItem> Conditions { get; set; }
        }

        public sealed class ConditionItem
        {
            /// <summary>
            /// Condition type
            /// </summary>
            public ConditionType Type { get; set; }

            /// <summary>
            /// Condition value
            /// </summary>
            public string Value { get; set; }
        }

        public enum ConditionType
        {
            /// <summary>
            /// Indicates that the action type is not being used.
            /// </summary>
            Unknown = 0,

            // System ----------------------------------------------------------------

            /// <summary>
            /// Checks whether the system time is greater than or equal to the given value.
            /// </summary>
            DateGreaterThanOrEqualTo = 1,

            /// <summary>
            /// Checks whether the system time is less than or equal to the given value.
            /// </summary>
            DateLessThanOrEqualTo = 2,

            // Contact ---------------------------------------------------------------

            /// <summary>
            /// Checks whether the session is authenticated
            /// </summary>
            Authenticated = 50,

            /// <summary>
            /// Checks whether the contact exists
            /// </summary>
            ContactExists = 51,

            /// <summary>
            /// Checks whether the tag is associated with the contact
            /// </summary>
            ContactWithTag = 52,

            /// <summary>
            /// Checks whether the tag is not associated with the contact
            /// </summary>
            ContactWithoutTag = 53
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
            var options = context.Action.Options?.ToObject<Options>();
            if (options == null || options.Conditions == null)
            {
                return context.CreateResult(ActionActivityStatusCode.Failed);
            }

            bool succeeded = true;
            var includeWithTagIds = new HashSet<int>();
            var excludeWithTagIds = new HashSet<int>();
            foreach (var condition in options.Conditions)
            {
                switch (condition?.Type)
                {
                    case ConditionType.DateGreaterThanOrEqualTo:
                        {
                            DateTime value;
                            succeeded &= DateTime.TryParse(condition.Value, out value) ? (DateTime.UtcNow >= value) : false;
                        }
                        break;
                    case ConditionType.DateLessThanOrEqualTo:
                        {
                            DateTime value;
                            succeeded &= DateTime.TryParse(condition.Value, out value) ? (DateTime.UtcNow <= value) : false;
                        }
                        break;
                    case ConditionType.Authenticated:
                        {
                            bool value;
                            succeeded &= bool.TryParse(condition.Value, out value) ? context.IsAuthenticated == value : false;
                        }
                        break;
                    case ConditionType.ContactExists:
                        {
                            bool value;
                            succeeded &= bool.TryParse(condition.Value, out value) ? context.ContactExists == value : false;
                        }
                        break;
                    case ConditionType.ContactWithTag:
                        {
                            int value;
                            if (succeeded &= int.TryParse(condition.Value, out value))
                            {
                                includeWithTagIds.Add(value);
                            }
                        }
                        break;
                    case ConditionType.ContactWithoutTag:
                        {
                            int value;
                            if (succeeded &= int.TryParse(condition.Value, out value))
                            {
                                excludeWithTagIds.Add(value);
                            }
                        }
                        break;
                    default:
                        succeeded = false;
                        break;
                }
                if (!succeeded)
                {
                    return context.CreateResult(ActionActivityStatusCode.Failed);
                }
            }

            if (context.ContactExists
                && context.ContactState != ObjectState.Deleted
                && (includeWithTagIds.Count | excludeWithTagIds.Count) > 0)
            {
                var projectManager = context.Resolve<ProjectManager>();
                succeeded &= await projectManager.HasBusinessTagsAsync(
                    context.Contact, includeWithTagIds, excludeWithTagIds, cancellationToken);
            }

            return context.CreateResult(succeeded ? ActionActivityStatusCode.Success : ActionActivityStatusCode.Failed);
        }
    }
}
