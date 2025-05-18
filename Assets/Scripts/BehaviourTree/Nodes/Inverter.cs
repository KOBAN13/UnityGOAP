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

            return Nodes[0].Process() switch
            {
                BTNodeStatus.Running => Status = BTNodeStatus.Running,
                BTNodeStatus.Failure => Status = BTNodeStatus.Success,
                _ => Status = BTNodeStatus.Failure
            };
        }
    }
}