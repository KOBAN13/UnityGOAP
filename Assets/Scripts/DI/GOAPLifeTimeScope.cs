using BehaviourTree;
using BehaviourTree.Debagger;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class GoapLifeTimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IBTDebugger, BTDebugger>(Lifetime.Singleton);
        }
    }
}