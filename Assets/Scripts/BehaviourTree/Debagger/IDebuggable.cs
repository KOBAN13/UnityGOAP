namespace BehaviourTree
{
    public interface IDebuggable
    {
        IBTDebugger Debugger { get; }
    }
}