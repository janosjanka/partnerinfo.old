// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;

namespace Partnerinfo.Logging.EntityFramework
{
    public class LoggingRuleOptions
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
}
