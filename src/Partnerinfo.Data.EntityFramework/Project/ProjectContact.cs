// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using Partnerinfo.Media.EntityFramework;

namespace Partnerinfo.Project.EntityFramework
{
    public class ProjectContact
    {
        /// <summary>
        /// Contact ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Project ID (Foreign Key)
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Project which owns this contact
        /// </summary>
        public virtual ProjectEntity Project { get; set; }

        /// <summary>
        /// Sponsor (Foreign Key)
        /// </summary>
        public int? SponsorId { get; set; }

        /// <summary>
        /// Sponsor
        /// </summary>
        public virtual ProjectContact Sponsor { get; set; }

        /// <summary>
        /// Gets or sets the facebook identifier for this contact.
        /// </summary>
        /// <value>
        /// The facebook identifier for this contact.
        /// </value>
        public long? FacebookId { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        public MailAddressItem Email { get; set; } = MailAddressItem.None;

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Nick name
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Person gender ( male | female )
        /// </summary>
        public PersonGender Gender { get; set; }

        /// <summary>
        /// Date when this user was born
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// DateTime in UTC when this contact was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Phone numbers
        /// </summary>
        public PhoneGroupItem Phones { get; set; } = PhoneGroupItem.Empty;

        /// <summary>
        /// Extra comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Navigation property for tags
        /// </summary>
        public virtual ICollection<ProjectContactTag> BusinessTags { get; } = new List<ProjectContactTag>();

        /// <summary>
        /// Navigation property for tags
        /// </summary>
        public virtual ICollection<MediaPlaylist> Playlists { get; } = new List<MediaPlaylist>();
    }
}
