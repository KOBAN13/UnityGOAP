using System.Collections.Generic;
using GOAP;

namespace Helpers.Factory
{
    public class GoalFactory
    {
        private HashSet<AgentGoal> _agentGoals;

        public GoalFactory(HashSet<AgentGoal> agentGoals)
        {
            _agentGoals = agentGoals;
        }

        public AgentGoal AddGoalAgent(string key, int priority, AgentBelief agentBelief)
        {
            var goal = new GoalBuilder(key);
            
            _agentGoals
                .Add(goal
                .WithPriority(priority)
                .WithDesiredEffect(agentBelief)
                .BuildGoal());

            return goal._agentGoal;
        }
    }
}