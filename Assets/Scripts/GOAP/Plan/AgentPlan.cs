using System.Collections.Generic;
using BehaviourTree;

namespace GOAP
{
    public class AgentPlan
    {
        public Stack<TempLeaf> Actions { get; }
        public float TotalCost { get; set; }
        
        public AgentPlan(float totalCost, Stack<TempLeaf> actions)
        {
            TotalCost = totalCost;
            Actions = actions;
        }
    }
}