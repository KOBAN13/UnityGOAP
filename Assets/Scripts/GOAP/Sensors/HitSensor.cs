using CharacterScripts;
using R3;
using UnityEngine;

namespace GOAP
{
    [RequireComponent(typeof(SphereCollider))]
    public class HitSensor : MonoBehaviour, ISensor
    {
        [SerializeField] private float _timeAggression;
        [SerializeField] private float _radiusDetect;
        [SerializeField] private SphereCollider _trigger;
        public Vector3 Target { get; private set; }
        public ReadOnlyReactiveProperty<bool> IsActiveSensor => _isActiveSensor;
        public bool IsActivate => _isActiveSensor.Value;
        private readonly ReactiveProperty<bool> _isActiveSensor = new();
        private readonly CompositeDisposable _compositeDisposable = new();
        
        private void Awake()
        {
            _trigger.isTrigger = true;
            _trigger.radius = _radiusDetect;
        }

        private void UnsubscribeUpdate()
        {
            _compositeDisposable.Clear();
            _compositeDisposable.Dispose();
        }

        private void OnDestroy()
        {
            UnsubscribeUpdate();
            _isActiveSensor.Dispose();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerComponents playerComponents)) return;
                
            Target = playerComponents.transform.position;
            _isActiveSensor.Value = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out PlayerComponents playerComponents)) return;
                
            Target = Vector3.zero;
            _isActiveSensor.Value = false;
        }
    }
}