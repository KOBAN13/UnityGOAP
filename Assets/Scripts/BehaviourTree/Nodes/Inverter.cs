namespace BehaviourTree
{
    public class Inverter : Node
    {
        public override BTNodeStatus Status { get; protected set; }
        public sealed override string Name { get; protected set; }
        
        protected Inverter(string name, int cost, IBTDebugger debugger) : base(cost, debugger)
        {
            Name = name;
        }

        public override BTNodeStatus Process()
        {
            Debug(this, Name);
            
            switch (Nodes[0].Process())
            {
                case BTNodeStatus.Running: return Status = BTNodeStatus.Running;
                case BTNodeStatus.Failure: return Status = BTNodeStatus.Success;
                default: return Status = BTNodeStatus.Failure;
            }
        }

        public override void AddChild(INode node) => _nodes.Add(node);
    }
}