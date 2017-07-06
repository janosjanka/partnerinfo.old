// Copyright (c) János Janka. All rights reserved.

using System;
using System.IO;

namespace System.Net.Http
{
    internal sealed class FileStreamContent : IDisposable
    {
        private string _fileName;
        private Stream _fileStream;
        private string _fileExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStreamContent"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileStream">The file stream.</param>
        public FileStreamContent(string fileName, Stream fileStream)
        {
            _fileName = fileName;
            _fileStream = fileStream;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// Gets the file stream to read.
        /// </summary>
        /// <value>
        /// The file stream to read.
        /// </value>
        public Stream FileStream
        {
            get { return _fileStream; }
        }

        /// <summary>
        /// Gets the extension of the filename.
        /// </summary>
        /// <value>
        /// The extension of the filename.
        /// </value>
        public string FileExtension
        {
            get
            {
                if (_fileExtension == null)
                {
                    _fileExtension = Path.GetExtension(_fileName);
                }

                return _fileExtension;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_fileStream != null)
            {
                _fileStream.Dispose();
                _fileStream = null;
            }
        }
    }
}