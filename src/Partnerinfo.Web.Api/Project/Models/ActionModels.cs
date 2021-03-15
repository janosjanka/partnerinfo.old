// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Partnerinfo.Project.Models
{
    public sealed class ActionQueryDto
    {
        /// <summary>
        /// The project whose associated actions to retrieve.
        /// </summary>
        [HttpBindRequired]
        public int ProjectId { get; set; }

        /// <summary>
        /// The name provided to identify an action.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The order in which actions are returned in a result set.
        /// </summary>
        public ActionSortOrder OrderBy { get; set; } = ActionSortOrder.None;

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
        public ActionField Fields { get; set; } = ActionField.None;
    }

    public sealed class ActionItemDto
    {
        /// <summary>
        /// MailMessage ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Action type
        /// </summary>
        public ActionType Type { get; set; }

        /// <summary>
        /// Returns true if this action is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Action name (optional)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Options
        /// </summary>
        [JsonProperty]
        public JObject Options { get; set; }
    }

    public sealed class ActionSortOrderDto
    {
        /// <summary>
        /// Reference ID
        /// </summary>
        public int? ReferenceId { get; set; }
    }

    public sealed class ActionResultDto
    {
        /// <summary>
        /// Action ID (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Project which owns this action
        /// </summary>
        public UniqueItem Project { get; set; }

        /// <summary>
        /// Action type
        /// </summary>
        public ActionType Type { get; set; }

        /// <summary>
        /// Returns true if this action is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// DateTime in UTC when this playlist was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Action name (optional)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Options
        /// </summary>
        public JObject Options { get; set; }

        /// <summary>
        /// Navigation property for children
        /// </summary>
        public ICollection<ActionResultDto> Children { get; set; }

        /// <summary>
        /// Action link
        /// </summary>
        public string Link { get; set; }
    }

    public sealed class ActionLinkResultDto
    {
        /// <summary>
        /// Action
        /// </summary>
        public ActionResultDto Action { get; set; }

        /// <summary>
        /// Contact
        /// </summary>
        public AccountItem Contact { get; set; }

        /// <summary>
        /// Custom URI
        /// </summary>
        public string CustomUri { get; set; }

        /// <summary>
        /// Action link
        /// </summary>
        public string Link { get; set; }
    }
}