using System.Collections.Generic;
using System.Linq;
using GOAP;
using UnityEngine;

namespace BehaviourTree
{
    public class Leaf : Node
    {
        public override BTNodeStatus Status { get; protected set; }
        public sealed override string Name { get; protected set; }
        public bool IsLeafDead => Nodes.Count == 0 && _agentAction.Equals(default);
        public readonly HashSet<AgentBelief> RequiredEffects;
        
        private AgentAction _agentAction;

        public override BTNodeStatus Process()
        {
            if (!_agentAction.Precondition.All(b => b.CheckCondition()))
            {
                return Status = BTNodeStatus.Failure;
            }
            
            DebugStatus();
            Status = _agentAction.Update(Time.deltaTime);
            return Status;
        }

        public override void Stop()
        {
            _agentAction.Stop();
        }

        public override void Start()
        {
            if (!_agentAction.Precondition.All(b => b.CheckCondition()))
            {
                return;
            }
            
            Debug(this, Name);
            _agentAction.Start();
        }

        public Leaf(AgentAction agentAction, HashSet<AgentBelief> requiredEffects, float cost, string name, IBTDebugger btDebugger) : base(cost, btDebugger)
        {
            _agentAction = agentAction;
            RequiredEffects = new HashSet<AgentBelief>(requiredEffects);
            Name = name;
        }
    }
}