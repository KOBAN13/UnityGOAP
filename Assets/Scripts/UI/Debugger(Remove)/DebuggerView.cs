using BehaviourTree;
using ObservableCollections;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace DI.Debugger_Remove_
{
    public class DebuggerView : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI Name { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Type { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Status { get; private set; }

        private IBTDebugger _debugger;
        private readonly CompositeDisposable _compositeDisposable = new();

        [Inject]
        public void Construct(IBTDebugger debugger) => _debugger = debugger;
        
        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            _compositeDisposable.Clear();
            _compositeDisposable.Dispose();
        }

        private void Subscribe()
        {
            ShowName();
            ShowType();
            ShowStatus();
        }

        private void ShowName()
        {
            _debugger.NameNode
                .ObserveAdd()
                .Subscribe(_ =>
                {
                    var list = _debugger.GetNameNode();
                    list.Reverse();
                    Name.text = string.Join("-", list);
                })
                .AddTo(_compositeDisposable);
        }

        private void ShowType()
        {
            _debugger.TypeNode
                .ObserveAdd()
                .Subscribe(_ =>
                {
                    var list = _debugger.GetTypeNode();
                    list.Reverse();
                    Type.text = string.Join("-", list);
                })
                .AddTo(_compositeDisposable);
        }

        private void ShowStatus()
        {
            _debugger.NodeStatus
                .Skip(1)
                .Subscribe(_ => Status.text = _debugger.GetStatusDebug(_debugger.NodeStatus.Value))
                .AddTo(_compositeDisposable);
        }
    }
}