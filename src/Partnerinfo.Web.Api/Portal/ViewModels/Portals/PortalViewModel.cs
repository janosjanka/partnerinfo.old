// Copyright (c) Partnerinfo Ltd. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Partnerinfo.Portal.ViewModels.Pages;

namespace Partnerinfo.Portal.ViewModels.Portals
{
    public class PortalViewModel : ResourceItem
    {
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>
        /// The project.
        /// </value>
        public UniqueItem Project { get; set; }

        /// <summary>
        /// Gets or sets the home page.
        /// </summary>
        /// <value>
        /// The home page.
        /// </value>
        public ResourceItem HomePage { get; set; }

        /// <summary>
        /// Gets or sets the master page.
        /// </summary>
        /// <value>
        /// The master page.
        /// </value>
        public ResourceItem MasterPage { get; set; }

        /// <summary>
        /// Gets or sets the description of this <see cref="PortalItem" />.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [MaxLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tracking code of Google Analytics.
        /// </summary>
        /// <value>
        /// The tracking code.
        /// </value>
        [MaxLength(128)]
        public string GATrackingId { get; set; }

        /// <summary>
        /// DateTime in UTC when this Project was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// DateTime in UTC when this Project was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; }
        
        /// <summary>
        /// Gets a collection of <see cref="AccountItem" />s that represent owners of this <see cref="PageResultViewModel" />.
        /// </summary>
        /// <value>
        /// A collection of <see cref="AccountItem" />s.
        /// </value>
        public ICollection<AccountItem> Owners { get; set; }

        /// <summary>
        /// Gets a collection of <see cref="PageResultViewModel" />s that represent pages of this <see cref="PageResultViewModel" />.
        /// </summary>
        /// <value>
        /// A collection of <see cref="PageResultViewModel" />s.
        /// </value>
        public ICollection<PageViewModel> Pages { get; set; }       
    }
}