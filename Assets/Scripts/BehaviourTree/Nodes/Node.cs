using System.Collections.Generic;

namespace BehaviourTree
{
    public abstract class Node : INode, IDebuggable
    {
        public abstract string Name { get; protected set; }
        public IReadOnlyList<INode> Nodes => _nodes;
        public abstract BTNodeStatus Status { get; protected set; }
        public int CurrentChild { get; protected set; }
        public IBTDebugger Debugger { get; }
        public float Cost { get; }
        
        protected readonly List<INode> _nodes = new();
        public abstract void AddChild(INode node);

        public virtual BTNodeStatus Process()
        {
            Status = Nodes[CurrentChild].Process();
            return Status;
        }
        
        public virtual void Stop()
        {
            Nodes[CurrentChild].Stop();
        }

        public virtual void Start()
        {
            Nodes[CurrentChild].Start();
        }

        public void Reset()
        {
            CurrentChild = 0;
            
            foreach (var node in _nodes)
                node.Stop();
            
            _nodes.Clear();
            DebugReset();
        }

        private void DebugReset()
        {
            Debugger.NodeStatus.Value = Status;
            Debugger.NameNode.Clear();
            Debugger.TypeNode.Clear();
        }

        protected void Debug<T>(T node, string nameNode)
        {
            Debugger.NameNode.Add(nameNode);
            Debugger.TypeNode.Add(node.GetType().ToString());
        }

        protected void DebugStatus()
        {
            Debugger.NodeStatus.Value = Status;
        }

        protected Node(float cost, IBTDebugger debugger)
        {
            Cost = cost;
            Debugger = debugger;
        }
    }
}
