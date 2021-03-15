// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Input
{
    public class CommandObject
    {
        /// <summary>
        /// Object type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Object Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Sub object
        /// </summary>
        public CommandObject Object { get; set; }

        /// <summary>
        /// Converts the value of this instance to a <see cref="System.String" />.
        /// </summary>
        /// <returns>
        /// A string whose value is the same as this instance.
        /// </returns>
        public override string ToString() => $"{Type}: {Id}";
    }
}
