using GOAP.Pools;

namespace BehaviourTree
{
    public class Selector : Node, IPool
    {
        private int _previouslyChild;
        public override BTNodeStatus Status { get; protected set; }
        public sealed override string Name { get; protected set; }

        public Selector(string name, int cost, IBTDebugger debugger) : base(cost, debugger)
        {
            Name = name;
        }

        public void Clear()
        {
            _previouslyChild = 0;
            CurrentChild = 0;
            Status = BTNodeStatus.Success;
        }

        public override BTNodeStatus Process()
        {
            if (CurrentChild < Nodes.Count)
            {
                switch (Nodes[CurrentChild].Process())
                {
                    case BTNodeStatus.Running : return Status = BTNodeStatus.Running;
                    case BTNodeStatus.Success : return CompleteLeaf();
                    default:
                        return CompleteLeaf();
                }
            }
            
            Reset();
            return Status = BTNodeStatus.Success;
        }
        
        public override void Start()
        {
            Nodes[CurrentChild].Start();
            Debug(this, Name);
        }
        
        public override void Stop()
        {
            Nodes[_previouslyChild].Stop();
        }
        
        private BTNodeStatus CompleteLeaf()
        {
            _previouslyChild = CurrentChild;
            CurrentChild++;
            Status = CurrentChild == Nodes.Count ? BTNodeStatus.Success : BTNodeStatus.Running;
            Stop();
            if (Status != BTNodeStatus.Success)
            {
                Start();
            }
            return Status;
        }
    }
}