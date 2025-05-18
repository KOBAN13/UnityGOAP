
using System.Collections.Generic;

namespace BehaviourTree
{
    public interface INode
    {
        BTNodeStatus Status { get; }
        IReadOnlyList<INode> Nodes { get; }
        string Name { get; }
        float Cost { get; }
        int CurrentChild { get; }
        BTNodeStatus Process();
        void Stop();
        void Start();
        void AddChild(INode node);
    }
}