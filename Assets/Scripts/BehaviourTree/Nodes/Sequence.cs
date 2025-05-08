namespace BehaviourTree
{
    public class Sequence : Node
    {
        public override BTNodeStatus Status { get; protected set; }
        public sealed override string Name { get; protected set; }
        private int _previouslyChild = 0;

        public Sequence(string name, int cost, IBTDebugger debugger) : base(cost, debugger)
        {
            Name = name;
        }

        public override BTNodeStatus Process()
        {
            if (CurrentChild < Nodes.Count)
            {
                switch (Nodes[CurrentChild].Process())
                {
                    case BTNodeStatus.Running: return Status = BTNodeStatus.Running;
                    case BTNodeStatus.Failure: Reset();
                        return Status = BTNodeStatus.Failure;
                    default: return CompleteLeaf();
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

        public override void AddChild(INode node) => _nodes.Add(node);

        private BTNodeStatus CompleteLeaf()
        {
            _previouslyChild = CurrentChild;
            CurrentChild++;
            Status = CurrentChild == _nodes.Count ? BTNodeStatus.Success : BTNodeStatus.Running;
            Stop();
            if (Status != BTNodeStatus.Success)
            {
                Start();
            }
            return Status;
        }
    }
}