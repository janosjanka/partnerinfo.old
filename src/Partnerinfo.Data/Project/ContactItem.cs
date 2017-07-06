// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Project
{
    public class ContactItem : AccountItem
    {
        /// <summary>
        /// Gets or sets the <see cref="ProjectItem" /> which owns this <see cref="ContactItem" />.
        /// </summary>
        /// <value>
        /// The <see cref="ProjectItem" />.
        /// </value>
        public UniqueItem Project { get; set; }

        /// <summary>
        /// Gets or sets the sponsor of this <see cref="ContactItem" />.
        /// </summary>
        /// <value>
        /// The sponsor.
        /// </value>
        public AccountItem Sponsor { get; set; }

        /// <summary>
        /// Gets or sets the phone numbers for this <see cref="ContactItem" />.
        /// </summary>
        /// <value>
        /// The phone numbers for this <see cref="ContactItem" />.
        /// </value>
        [TemplateField("Personal", Name = "TemplateField_Phones_Personal", ResourceType = typeof(Resources))]
        [TemplateField("Business", Name = "TemplateField_Phones_Business", ResourceType = typeof(Resources))]
        [TemplateField("Mobile", Name = "TemplateField_Phones_Mobile", ResourceType = typeof(Resources))]
        [TemplateField("Other", Name = "TemplateField_Phones_Other", ResourceType = typeof(Resources))]
        public PhoneGroupItem Phones { get; set; }

        /// <summary>
        /// Gets or sets the comment for this <see cref="ContactItem" />.
        /// </summary>
        /// <value>
        /// The comment for this <see cref="ContactItem" />.
        /// </value>
        [TemplateField(Name = "TemplateField_Comment", ResourceType = typeof(Resources))]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when this <see cref="ContactItem" /> was last modified.
        /// </summary>
        /// <value>
        /// The date and time, in UTC, when this <see cref="ContactItem" /> was last modified.
        /// </value>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets a collection of <see cref="UniqueItem" />s to be belonging to this <see cref="ContactItem" />.
        /// </summary>
        /// <value>
        /// A collection of <see cref="UniqueItem" />s to be belonging to this <see cref="ContactItem" />.
        /// </value>
        public ICollection<UniqueItem> BusinessTags { get; set; } = new List<UniqueItem>();
    }
}
