
namespace BehaviourTree
{
    public interface INode
    {
        BTNodeStatus Status { get; }
        string Name { get; }
        float Cost { get; }
        int CurrentChild { get; }
        BTNodeStatus Process();
        void Stop();
        void Start();
    }
}