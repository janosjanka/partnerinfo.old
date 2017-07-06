// Copyright (c) János Janka. All rights reserved.

using System;
using System.ComponentModel;
using System.Threading;
using Hangfire;

namespace Partnerinfo.Input.HangFire
{
    public class CommandCleanerJob
    {
        private readonly CommandManager _commandManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCleanerJob" /> class.
        /// </summary>
        /// <param name="commandManager">The command manager.</param>
        public CommandCleanerJob(CommandManager commandManager)
        {
            if (commandManager == null)
            {
                throw new ArgumentNullException(nameof(commandManager));
            }
            _commandManager = commandManager;
        }

        [DisplayName("Removes all the expired commands")]
        [AutomaticRetry(Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async void Execute() => await _commandManager.CleanAsync(CancellationToken.None);
    }
}
