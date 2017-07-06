// Copyright (c) János Janka. All rights reserved.

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Partnerinfo.Drive
{
    /// <summary>
    /// Represents a user document in the file system.
    /// </summary>
    public sealed class DriveFile
    {
        /// <summary>
        /// Represents the name of the root folder.
        /// </summary>
        private const string FolderName = "Documents";

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int UserId { get; }

        /// <summary>
        /// Gets the root directory where the files can be placed.
        /// </summary>
        /// <value>
        /// The root directory.
        /// </value>
        public string ContentRoot { get; }

        /// <summary>
        /// Gets the relative path which includes both folder and filename.
        /// </summary>
        /// <value>
        /// The relative path.
        /// </value>
        public string RelativePath { get; private set; }

        /// <summary>
        /// Gets the absolute path which includes both folder and filename.
        /// </summary>
        /// <value>
        /// The absolute path.
        /// </value>
        public string AbsolutePath { get; private set; }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <returns>
        /// True if exists.
        /// </returns>
        public bool Exists => File.Exists(AbsolutePath);

        /// <summary>
        /// Gets the name of the file of the specified user.
        /// </summary>
        /// <returns>
        /// The full path of the document file.
        /// </returns>
        public DateTime ModifiedDate => File.GetLastWriteTimeUtc(AbsolutePath);

        /// <summary>
        /// Initializes a new instance of the <see cref="DriveFile" /> class.
        /// </summary>
        /// <param name="contentRoot">The content root described the current hosting environment object.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="path">The path.</param>
        /// <exception cref="System.ArgumentNullException">root</exception>
        public DriveFile(string contentRoot, int userId, string path)
        {
            if (contentRoot == null)
            {
                throw new ArgumentNullException(nameof(contentRoot));
            }
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            UserId = userId;
            ContentRoot = Path.Combine(contentRoot, userId.ToString(), FolderName);
            RelativePath = path;
            AbsolutePath = Path.Combine(ContentRoot, path);
        }

        /// <summary>
        /// Asynchronously reads the bytes from the current stream and writes them to a file stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous save operation.
        /// </returns>
        public async Task CreateFileAsync(Stream stream, CancellationToken cancellationToken)
        {
            string dirName = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string fileName = UriUtility.Normalize(Path.GetFileName(RelativePath));
            string filePath = GetFileName(Path.Combine(CreateDirectory(dirName), fileName));

            using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
            {
                await stream.CopyToAsync(fileStream, 81920, cancellationToken);
            }

            RelativePath = Path.Combine(dirName, fileName);
            AbsolutePath = filePath;
        }

        /// <summary>
        /// Deletes the specified document file.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">path</exception>
        public void Delete()
        {
            try
            {
                File.Delete(AbsolutePath);
            }
            catch (IOException)
            {
                // A delete operation should never throw exceptions.
            }
        }

        /// <summary>
        /// Creates a <see cref="FileStream" /> that can be used to read the bytes from the stream.
        /// </summary>
        /// <returns>
        /// The <see cref="FileStream" /> instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public Stream OpenRead() => new FileStream(AbsolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        /// <summary>
        /// Creates the specified directory if that does not exist yet.
        /// </summary>
        /// <param name="path">The directory to create.</param>
        /// <returns>
        /// The created path.
        /// </returns>
        private string CreateDirectory(string path)
        {
            path = Path.Combine(ContentRoot, path);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        /// <summary>
        /// Searches a free file name if the specified name already exists.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>
        /// The path which represents a free file name.
        /// </returns>
        private static string GetFileName(string path)
        {
            string root = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path).ToLower();

            var builder = new StringBuilder();
            for (int i = 0; File.Exists(path = GenerateFileName(builder, root, fileName, extension, i)); ++i) ;
            return path;
        }

        /// <summary>
        /// Generates a random file name using the specified values.
        /// </summary>
        /// <returns>
        /// The file name.
        /// </returns>
        private static string GenerateFileName(StringBuilder builder, string path, string fileName, string extension, int index)
        {
            builder.Clear();
            builder.Append(fileName);
            if (index > 0)
            {
                builder.Append('-');
                builder.Append(DateTime.UtcNow.ToString("hhmmss"));
            }
            builder.Append(extension);
            return Path.Combine(path, builder.ToString());
        }
    }
}
