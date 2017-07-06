// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Drive.Archives
{
    public class PackageMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageMetadata"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public PackageMetadata()
        {
        }

        /// <summary>
        /// Gets or sets the format of the archive.
        /// </summary>
        /// <value>
        /// The format of the archive.
        /// </value>
        public PackageFormat Format { get; set; }
    }
}
