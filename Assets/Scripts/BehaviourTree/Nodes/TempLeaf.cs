using System.Collections.Generic;
using GOAP;
using GOAP.Pools;

namespace BehaviourTree
{
    public class TempLeaf : IPool
    {
        public AgentAction AgentAction { get; private set; }
        public HashSet<AgentBelief> RequiredEffects { get; private set; }
        public float Cost { get; private set; }
        public string Name { get; private set; }
        
        public IReadOnlyList<TempLeaf> Children => _children;
        private readonly List<TempLeaf> _children = new();

        public void InitializeLeaf(AgentAction action, HashSet<AgentBelief> effects, float cost, string name)
        {
            AgentAction = action;
            RequiredEffects = effects;
            Cost = cost;
            Name = name;
        }

        public void AddChild(TempLeaf child) => _children.Add(child);
        
        public void Clear()
        {
            _children.Clear();
        }
    }
}