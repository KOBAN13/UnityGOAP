using System.Collections.Generic;
using Customs;

namespace GOAP
{
    public class AgentGoal
    {
        public string Name { get; }
        public float Priority { get; private set; }
        public List<AgentBelief> DesiredEffects { get; } //прийдется переделывать на NativeArray

        public AgentGoal(string name)
        {
            Name = name;
            DesiredEffects = new List<AgentBelief>();
            Priority = 0;
        }
        
        public void SetPriority(float priority)
        {
            Preconditions.CheckValidateData(priority);
            Priority = priority;
        }
    }
}