// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Logging.Rules
{
    public class RuleEvaluator : IRuleEvaluator
    {
        public virtual bool Evaluate(RuleProContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            bool succeeded = true;

            foreach (var condition in context.RuleItem.Conditions)
            {
                switch (condition.Code)
                {
                    case RuleConditionCode.StartDateGreaterThan:
                        {
                            DateTime value;
                            succeeded &= DateTime.TryParse(condition.Value, out value) ? (context.EventItem.StartDate > value) : false;
                        }
                        break;
                    case RuleConditionCode.StartDateLessThan:
                        {
                            DateTime value;
                            succeeded &= DateTime.TryParse(condition.Value, out value) ? (context.EventItem.StartDate < value) : false;
                        }
                        break;
                    case RuleConditionCode.ClientIdContains:
                        {
                            succeeded &= context.EventItem.ClientId?.Contains(condition.Value) ?? false;
                        }
                        break;
                    case RuleConditionCode.CustomUriContains:
                        {
                            succeeded &= context.EventItem.CustomUri?.Contains(condition.Value) ?? false;
                        }
                        break;
                    case RuleConditionCode.ReferrerUrlContains:
                        {
                            succeeded &= context.EventItem.ReferrerUrl?.Contains(condition.Value) ?? false;
                        }
                        break;
                    case RuleConditionCode.ProjectIdEquals:
                        {
                            int value;
                            succeeded &= int.TryParse(condition.Value, out value)
                                ? (context.EventItem.Project?.Id == value)
                                : (context.EventItem.Project?.Id == null && condition.Value == null);
                        }
                        break;
                    case RuleConditionCode.ContactStateEquals:
                        {
                            ObjectState value;
                            succeeded &= Enum.TryParse(condition.Value, out value) ? (context.EventItem.ContactState == value) : false;
                        }
                        break;
                    case RuleConditionCode.ContactMailContains:
                        {
                            succeeded &= context.EventItem.Contact?.Email?.Address?.Contains(condition.Value) ?? false;
                        }
                        break;
                }
            }

            return succeeded;
        }
    }
}
