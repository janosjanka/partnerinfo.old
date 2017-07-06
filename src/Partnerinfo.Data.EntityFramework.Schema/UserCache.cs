// -----------------------------------------------------------------------
// <copyright>
//    Copyright (c) Partnerinfo. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Partnerinfo.Data
{
    using System;

    public class UserCache : IUserCache
    {
        private const string Key = "user";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCache" /> class.
        /// </summary>
        /// <param name="cacheProvider">The cache provider.</param>
        public UserCache(AppFabricCache cacheProvider)
        {
            if (cacheProvider == null)
            {
                throw new ArgumentNullException("cacheProvider");
            }

            this.CacheProvider = cacheProvider;
        }

        /// <summary>
        /// Distributed cache provider
        /// </summary>
        protected AppFabricCache CacheProvider { get; private set; }

        /// <summary>
        /// Gets the information from the cache for the user associated with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique user identifier from the data source for the user.</param>
        /// <returns>Returns the user associated with the specified unique identifier.</returns>
        public User GetUser(int id)
        {
            return CacheProvider.Get(CreateKey(id)) as User;
        }

        /// <summary>
        /// Adds or replaces an user in the cache.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>
        ///   <c>true</c> if the update was successful; otherwise, <c>false</c>.
        /// </returns>
        public bool PutUser(User user)
        {
            if (user != null)
            {
                CacheProvider.Put(CreateKey(user.Id), user, TimeSpan.FromMinutes(10));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the specified user from the cache.
        /// </summary>
        /// <param name="id">The unique user identifier from the data source for the user.</param>
        public void RemoveUser(int id)
        {
            CacheProvider.Remove(CreateKey(id));
        }

        /// <summary>
        /// Creates a new cache key
        /// </summary>
        private static string CreateKey(int id)
        {
            return Key + id;
        }
    }
}
