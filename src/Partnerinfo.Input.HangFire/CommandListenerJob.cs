// Copyright (c) János Janka. All rights reserved.

using System;
using System.ComponentModel;
using System.Threading;
using Hangfire;

namespace Partnerinfo.Input.HangFire
{
    public class CommandListenerJob
    {
        private readonly CommandClient _commandClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandListenerJob" /> class.
        /// </summary>
        /// <param name="commandClient">The client.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public CommandListenerJob(CommandClient commandClient)
        {
            if (commandClient == null)
            {
                throw new ArgumentNullException(nameof(commandClient));
            }
            _commandClient = commandClient;
        }

        [DisplayName("Listens to incoming commands and executes them")]
        [AutomaticRetry(Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async void Execute() => await _commandClient.ExecuteAsync(CancellationToken.None);
    }
}
