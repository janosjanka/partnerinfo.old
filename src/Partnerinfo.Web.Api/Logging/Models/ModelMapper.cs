// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Logging.Models
{
    /// <summary>
    /// Defines mapping functions for domain and data transfer objects (DTO).
    /// </summary>
    public static class ModelMapper
    {
        /// <summary>
        /// Returns with a <see cref="RuleResultDto" /> view of the <see cref="RuleItem" /> domain object.
        /// </summary>
        /// <param name="rule">The domain object to map to a data transfer object (DTO).</param>
        /// <returns>
        /// The <see cref="RuleResultDto" />.
        /// </returns>
        public static RuleResultDto ToRuleResultDto(RuleItem rule)
        {
            return new RuleResultDto
            {
                Id = rule.Id,
                Actions = rule.Actions,
                Conditions = rule.Conditions
            };
        }

        /// <summary>
        /// Returns with a <see cref="RuleItem" /> domain object.
        /// </summary>
        /// <param name="ruleDto">The data transfer object (DTO) to map to a domain object.</param>
        /// <returns>
        /// The <see cref="RuleItem" />.
        /// </returns>
        public static RuleItem ToRuleItem(RuleItemDto ruleDto)
        {
            var rule = new RuleItem();
            if (ruleDto.Conditions != null)
            {
                foreach (var condition in ruleDto.Conditions)
                {
                    rule.Conditions.Add(condition);
                }
            }
            if (ruleDto.Actions != null)
            {
                foreach (var action in ruleDto.Actions)
                {
                    rule.Actions.Add(action);
                }
            }
            return rule;
        }
    }
}