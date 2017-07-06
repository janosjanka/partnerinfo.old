// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel;
using System.Threading;
using Hangfire;

namespace Partnerinfo.Project.Actions
{
    public sealed class ScheduleActionJob
    {
        private readonly ProjectManager _projectManager;
        private readonly WorkflowInvoker _workflowInvoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleActionJob" /> class.
        /// </summary>
        /// <param name="projectManager">The project manager.</param>
        /// <param name="workflowInvoker">The workflow invoker.</param>
        public ScheduleActionJob(ProjectManager projectManager, WorkflowInvoker workflowInvoker)
        {
            _projectManager = projectManager;
            _workflowInvoker = workflowInvoker;
        }

        [DisplayName("Schedules actions")]
        [AutomaticRetry(Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async void Execute(ScheduleActionJobData data)
        {
            var project = await _projectManager.FindByIdAsync(data.ProjectId, CancellationToken.None);
            if (project == null)
            {
                return;
            }
            var scheduleAction = await _projectManager.GetActionByIdAsync(data.ActionId, CancellationToken.None);
            if (scheduleAction == null || scheduleAction.Children.Count == 0)
            {
                return;
            }

            var collectionAction = new ActionItem
            {
                Id = scheduleAction.Id,
                Parent = scheduleAction.Parent,
                Project = scheduleAction.Project,
                Type = ActionType.Sequence,
                Enabled = scheduleAction.Enabled,
                ModifiedDate = scheduleAction.ModifiedDate,
                Name = scheduleAction.Name,
                Options = scheduleAction.Options
            };

            foreach (var action in scheduleAction.Children)
            {
                collectionAction.Children.Add(action);
            }

            await _workflowInvoker.InvokeAsync(
                new ActionActivityContext(
                    project: project,
                    action: collectionAction,
                    authTicket: data.AuthTicket,
                    contact: data.Contact,
                    contactState: data.ContactState,
                    properties: data.Properties,
                    eventItem: data.Event),
                CancellationToken.None);
        }
    }
}
