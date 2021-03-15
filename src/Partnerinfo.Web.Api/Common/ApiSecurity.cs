// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Partnerinfo.Security;
using WebMatrix.WebData;

namespace Partnerinfo
{
    internal static class ApiSecurity
    {
        /// <summary>
        /// Gets the ID for the current user.
        /// </summary>
        /// <value>
        /// The ID for the current user.
        /// </value>
        public static int CurrentUserId => WebSecurity.CurrentUserId;

        /// <summary>
        /// Gets the user name for the current user.
        /// </summary>
        /// <value>
        /// The name of the current user.
        /// </value>
        public static string CurrentUserName => WebSecurity.CurrentUserName;

        /// <summary>
        /// Gets an instance of the <see cref="Manager" /> class.
        /// </summary>
        /// <value>
        /// The <see cref="Manager" />.
        /// </value>
        public static SecurityManager Manager => ApiConfig.HttpConfiguration.DependencyResolver.GetService(typeof(SecurityManager)) as SecurityManager;

        /// <summary>
        /// Throws a <see cref="HttpResponseException" /> at run time if the security requirement is not met.
        /// </summary>
        /// <param name="resource">The <see cref="SharedResourceItem" /> to protect.</param>
        /// <param name="requiredPermission">The required permission.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public static async Task AuthorizeAsync(SharedResourceItem resource, AccessPermission requiredPermission, CancellationToken cancellationToken)
        {
            if (resource == null)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            var securityResult = await Manager.CheckAccessAsync(resource, WebSecurity.IsAuthenticated ? CurrentUserName : null, requiredPermission, cancellationToken);
            if (!securityResult.AccessGranted)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
        }

        /// <summary>
        /// Throws a <see cref="HttpResponseException" /> at run time if the security requirement is not met.
        /// </summary>
        /// <param name="objectType">The type of the object to protect.</param>
        /// <param name="objectId">The primary key for the object to protect.</param>
        /// <param name="requiredPermission">The required permission.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public static async Task AuthorizeAsync(AccessObjectType objectType, int objectId, AccessPermission requiredPermission, CancellationToken cancellationToken)
        {
            if (objectType == AccessObjectType.Unknown)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            var securityResult = await Manager.CheckAccessAsync(objectType, objectId, WebSecurity.IsAuthenticated ? CurrentUserName : null, requiredPermission, cancellationToken);
            if (!securityResult.AccessGranted)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
        }
    }
}