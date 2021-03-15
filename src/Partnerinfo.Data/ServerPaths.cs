// Copyright (c) János Janka. All rights reserved.

using System;
using System.IO;
using System.Web.Hosting;
using Partnerinfo.Properties;

namespace Partnerinfo
{
    public static class ServerPaths
    {
        public static readonly Uri BaseUri = new Uri("http://" + Settings.Default.AppHost);

        /// <summary>
        /// The virtual path to drive files on the server. 
        /// </summary>
        public static readonly string DriveFiles = @"\files";

        /// <summary>
        /// The virtual path to user media files on the server. 
        /// </summary>
        public static readonly string PortalMedia = @"\assets\static\portal\media";

        /// <summary>
        /// The virtual path to portal templates on the server.
        /// </summary>
        public static readonly string PortalTemplates = @"\assets\static\portal\templates";

        /// <summary>
        /// The virtual path to portal themes on the server.
        /// </summary>
        public static readonly string PortalThemes = @"\assets\static\portal\themes";

        /// <summary>
        /// Gets the physical path for the specified virtual path.
        /// </summary>
        /// <param name="serverPath">The server path specified on the <see cref="ServerPaths" /> class.</param>
        /// <returns>
        /// The physical path.
        /// </returns>
        public static string Map(string serverPath) => HostingEnvironment.MapPath("~" + serverPath);

        /// <summary>
        /// Gets the physical path for the specified virtual path.
        /// </summary>
        /// <param name="serverPath">The server path specified on the <see cref="ServerPaths" /> class.</param>
        /// <param name="path2">The first path to combine.</param>
        /// <returns>
        /// The physical path.
        /// </returns>
        public static string Map(string serverPath, string path2) => Path.Combine(Map(serverPath), path2);

        /// <summary>
        /// Gets the physical path for the specified virtual path.
        /// </summary>
        /// <param name="serverPath">The server path specified on the <see cref="ServerPaths" /> class.</param>
        /// <param name="path2">The first path to combine.</param>
        /// <param name="path2">The second path to combine.</param>
        /// <returns>
        /// The physical path.
        /// </returns>
        public static string Map(string serverPath, string path2, string path3) => Path.Combine(Map(serverPath), path2, path3);

        /// <summary>
        /// Gets the URI for the specified virtual path.
        /// </summary>
        /// <param name="serverPath">The server path specified on the <see cref="ServerPaths" /> class.</param>
        /// <returns>
        /// The physical path.
        /// </returns>
        public static Uri Uri(string serverPath) => new Uri(BaseUri, serverPath);

        /// <summary>
        /// Gets the URI for the specified virtual path.
        /// </summary>
        /// <param name="serverPath">The server path specified on the <see cref="ServerPaths" /> class.</param>
        /// <param name="path2">The first path to combine.</param>
        /// <returns>
        /// The physical path.
        /// </returns>
        public static Uri Uri(string serverPath, string path2) => Uri(string.Join("/", serverPath, path2));

        /// <summary>
        /// Gets the URI for the specified virtual path.
        /// </summary>
        /// <param name="serverPath">The server path specified on the <see cref="ServerPaths" /> class.</param>
        /// <param name="path2">The first path to combine.</param>
        /// <param name="path2">The second path to combine.</param>
        /// <returns>
        /// The physical path.
        /// </returns>
        public static Uri Uri(string serverPath, string path2, string path3) => Uri(string.Join("/", serverPath, path2, path3));
    }
}