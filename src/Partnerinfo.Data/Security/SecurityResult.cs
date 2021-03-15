// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Security
{
    public struct SecurityResult
    {
        /// <summary>
        /// A <see cref="SecurityResult" /> to refuse to take any further action. It corresponds to HTTP 403 Forbidden.
        /// </summary>
        public static readonly SecurityResult AccessDenied = new SecurityResult(false, AccessVisibility.Unknown);

        /// <summary>
        /// Gets or sets a value indicating whether this instance has permission.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has permission; otherwise, <c>false</c>.
        /// </value>
        public bool AccessGranted { get; }

        /// <summary>
        /// Gets the visibility for an ACE (Access Control Entry).
        /// </summary>
        /// <value>
        /// The visibility for an ACE (Access Control Entry).
        /// </value>
        public AccessVisibility Visibility { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityResult"/> struct.
        /// </summary>
        /// <param name="accessGranted">if set to <c>true</c> [allowed].</param>
        /// <param name="visibility">The visibility.</param>
        public SecurityResult(bool accessGranted, AccessVisibility visibility)
        {
            AccessGranted = accessGranted;
            Visibility = visibility;
        }
    }
}
