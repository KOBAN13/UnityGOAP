using System.Collections.Generic;
using GOAP;

namespace BehaviourTree
{
    public class TempLeaf
    {
        public AgentAction AgentAction { get; }
        public HashSet<AgentBelief> RequiredEffects { get; }
        public float Cost { get; }
        public string Name { get; }
        public bool IsLeafDead { get; set; }
        
        public IReadOnlyList<TempLeaf> Children => _children;
        private readonly List<TempLeaf> _children = new();

        public TempLeaf(AgentAction action, HashSet<AgentBelief> effects, float cost, string name)
        {
            AgentAction = action;
            RequiredEffects = effects;
            Cost = cost;
            Name = name;
        }
        
        public void AddChild(TempLeaf child) => _children.Add(child);
    }
}