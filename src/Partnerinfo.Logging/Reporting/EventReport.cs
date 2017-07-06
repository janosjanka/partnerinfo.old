// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Logging.Reporting
{
    public class EventReport : IEventReport
    {
        /// <summary>
        /// Generates report from a collection of events
        /// </summary>
        public Task GenerateAsync(Stream stream, IEnumerable<EventResult> events,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            return ExcelReportService.SaveAsync(stream, events, cancellationToken);
        }
    }
}
