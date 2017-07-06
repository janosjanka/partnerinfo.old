// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Partnerinfo.Logging;

namespace Partnerinfo.Project.Actions
{
    public sealed class ScheduleActionActivity : IActionActivity
    {
        public sealed class Options
        {
            /// <summary>
            /// Gets or sets the <see cref="DateTime"/> in UTC when this action will be executed.
            /// </summary>
            /// <value>
            /// The <see cref="DateTime"/> in UTC when this action will be executed.
            /// </value>
            public DateTime StartDate { get; set; } = DateTime.UtcNow;

            /// <summary>
            /// Gets or sets the <see cref="TimeSpan"/> that will be added to the <see cref="StartDate"/> property.
            /// </summary>
            /// <value>
            /// The <see cref="TimeSpan"/> that will be added to the <see cref="StartDate"/> property.
            /// </value>
            public TimeSpan OffsetTime { get; set; } = TimeSpan.Zero;
        }

        /// <summary>
        /// Called by the workflow runtime to execute an activity.
        /// </summary>
        /// <param name="context">The <see cref="ActionActivityContext"/> to associate with this activity and execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="ActionActivityResult"/> of the run task, which determines whether the activity remains in the executing state, or transitions to the closed state.
        /// </returns>
        public Task<ActionActivityResult> ExecuteAsync(ActionActivityContext context, CancellationToken cancellationToken)
        {
            if (!context.ContactExists || context.ContactState == ObjectState.Deleted)
            {
                return Task.FromResult(context.CreateResult(ActionActivityStatusCode.Failed));
            }
            var options = context.Action.Options?.ToObject<Options>();
            if (options == null)
            {
                return Task.FromResult(context.CreateResult(ActionActivityStatusCode.Failed));
            }

            // Schedule this action with its children using the given time values
            BackgroundJob.Schedule<ScheduleActionJob>(job => job.Execute(new ScheduleActionJobData
            {
                ActionId = context.Action.Id,
                AuthTicket = context.AuthTicket,
                AnonymId = context.AnonymId,
                Contact = context.Contact,
                ContactState = context.ContactState,
                Properties = context.Properties,
                Event = context.Event
            }),
            new DateTimeOffset(options.StartDate).Add(options.OffsetTime));

            // Break control flow to avoid to execute children actions
            return Task.FromResult(context.CreateResult(ActionActivityStatusCode.Failed));
        }
    }
}
