using Unity.Collections;

namespace GOAP
{
    public class GoalBuilder
    {
        public AgentGoal _agentGoal;

        public GoalBuilder(string name)
        {
            _agentGoal = new AgentGoal(name);
        }

        public GoalBuilder WithPriority(float priority)
        {
            _agentGoal.SetPriority(priority);
            return this;
        }

        public GoalBuilder WithDesiredEffect(AgentBelief agentBelief)
        {
            _agentGoal.DesiredEffects.Add(agentBelief);
            return this;
        }

        public AgentGoal BuildGoal() => _agentGoal;
    }
}