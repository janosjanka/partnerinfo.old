// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Project
{
    public class ProjectInvitation
    {
        /// <summary>
        /// The user who was sent this invitation
        /// </summary>
        [TemplateField(Name = "TemplateField_Invitation_From", ResourceType = typeof(Resources))]
        public ContactItem From { get; set; }

        /// <summary>
        /// A message that will be forwarded to the contacts below
        /// </summary>
        [TemplateField(Name = "TemplateField_Invitation_Message", ResourceType = typeof(Resources))]
        public string Message { get; set; }

        /// <summary>
        /// A collection of contacts who will be invited
        /// </summary>
        public IEnumerable<ContactItem> To { get; set; }
    }
}
