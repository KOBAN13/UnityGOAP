using System.Collections.Generic;
using Unity.VisualScripting;

namespace BehaviourTree
{
    public abstract class Node : INode, IDebuggable
    {
        public abstract string Name { get; protected set; }
        public abstract BTNodeStatus Status { get; protected set; }
        public int CurrentChild { get; protected set; }
        public IBTDebugger Debugger { get; }
        public float Cost { get; }
        public IReadOnlyList<INode> Nodes => _nodes;
        
        private readonly List<INode> _nodes = new();

        public virtual BTNodeStatus Process()
        {
            Status = Nodes[CurrentChild].Process();
            return Status;
        }
        
        public virtual void Stop()
        {
            if(Nodes.Count == 0)
                return;
            
            Nodes[CurrentChild].Stop();
        }

        public virtual void Start()
        {
            if(Nodes.Count == 0)
                return;
            
            Nodes[CurrentChild].Start();
        }
        
        public void AddChild(INode node) => _nodes.Add(node);

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
