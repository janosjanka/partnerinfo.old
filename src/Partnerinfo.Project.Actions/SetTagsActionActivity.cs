// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Partnerinfo.Logging;

namespace Partnerinfo.Project.Actions
{
    public sealed class SetTagsActionActivity : IActionActivity
    {
        public sealed class Options
        {
            /// <summary>
            /// Gets or sets a list of <see cref="BusinessTagItem"/> identifiers to be added to the contact.
            /// </summary>
            /// <value>
            /// A list of <see cref="BusinessTagItem"/> identifiers to be added to the contact.
            /// </value>
            [JsonConverter(typeof(ListJsonConverter<int>))]
            public IList<int> Include { get; set; }

            /// <summary>
            /// Gets or sets a list of <see cref="BusinessTagItem"/> identifiers to be removed from the contact.
            /// </summary>
            /// <value>
            /// A list of <see cref="BusinessTagItem"/> identifiers to be removed from the contact.
            /// </value>
            [JsonConverter(typeof(ListJsonConverter<int>))]
            public IList<int> Exclude { get; set; }
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
            var projectManager = context.Resolve<ProjectManager>();
            if (!context.ContactExists || context.ContactState == ObjectState.Deleted)
            {
                return context.CreateResult(ActionActivityStatusCode.Failed);
            }
            var options = context.Action.Options?.ToObject<Options>();
            if (options == null)
            {
                return context.CreateResult(ActionActivityStatusCode.Failed);
            }
            await projectManager.SetBusinessTagsAsync(
                new[] { context.Contact.Id },
                options.Include ?? Enumerable.Empty<int>(),
                options.Exclude ?? Enumerable.Empty<int>(),
                cancellationToken);
            context.ContactState = ObjectState.Modified;
            return context.CreateResult(ActionActivityStatusCode.Success);
        }
    }
}
