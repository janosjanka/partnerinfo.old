// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.IO;

namespace Partnerinfo.Drive.Archives
{
    public interface IPackageArchive
    {
        /// <summary>
        /// Extracts data from the specified input stream.
        /// </summary>
        /// <param name="inputStream">The input stream to extract.</param>
        /// <returns>
        /// An enumerable collection of output streams. The caller must dispose all output streams.
        /// </returns>
        IEnumerable<PackageEntry> Parse(Stream inputStream);
    }
}
