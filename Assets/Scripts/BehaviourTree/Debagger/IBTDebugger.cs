using System.Collections.Generic;
using ObservableCollections;
using R3;

namespace BehaviourTree
{
    public interface IBTDebugger
    {
        ObservableList<string> NameNode { get; }
        ObservableList<string> TypeNode { get; }
        ReactiveProperty<BTNodeStatus> NodeStatus { get; }

        string GetStatusDebug(BTNodeStatus btNodeStatus);

        public List<string> GetNameNode();

        public List<string> GetTypeNode();
    }
}