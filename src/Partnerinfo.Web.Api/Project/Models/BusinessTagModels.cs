// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Project.Models
{
    public sealed class BusinessTagItemDto
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Color
        /// </summary>
        public string Color { get; set; }
    }

    public sealed class BusinessTagResultDto
    {
        /// <summary>
        /// Identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Color
        /// </summary>
        public string Color { get; set; }
    }
}