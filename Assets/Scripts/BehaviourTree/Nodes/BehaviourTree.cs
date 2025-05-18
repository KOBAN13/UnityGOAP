using GOAP;

namespace BehaviourTree
{
    public class BehaviourTree : Node
    {
        public override BTNodeStatus Status { get; protected set; }
        public sealed override string Name { get; protected set; }
        public AgentGoal CurrentGoal { get; private set; }
        public AgentGoal LastGoal { get; private set; }

        public BehaviourTree(string name, float cost, IBTDebugger debugger) : base(cost, debugger)
        {
            Name = name;
        }

        public void SetGoalsState(AgentGoal currentGoal, AgentGoal lastGoal)
        {
            LastGoal = lastGoal;
            CurrentGoal = currentGoal;
        }

        public override BTNodeStatus Process()
        {
            while (CurrentChild < Nodes.Count)
            {
                Status = Nodes[CurrentChild].Process();

                if (Status != BTNodeStatus.Success) return Status;

                CurrentChild++;
            }

            return Status = BTNodeStatus.Success;
        }

        public override void Start()
        {
            base.Start();
            Debug(this, Name);
        }
    }
}