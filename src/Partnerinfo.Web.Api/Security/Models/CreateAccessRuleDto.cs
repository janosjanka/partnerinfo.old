// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Security.Models
{
    public class CreateAccessRuleDto
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CreateAccessRuleDto" /> is anyone.
        /// </summary>
        /// <value>
        ///   <c>true</c> if anyone; otherwise, <c>false</c>.
        /// </value>
        public bool Anyone { get; set; }

        /// <summary>
        /// Gets or sets the account for the item.
        /// </summary>
        /// <value>
        /// The account for the item.
        /// </value>
        [Email]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the permission level for the item.
        /// </summary>
        /// <value>
        /// The permission level for the item.
        /// </value>
        public AccessPermission Permission { get; set; }

        /// <summary>
        /// Gets or sets the visibility level for the item.
        /// </summary>
        /// <value>
        /// The visibility level for the item.
        /// </value>
        public AccessVisibility Visibility { get; set; }
    }
}