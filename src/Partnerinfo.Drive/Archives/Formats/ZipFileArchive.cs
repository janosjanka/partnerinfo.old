// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Partnerinfo.Drive.Archives.Formats
{
    public class ZipFileArchive : IPackageArchive
    {
        /// <summary>
        /// Extracts data from the specified input stream.
        /// </summary>
        /// <param name="inputStream">The input stream to extract.</param>
        /// <returns>
        /// An enumerable collection of output streams. The caller must dispose all output streams.
        /// </returns>
        public IEnumerable<PackageEntry> Parse(Stream inputStream)
        {
            using (var archive = new ZipArchive(inputStream))
            {
                foreach (var entry in archive.Entries)
                {
                    yield return new PackageEntry(entry.Name, () => Open(entry));
                }
            }
        }

        /// <summary>
        /// Opens a ZIP stream.
        /// </summary>
        /// <param name="entry">The entry to open.</param>
        /// <returns>
        /// The opened stream.
        /// </returns>
        private static Stream Open(ZipArchiveEntry entry)
        {
            var stream = new MemoryStream();
            try
            {
                using (var zipStream = entry.Open())
                {
                    zipStream.CopyTo(stream);
                }
                stream.Seek(0, SeekOrigin.Begin);
            }
            catch
            {
                stream.Dispose();
                throw;
            }
            return stream;
        }
    }
}
