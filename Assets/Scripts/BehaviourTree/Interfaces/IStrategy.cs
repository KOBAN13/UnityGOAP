namespace BehaviourTree
{
    public interface IStrategy
    {
        BTNodeStatus Process();
        void Reset() {}
    }
}