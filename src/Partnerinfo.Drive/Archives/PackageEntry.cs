// Copyright (c) János Janka. All rights reserved.

using System;
using System.IO;

namespace Partnerinfo.Drive.Archives
{
    public sealed class PackageEntry
    {
        private readonly Func<Stream> _streamAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageEntry" /> class.
        /// </summary>
        public PackageEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageEntry" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="streamAccessor">The stream.</param>
        public PackageEntry(string name, Func<Stream> streamAccessor)
        {
            Name = name;
            _streamAccessor = streamAccessor;
        }

        /// <summary>
        /// Gets the name of the current package item.
        /// </summary>
        /// <value>
        /// The name of the current package item.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the length of the current package item.
        /// </summary>
        /// <value>
        /// The length of the current package item.
        /// </value>
        public long Length { get; private set; }

        /// <summary>
        /// Opens the entry from the package.
        /// </summary>
        /// <returns>
        /// The stream.
        /// </returns>
        public Stream Open()
        {
            var stream = _streamAccessor();
            if (stream != null)
            {
                Length = stream.Length;
            }
            return stream;
        }
    }
}
