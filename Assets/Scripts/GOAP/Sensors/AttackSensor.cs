using CharacterScripts;
using R3;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace GOAP
{
    [RequireComponent(typeof(SphereCollider))]
    public class AttackSensor : MonoBehaviour, ISensor
    {
        public Vector3 Target => _target ? _target.transform.position : Vector3.zero;
        public bool IsActivate => _isActiveSensor.Value;
        public ReadOnlyReactiveProperty<bool> IsActiveSensor => _isActiveSensor;
        
        [SerializeField] private float _radiusDetect;
        [SerializeField] private SphereCollider _trigger;
        
        private GameObject _target;
        private Vector3 _lastKnownPosition;
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly ReactiveProperty<bool> _isActiveSensor = new();
        

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                UpdateTargetPosition(other.gameObject);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                UpdateTargetPosition();
            }
        }

        private void OnDisable()
        {
            _compositeDisposable?.Clear();
            _compositeDisposable?.Dispose();
        }

        private void Awake()    
        {
            _trigger.isTrigger = true;
            _trigger.radius = _radiusDetect;
        }

        private void UpdateTargetPosition(GameObject target = null)
        {
            _target = target;
            _isActiveSensor.Value = Target != Vector3.zero;

            if (!_isActiveSensor.Value || (_lastKnownPosition != Target && _lastKnownPosition != Vector3.zero)) return;
            _lastKnownPosition = Target;
        }
    }
}