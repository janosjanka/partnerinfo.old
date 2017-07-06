// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Logging
{
    public interface IEventReport
    {
        /// <summary>
        /// Generates report from a collection of events
        /// </summary>
        Task GenerateAsync(Stream stream, IEnumerable<EventResult> events, CancellationToken cancellationToken);
    }
}
