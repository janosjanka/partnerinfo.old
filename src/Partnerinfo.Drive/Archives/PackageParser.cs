// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Partnerinfo.Drive.Archives
{
    public class PackageParser : IPackageParser
    {
        private IReadOnlyDictionary<PackageFormat, IPackageArchive> _archives;

        public IEnumerable<Lazy<IPackageArchive, PackageMetadata>> Archives { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageParser"/> class.
        /// </summary>
        public PackageParser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageParser"/> class.
        /// </summary>
        /// <param name="archives">The archives.</param>
        public PackageParser(IDictionary<PackageFormat, IPackageArchive> archives)
        {
            _archives = new ReadOnlyDictionary<PackageFormat, IPackageArchive>(archives);
        }

        /// <summary>
        /// Extracts data from the specified input stream.
        /// </summary>
        /// <param name="inputStream">The input stream to extract.</param>
        /// <param name="format">The format of the input stream.</param>
        /// <returns>
        /// An enumerable collection of output streams. The caller must dispose all output streams.
        /// </returns>
        public IEnumerable<PackageEntry> Parse(Stream inputStream, PackageFormat format)
        {
            if (_archives != null)
            {
                return _archives[format].Parse(inputStream);
            }
            return Enumerable.Empty<PackageEntry>();
        }

        /// <summary>
        /// Called when all imports have been satisfied.
        /// </summary>
        public void OnImportsSatisfied()
        {
            var archives = new Dictionary<PackageFormat, IPackageArchive>();
            foreach (var archive in Archives)
            {
                archives[archive.Metadata.Format] = archive.Value;
            }
            _archives = new ReadOnlyDictionary<PackageFormat, IPackageArchive>(archives);
        }
    }
}
