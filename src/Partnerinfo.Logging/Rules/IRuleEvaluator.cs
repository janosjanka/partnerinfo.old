// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Logging.Rules
{
    public interface IRuleEvaluator
    {
        bool Evaluate(RuleProContext context);
    }
}
