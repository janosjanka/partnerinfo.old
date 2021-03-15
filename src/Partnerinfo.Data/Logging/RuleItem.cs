// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;

namespace Partnerinfo.Logging
{
    public class RuleItem : UniqueItem
    {
        /// <summary>
        /// A collection of conditions to be checked
        /// </summary>
        public ICollection<RuleConditionItem> Conditions { get; } = new List<RuleConditionItem>();

        /// <summary>
        /// A collection of actions to be performed if all the conditions are met
        /// </summary>
        public ICollection<RuleActionItem> Actions { get; } = new List<RuleActionItem>();
    }
}
