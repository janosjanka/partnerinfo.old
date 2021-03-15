// Copyright (c) János Janka. All rights reserved.

using System;
using System.Composition;
using System.Net.Mail;
using Hangfire;
using Partnerinfo.Input;
using Partnerinfo.Input.HangFire;
using Partnerinfo.Logging;
using Partnerinfo.Project;
using Partnerinfo.Project.Actions;
using Partnerinfo.Project.Mail;

namespace Partnerinfo.Composition
{
    public class HangFireJobActivator : JobActivator
    {
        private readonly CompositionContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="HangFireJobActivator" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public HangFireJobActivator(CompositionContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Activates the job.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public override object ActivateJob(Type type)
        {
            if (type == typeof(ScheduleActionJob))
            {
                return new ScheduleActionJob(_context.GetExport<ProjectManager>(), _context.GetExport<WorkflowInvoker>());
            }

            if (type == typeof(MailJobClient))
            {
                var client = new MailJobClient(new SmtpClient());
                client.LogManager = _context.GetExport<LogManager>();
                return client;
            }

            if (type == typeof(CommandListenerJob))
            {
                return new CommandListenerJob(_context.GetExport<CommandClient>());
            }

            if (type == typeof(CommandCleanerJob))
            {
                return new CommandCleanerJob(_context.GetExport<CommandManager>());
            }

            return null;
        }
    }
}