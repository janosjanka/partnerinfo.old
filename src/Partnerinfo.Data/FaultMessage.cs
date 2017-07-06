// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Partnerinfo
{
    public sealed class FaultMessage
    {
        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        public string Culture { get; } = Thread.CurrentThread.CurrentUICulture.Name;

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        public ImmutableArray<FaultMember> Members { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FaultMessage" /> class.
        /// </summary>
        public FaultMessage()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FaultMessage" /> class.
        /// </summary>
        public FaultMessage(string message)
            : this(message, Enumerable.Empty<FaultMember>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FaultMessage" /> class.
        /// </summary>
        public FaultMessage(IEnumerable<FaultMember> members)
            : this(null, members)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FaultMessage" /> class.
        /// </summary>
        public FaultMessage(string message, IEnumerable<FaultMember> members)
        {
            if (members == null)
            {
                throw new ArgumentNullException(nameof(members));
            }
            Members = members == null ? ImmutableArray<FaultMember>.Empty : members.ToImmutableArray();
            Message = message ?? string.Join(Environment.NewLine, Members.Select(e => e.Message));
        }
    }
}