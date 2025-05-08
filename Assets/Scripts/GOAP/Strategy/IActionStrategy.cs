using System.Threading;
using BehaviourTree;

namespace GOAP
{
    public interface IActionStrategy
    {
        bool CanPerform { get; }
        bool Complete { get; }
        CancellationTokenSource CancellationTokenSource { get; }
        void Start() {}
        void Update(float deltaTime) {}
        void Stop() {}
    }
}