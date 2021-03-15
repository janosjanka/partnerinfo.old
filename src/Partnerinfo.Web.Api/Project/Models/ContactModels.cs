// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using Newtonsoft.Json;

namespace Partnerinfo.Project.Models
{
    public sealed class ContactQueryDto
    {
        /// <summary>
        /// The project whose associated contacts to retrieve.
        /// </summary>
        [HttpBindRequired]
        public int ProjectId { get; set; }

        /// <summary>
        /// The name provided to identify a contact.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of <see cref="BusinessTagItem"/> to be specified on all the contacts.
        /// </summary>
        public IEnumerable<int> IncludeWithTags { get; } = new HashSet<int>();

        /// <summary>
        /// A list of <see cref="BusinessTagItem"/> to be unspecified on all the contacts.
        /// </summary>
        public IEnumerable<int> ExcludeWithTags { get; } = new HashSet<int>();

        /// <summary>
        /// The order in which contacts are returned in a result set.
        /// </summary>
        public ContactSortOrder OrderBy { get; set; } = ContactSortOrder.None;

        /// <summary>
        /// The index of the page of results to return. Use 1 to indicate the first page.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        /// <summary>
        /// The size of the page of results to return. <see cref="Page"/> is non-zero-based.
        /// </summary>
        [Range(1, 50)]
        public int Limit { get; set; } = 50;

        /// <summary>
        /// The related fields to include in the query results.
        /// </summary>
        public ContactField Fields { get; set; } = ContactField.None;
    }

    public sealed class ContactItemDto
    {
        /// <summary>
        /// Sponsor ID
        /// </summary>
        public int? SponsorId { get; set; }

        /// <summary>
        /// Facebook ID
        /// </summary>
        public long? FacebookId { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        public MailAddressItem Email { get; set; }

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
        /// Phone numbers
        /// </summary>
        public PhoneGroupItem Phones { get; set; }

        /// <summary>
        /// Extra comment
        /// </summary>
        public string Comment { get; set; }
    }

    public sealed class ContactTagsDto
    {
        /// <summary>
        /// A list of contacts IDs
        /// </summary>
        [HttpBindRequired, Required]
        public IEnumerable<int> Ids { get; set; }

        /// <summary>
        /// A list of contact tag IDs to be included
        /// </summary>
        public IEnumerable<int> TagsToAdd { get; set; }

        /// <summary>
        /// A list of contact tag IDs to be excluded
        /// </summary>
        public IEnumerable<int> TagsToRemove { get; set; }
    }
}