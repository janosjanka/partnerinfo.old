// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Partnerinfo.Input
{
    public class CommandResult
    {
        public CommandResult()
            : this(CommandStatusCode.Success, Enumerable.Empty<string>())
        {
        }

        public CommandResult(CommandStatusCode statusCode)
            : this(statusCode, Enumerable.Empty<string>())
        {
        }

        public CommandResult(CommandStatusCode statusCode, IEnumerable<string> errors)
        {
            if (errors == null)
            {
                throw new ArgumentNullException("errors");
            }
            StatusCode = statusCode;
            Errors = new List<string>(errors);
        }

        /// <summary>
        /// Status code
        /// </summary>
        public CommandStatusCode StatusCode { get; }

        /// <summary>
        /// Error messages
        /// </summary>
        public IList<string> Errors { get; }
    }
}
