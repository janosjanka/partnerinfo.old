// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;

namespace Partnerinfo.Project.Models
{
    public sealed class MailMessageQueryDto
    {
        /// <summary>
        /// Project ID (Foreign Key)
        /// </summary>
        [HttpBindRequired]
        public int ProjectId { get; set; }

        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Order by
        /// </summary>
        public MailMessageSortOrder OrderBy { get; set; } = MailMessageSortOrder.None;

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
        public MailMessageField Fields { get; set; } = MailMessageField.None;
    }

    public sealed class MailMessageHeaderDto
    {
        /// <summary>
        /// Sender
        /// </summary>
        public ContactItem From { get; set; }

        /// <summary>
        /// Gets the address collection that contains the recipients of this mail message.
        /// </summary>
        /// <value>
        /// The address collection that contains the recipients of this mail message.
        /// </value>
        public IEnumerable<int> To { get; set; }

        /// <summary>
        /// Gets the business tag collections that must be specified on
        /// </summary>
        public IEnumerable<int> IncludeWithTags { get; set; }

        /// <summary>
        /// Contacts that do not contain the given tags
        /// </summary>
        public IEnumerable<int> ExcludeWithTags { get; set; }

        /// <summary>
        /// Message placeholders
        /// </summary>
        public IDictionary<string, object> Placeholders { get; set; }

        /// <summary>
        /// Converts the model object to a <see cref="MailMessageHeader"/> object.
        /// </summary>
        /// <returns>
        /// The <see cref="MailMessageHeader"/> object.
        /// </returns>
        public MailMessageHeader ToMailMessageHeader()
        {
            var header = new MailMessageHeader { From = From };
            if (To != null)
            {
                foreach (var to in To)
                {
                    header.To.Add(to);
                }
            }
            if (IncludeWithTags != null)
            {
                foreach (var tag in IncludeWithTags)
                {
                    header.IncludeWithTags.Add(tag);
                }
            }
            if (ExcludeWithTags != null)
            {
                foreach (var tag in ExcludeWithTags)
                {
                    header.ExcludeWithTags.Add(tag);
                }
            }
            if (Placeholders != null)
            {
                foreach (var placeHolder in Placeholders)
                {
                    header.Placeholders.Add(placeHolder);
                }
            }
            return header;
        }
    }

    public sealed class MailMessageItemDto
    {
        /// <summary>
        /// Mail subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Mail body
        /// </summary>
        public string Body { get; set; }
    }
}