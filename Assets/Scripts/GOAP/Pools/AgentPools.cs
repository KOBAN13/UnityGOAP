using System.Collections.Generic;
using BehaviourTree;

namespace GOAP.Pools
{
    public class AgentPools
    {
        public CollectionPool<HashSet<AgentBelief>> PoolHashSet { get; private set; }
        public InstantiablePool<TempLeaf> TempLeafPool { get; private set; }
        public FixedObjectPool<INode> NodesBehaviourTree { get; private set; }

        public AgentPools(IBTDebugger debugger)
        {
            InitializePools(debugger);
        }

        private void InitializePools(IBTDebugger debugger)
        {
            var listSelectors = new List<INode>();

            for (var i = 0; i < 10; i++)
            {
                listSelectors.Add(new Selector("Selector Leafs", 0, debugger));
            }
            
            PoolHashSet = new CollectionPool<HashSet<AgentBelief>>(null, null, 10);
            TempLeafPool = new InstantiablePool<TempLeaf>(null, null, 10);
            NodesBehaviourTree = new FixedObjectPool<INode>(listSelectors, () => new Selector("Selector Leafs", 0, debugger));
        }
    }
}