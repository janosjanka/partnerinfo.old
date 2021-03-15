// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Partnerinfo.Drive.Archives;
using Partnerinfo.Security;

namespace Partnerinfo.Drive
{
    public class DriveManager
    {
        private readonly SecurityManager _securityManager;
        private readonly IPersistenceServices _services;
        private readonly string _contentRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriveManager" /> class.
        /// </summary>
        /// <param name="securityManager">The security manager.</param>
        /// <param name="services">The services.</param>
        /// <param name="contentRoot">The root.</param>
        /// <exception cref="System.ArgumentNullException">
        /// securityManager
        /// or
        /// services
        /// or
        /// root
        /// </exception>
        public DriveManager(SecurityManager securityManager, IPersistenceServices services, string contentRoot)
        {
            if (securityManager == null)
            {
                throw new ArgumentNullException(nameof(securityManager));
            }
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (contentRoot == null)
            {
                throw new ArgumentNullException(nameof(contentRoot));
            }

            _securityManager = securityManager;
            _services = services;
            _contentRoot = contentRoot;
        }

        /// <summary>
        /// Adds a document to the database.
        /// </summary>
        /// <param name="file">The file to create.</param>
        /// <param name="ownerId">The unique user identifier from the data source for the user.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">file</exception>
        public virtual async Task<FileItem> CreateAsync(FileItem file, int ownerId, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            if (file.ParentId != null)
            {
                await ExpectTypeAsync(file.ParentId.Value, FileType.Folder, cancellationToken);
            }

            file = await AddAsync(file, ownerId, cancellationToken);
            await _services.SaveAsync(cancellationToken);
            await _securityManager.SetAccessRuleAsync(AccessObjectType.File, file.Id, new AccessRuleItem { User = new AccountItem { Id = ownerId }, Permission = AccessPermission.IsOwner }, cancellationToken);
            return file;
        }

        /// <summary>
        /// Asynchronously adds a file to the database.
        /// </summary>
        /// <param name="stream">An instance of the <see cref="Stream" /> class for reading data from the file resource.</param>
        /// <param name="parentId">The unique file identifier from the data source for the file.</param>
        /// <param name="ownerId">The unique user identifier from the data source for the user.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">stream
        /// or
        /// fileName</exception>
        public virtual async Task<FileItem> CreateFromStreamAsync(Stream stream, int? parentId, int ownerId, string fileName, CancellationToken cancellationToken)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var file = new DriveFile(_contentRoot, ownerId, fileName);
            await file.CreateFileAsync(stream, cancellationToken);

            return await CreateAsync(
                new FileItem
                {
                    ParentId = parentId,
                    Type = FileType.File,
                    Name = Path.GetFileName(fileName),
                    PhysicalPath = file.RelativePath,
                    Length = (int)stream.Length
                },
                ownerId,
                cancellationToken);
        }

        /// <summary>
        /// Asynchronously adds a package file to the database.
        /// </summary>
        /// <param name="stream">An instance of the <see cref="Stream" /> class for reading data from the package resource.</param>
        /// <param name="parentId">The unique file identifier from the data source for the file.</param>
        /// <param name="ownerId">The unique user identifier from the data source for the user.</param>
        /// <param name="format">The package format.</param>
        /// <param name="parser">The parser.</param>
        /// <returns>
        /// A task that represents the asynchronous create operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public virtual async Task<IList<FileItem>> CreateFromStreamAsync(Stream stream, int? parentId, int ownerId, PackageFormat format, IPackageParser parser, CancellationToken cancellationToken)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            var list = new List<FileItem>();
            var entries = parser.Parse(stream, format);

            foreach (PackageEntry entry in entries)
            {
                using (Stream entryStream = entry.Open())
                {
                    list.Add(await CreateFromStreamAsync(entryStream, parentId, ownerId, entry.Name, cancellationToken));
                }
            }

            return list;
        }

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="file">The file to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual Task<ValidationResult> DeleteAsync(FileItem file, CancellationToken cancellationToken)
        {
            if (file.Type == FileType.File)
            {
                new DriveFile(_contentRoot, file.OwnerId, file.PhysicalPath).Delete();
            }

            return _services.Drive.DeleteAsync(file, cancellationToken);
        }

        /// <summary>
        /// Deletes the file with physical data.
        /// </summary>
        /// <param name="id">The unique file identifier from the data source for the file.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var file = await _services.Drive.FindByIdAsync(id, cancellationToken);
            if (file == null)
            {
                return;
            }

            await DeleteAsync(file, cancellationToken);
            await _services.SaveAsync(cancellationToken);
        }

        /// <summary>
        /// Deletes the specified files.
        /// </summary>
        /// <param name="ids">A collection of unique file identifiers.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public virtual async Task DeleteAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            var files = await _services.Drive.FindAllAsync(ids, cancellationToken);
            try
            {
                foreach (FileItem file in files)
                {
                    await DeleteAsync(file, cancellationToken);
                }
            }
            finally
            {
                await _services.SaveAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Gets the package format for the specified file type.
        /// </summary>
        /// <param name="fileExt">The file extension to parse.</param>
        /// <returns>
        /// The <see cref="PackageFormat" /> value.
        /// </returns>
        public PackageFormat GetPackageFormat(string fileExt)
        {
            if (string.IsNullOrEmpty(fileExt))
            {
                return PackageFormat.None;
            }
            if (string.Equals(fileExt, ".zip", StringComparison.OrdinalIgnoreCase))
            {
                return PackageFormat.ZipArchive;
            }
            return PackageFormat.None;
        }

        /// <summary>
        /// Creates a new file and adds it to the context without saving changes.
        /// </summary>
        /// <param name="file">The file to add.</param>
        /// <param name="ownerId">The unique user identifier from the data source for the user.</param>
        /// <returns>
        /// A <see cref="FileItem" /> instance if the operation was successful; otherwise <c>null</c>.
        /// </returns>
        protected virtual async Task<FileItem> AddAsync(FileItem file, int ownerId, CancellationToken cancellationToken)
        {
            await _services.Drive.CreateAsync(file, cancellationToken);
            file.OwnerId = ownerId;
            file.Slug = UriUtility.EncodeRandomUriToken(32);
            return file;
        }

        /// <summary>
        /// Ensures a folder type of the file.
        /// </summary>
        /// <param name="fileId">The file id.</param>
        /// <param name="type">The type.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Invalid file type.</exception>
        private async Task ExpectTypeAsync(int fileId, FileType type, CancellationToken cancellationToken)
        {
            var file = await _services.Drive.FindByIdAsync(fileId, cancellationToken);
            if (file == null)
            {
                throw new InvalidOperationException($"File (${fileId}) was not found.");
            }
            if (file.Type != type)
            {
                throw new InvalidOperationException($"Invalid file type: ${type}.");
            }
        }
    }
}
