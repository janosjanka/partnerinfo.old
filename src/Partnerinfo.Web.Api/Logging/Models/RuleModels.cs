// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;

namespace Partnerinfo.Logging.Models
{
    public sealed class RuleQueryDto
    {
        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        public RuleField Fields { get; set; }
    }

    public sealed class RuleItemDto
    {
        /// <summary>
        /// A collection of conditions to be checked
        /// </summary>
        public IEnumerable<RuleConditionItem> Conditions { get; set; }

        /// <summary>
        /// A collection of actions to be performed if all the conditions are met
        /// </summary>
        public IEnumerable<RuleActionItem> Actions { get; set; }
    }

    public sealed class RuleResultDto
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A collection of conditions to be checked
        /// </summary>
        public IEnumerable<RuleConditionItem> Conditions { get; set; }

        /// <summary>
        /// A collection of actions to be performed if all the conditions are met
        /// </summary>
        public IEnumerable<RuleActionItem> Actions { get; set; }
    }
}