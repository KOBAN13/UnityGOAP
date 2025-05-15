using Unity.Collections;

namespace GOAP
{
    public class ActionBuilder
    {
        private AgentAction agentAction;

        public ActionBuilder(string name)
        {
            agentAction = new AgentAction(name);
        }

        public ActionBuilder WithCost(int goalPriority, float time, float energy, float resources = 1f, float timeFactor = 1f, float resourceFactor = 1f, float energyFactor = 1f)
        {
            agentAction.CalculateCost(goalPriority, time, resources, energy, timeFactor, resourceFactor, energyFactor);
            return this;
        }
        
        public ActionBuilder WithActionStrategy(IActionStrategy strategy)
        {
            agentAction.SetActionStrategy(strategy);
            return this;
        }

        public ActionBuilder WithPrecondition(AgentBelief precondition)
        {
            agentAction.Precondition.Add(precondition);
            return this;
        }

        public ActionBuilder WithEffect(AgentBelief agentBelief)
        {
            agentAction.Effects.Add(agentBelief);
            return this;
        }

        public AgentAction BuildAgentAction() => agentAction;

    }
}