// Copyright (c) János Janka. All rights reserved.

using System;
using System.ComponentModel;
using System.Configuration;

namespace System.Web.Mvc.Configuration
{
    public sealed class LocalizationSettingsSection : ConfigurationSection
    {
        /// <summary>
        /// Gets or sets the culture key that represents both ASP.NET MVC route data key and cookie name.
        /// </summary>
        /// <value>
        /// The culture key.
        /// </value>
        [ConfigurationProperty("name", DefaultValue = "culture", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets or sets the domain associated with the cookie.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        [ConfigurationProperty("domain", DefaultValue = "")]
        public string Domain
        {
            get { return (string)this["domain"]; }
            set { this["domain"] = value; }
        }

        /// <summary>
        /// Gets or sets the virtual path associated with the cookie.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        [ConfigurationProperty("path", DefaultValue = "/")]
        [StringValidator(MinLength = 1)]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = string.IsNullOrEmpty(value) ? "/" : value; }
        }

        /// <summary>
        /// Gets or sets the expiration date and time for the cookie.
        /// </summary>
        /// <value>
        /// The expiration date and time.
        /// </value>
        [ConfigurationProperty("expiration", DefaultValue = "365.00:30:00")]
        [TypeConverter(typeof(TimeSpanConverter))]
        [TimeSpanValidator]
        public TimeSpan Expiration
        {
            get { return (TimeSpan)this["expiration"]; }
            set { this["expiration"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a specific culture must be used in MVC routes.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [ConfigurationProperty("useSpecificCulture", DefaultValue = false)]
        public bool UseSpecificCulture
        {
            get { return (bool)this["useSpecificCulture"]; }
            set { this["useSpecificCulture"] = value; }
        }
    }
}
