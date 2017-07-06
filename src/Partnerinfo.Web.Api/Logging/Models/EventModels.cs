// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;

namespace Partnerinfo.Logging.Models
{
    [Flags]
    public enum EventCompQueryField : byte
    {
        None = 0,
        Categories = 1 << 0
    }

    public sealed class EventCompQueryDto
    {
        /// <summary>
        /// Fields
        /// </summary>
        public EventCompQueryField Fields { get; set; }

        /// <summary>
        /// Event query
        /// </summary>
        public EventQueryDto Events { get; set; }

        /// <summary>
        /// Category query
        /// </summary>
        public CategoryQueryDto Categories { get; set; }
    }

    public sealed class EventCompResultDto
    {
        /// <summary>
        /// Event items
        /// </summary>
        public ListResult<EventResult> Events { get; set; }

        /// <summary>
        /// Event categories
        /// </summary>
        public ListResult<CategoryItem> Categories { get; set; }
    }

    public class EventFilterDto
    {
        /// <summary>
        /// Category ID (Foreign Key)
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// DateTime in UTC
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// DateTime in UTC
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Object Type
        /// </summary>
        public ObjectType? ObjectType { get; set; }

        /// <summary>
        /// Object ID (Foreign Key)
        /// </summary>
        public int? ObjectId { get; set; }

        /// <summary>
        /// Contact ID (Foreign Key)
        /// </summary>
        public int? ContactId { get; set; }

        /// <summary>
        /// Project ID (Foreign Key)
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Contact State
        /// </summary>
        public ObjectState? ContactState { get; set; }

        /// <summary>
        /// Custom URI
        /// </summary>
        public string CustomUri { get; set; }

        /// <summary>
        /// Emails to be included
        /// </summary>
        public IEnumerable<string> Emails { get; set; }

        /// <summary>
        /// Clients to be included
        /// </summary>
        public IEnumerable<string> Clients { get; set; }
    }

    public sealed class EventQueryDto : EventFilterDto
    {
        /// <summary>
        /// The index of the page of results to return. Use 1 to indicate the first page.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// The size of the page of results to return. The page index is non-zero-based.
        /// </summary>
        [Range(0, 100)]
        public int Limit { get; set; } = 50;
    }

    public class EventBulkUpdateDto
    {
        /// <summary>
        /// Event IDs
        /// </summary>
        public IEnumerable<int> Ids { get; set; }

        /// <summary>
        /// Event filter
        /// </summary>
        public EventFilterDto Filter { get; set; }
    }

    public sealed class EventCategoryBulkUpdateDto : EventBulkUpdateDto
    {
        /// <summary>
        /// Category ID (Foreign Key)
        /// </summary>
        [HttpBindRequired]
        public int? CategoryId { get; set; }
    }

    public sealed class EventContactBulkUpdateDto : EventBulkUpdateDto
    {
        /// <summary>
        /// Contact ID (Foreign Key)
        /// </summary>
        [HttpBindRequired]
        public int? ContactId { get; set; }
    }
}