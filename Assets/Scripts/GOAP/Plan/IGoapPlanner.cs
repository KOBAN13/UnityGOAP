using System.Collections.Generic;

namespace GOAP
{
    public interface IGoapPlanner
    {
        public GoapPlanContext GetPlan(
            HashSet<AgentAction> agent,
            HashSet<AgentGoal> goals,
            AgentGoal mostRecentGoal = default
        );
    }
}