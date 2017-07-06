// Copyright (c) János Janka. All rights reserved.

using System.Configuration;
using System.Threading;

namespace System.Web.Mvc.Configuration
{
    public sealed class LocalizationSettings
    {
        private static object s_syncRoot = new object();
        private static LocalizationSettings s_instance;
        public static readonly LocalizationSettings Default = new LocalizationSettings
        {
            Name = "LOC",
            Domain = null,
            Path = "/",
            Expiration = new TimeSpan(365, 0, 0, 0, 0),
            UseSpecificCulture = false
        };

        /// <summary>
        /// Prevents a default instance of the <see cref="LocalizationSettings" /> class from being created.
        /// </summary>
        private LocalizationSettings()
        {
        }

        /// <summary>
        /// Gets a singleton instance.
        /// </summary>
        /// <value>
        /// The singleton instance.
        /// </value>
        public static LocalizationSettings Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (s_syncRoot)
                    {
                        if (s_instance == null)
                        {
                            Thread.MemoryBarrier();
                            var config = (LocalizationSettingsSection)ConfigurationManager.GetSection("localization");
                            s_instance = config == null ? Default : new LocalizationSettings
                            {
                                Name = config.Name,
                                Domain = config.Domain,
                                Path = config.Path,
                                Expiration = config.Expiration,
                                UseSpecificCulture = config.UseSpecificCulture
                            };
                        }
                    }
                }
                return s_instance;
            }
        }

        /// <summary>
        /// Gets the culture key that represents both ASP.NET MVC route data key and cookie name.
        /// </summary>
        /// <value>
        /// The culture key.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the domain associated with the cookie.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public string Domain { get; private set; }

        /// <summary>
        /// Gets the virtual path associated with the cookie.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the expiration date and time for the cookie.
        /// </summary>
        /// <value>
        /// The expiration date and time.
        /// </value>
        public TimeSpan Expiration { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a specific culture (en-US, hu-HU, etc.) must be used.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public bool UseSpecificCulture { get; private set; }
    }
}
