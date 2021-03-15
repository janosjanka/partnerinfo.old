// Copyright (c) János Janka. All rights reserved.

using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using Partnerinfo.Properties;

namespace Partnerinfo.Portal
{
    public class PortalHosting : IDisposable
    {
        private ServerManager _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalHosting"/> class.
        /// </summary>
        public PortalHosting()
            : this(new ServerManager())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalHosting" /> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public PortalHosting(ServerManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            _manager = manager;
        }

        /// <summary>
        /// Indicates whether the string is well-formed by attempting to construct a
        /// URI with the string and ensures that the string does not require further escaping.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <returns>
        ///   <c>true</c> if the string was well-formed; otherwise <c>false</c>.
        /// </returns>
        public static bool IsValidDomain(string domain)
        {
            return !string.IsNullOrEmpty(domain) && Uri.IsWellFormedUriString("http://" + domain, UriKind.Absolute);
        }

        /// <summary>
        /// Gets the domain name without extra information.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <returns>
        /// A string that represents the domain without extra information.
        /// </returns>
        public static string ExtractDomain(string domain)
        {
            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }

            domain = new Uri("http://" + domain.ToLowerInvariant()).Host;

            if (domain.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                domain = domain.Remove(0, 4);
            }

            return domain;
        }

        /// <summary>
        /// Checks DNS settings.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <returns></returns>
        public static async Task<bool> CheckDnsAsync(string domain)
        {
            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }

            IPAddress[][] results;

            try
            {
                results = await Task.WhenAll(
                    Dns.GetHostAddressesAsync(Settings.Default.PortalHost),
                    Dns.GetHostAddressesAsync(domain));
            }
            catch (SocketException)
            {
                return false;
            }

            return AddressListEquals(results[0], results[1]);
        }

        /// <summary>
        /// Gets the information from IIS for the site associated with the specified site.
        /// </summary>
        /// <param name="domain">The domain to search for.</param>
        /// <returns>
        /// Returns the site associated with the specified domain.
        /// </returns>
        public Site FindSiteByDomain(string domain)
        {
            domain = CreateFullName(domain);

            for (int i = _manager.Sites.Count - 1; i >= 0; i--)
            {
                if (string.Equals(_manager.Sites[i].Name, domain, StringComparison.OrdinalIgnoreCase))
                {
                    return _manager.Sites[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns with a value indicating whether the specified site exists.
        /// </summary>
        /// <param name="domain">The domain to search for.</param>
        /// <returns></returns>
        public bool SiteExists(string domain)
        {
            return FindSiteByDomain(domain) != null;
        }

        /// <summary>
        /// Returns a value whether the domain exists. It is more precise than the SiteExists method.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <returns>
        ///   <c>true</c> if the domain exists; otherwise <c>false</c>.
        /// </returns>
        public bool DomainExists(string domain)
        {
            string wwwDomain = "www." + domain;

            for (int i = _manager.Sites.Count - 1; i >= 0; i--)
            {
                var site = _manager.Sites[i];
                for (int j = site.Bindings.Count - 1; j >= 0; j--)
                {
                    string hostName = site.Bindings[j].Host;
                    if (string.Equals(hostName, domain, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(hostName, wwwDomain, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a new IIS site with a new application pool.
        /// </summary>
        /// <param name="domain">The domain to search for.</param>
        /// <returns>
        /// The <see cref="Site" /> instance if the insert was successful; otherwise, <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentException">The domain name is required.</exception>
        public long CreateSite(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentException("The domain name is required.");
            }

            string siteName = CreateFullName(domain);

            CreateAppPool(domain);

            var site = _manager.Sites.Add(siteName, Settings.Default.PortalPath, 80);
            site.ServerAutoStart = true;
            site.ApplicationDefaults.ApplicationPoolName = siteName;

            site.Bindings.Clear();
            site.Bindings.Add("*:80:www." + domain, "http");
            site.Bindings.Add("*:80:" + domain, "http");

            return site.Id;
        }

        /// <summary>
        /// Deletes the specified site
        /// </summary>
        /// <param name="domain">The name of the site.</param>
        public void DeleteSite(string domain)
        {
            var site = FindSiteByDomain(domain);
            if (site != null)
            {
                _manager.Sites.Remove(site);
                DeleteAppPool(domain);
            }
        }

        /// <summary>
        /// Saves changes.
        /// </summary>
        public void Save()
        {
            _manager.CommitChanges();
        }

        /// <summary>
        /// Gets the name of the site in IIS.
        /// </summary>
        /// <param name="domain">The name of the site.</param>
        /// <returns>
        /// A string that represents the name of the site in IIS.
        /// </returns>
        private static string CreateFullName(string domain)
        {
            return string.Format(Settings.Default.PortalName, domain);
        }

        /// <summary>
        /// Determines whether the two IP address lists are equal.
        /// </summary>
        /// <param name="addressList1">The first IP address list.</param>
        /// <param name="addressList2">The second IP address list.</param>
        /// <returns></returns>
        private static bool AddressListEquals(IPAddress[] addressList1, IPAddress[] addressList2)
        {
            Contract.Assert(addressList1 != null);
            Contract.Assert(addressList2 != null);

            for (int i = 0; i < addressList1.Length; i++)
            {
                for (int j = 0; j < addressList2.Length; j++)
                {
                    if (IPAddress.Equals(addressList1[i], addressList2[j]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the information from IIS for the application pool associated with the specified domain.
        /// </summary>
        /// <param name="domain">The domain to search for.</param>
        /// <returns>
        /// Returns the application pool associated with the specified domain.
        /// </returns>
        private ApplicationPool FindAppPool(string domain)
        {
            string appPoolName = CreateFullName(domain);

            for (int i = _manager.ApplicationPools.Count - 1; i >= 0; i--)
            {
                if (string.Equals(_manager.ApplicationPools[i].Name, appPoolName, StringComparison.OrdinalIgnoreCase))
                {
                    return _manager.ApplicationPools[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a new application pool for the site.
        /// </summary>
        /// <param name="domain">The name of the application pool.</param>
        /// <returns></returns>
        private ApplicationPool CreateAppPool(string domain)
        {
            string appPoolName = CreateFullName(domain);
            ApplicationPool appPool = _manager.ApplicationPools.Add(appPoolName);
            appPool.ManagedRuntimeVersion = "v4.0";
            appPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
            appPool.AutoStart = true;
            return appPool;
        }

        /// <summary>
        /// Deletes the specified application pool.
        /// </summary>
        /// <param name="domain">The domain.</param>
        private void DeleteAppPool(string domain)
        {
            ApplicationPool appPool = FindAppPool(domain);
            if (appPool != null)
            {
                _manager.ApplicationPools.Remove(appPool);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_manager != null)
                {
                    _manager.Dispose();
                    _manager = null;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
    }
}
