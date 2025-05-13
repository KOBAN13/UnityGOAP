using System.Collections.Generic;

namespace GOAP
{
    public interface IGoapPlanner
    {
        public (AgentPlan plan, AgentGoal goal) GetPlan(
            HashSet<AgentAction> agent,
            HashSet<AgentGoal> goals,
            AgentGoal mostRecentGoal = null
        );
    }
}