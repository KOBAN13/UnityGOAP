using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace GOAP
{
    public class EyesSensor : MonoBehaviour, ISensor
    {
        public Vector3 Target { get; private set; }
        public bool IsActivate => _isActiveSensor.Value; 
        
        private IReadOnlyList<GameObject> Objects => _objects;
        [field: SerializeField] private float distance;
        [field: SerializeField] private float angle;
        [field: SerializeField] private float height;
        [field: SerializeField] private LayerMask playerMask;
        [field: SerializeField] private LayerMask occlusionLayers;
        [field: SerializeField] private float delayScan;
        
        private readonly Collider[] _colliders = new Collider[50];
        private readonly WedgeMeshBuilder _wedgeMeshBuilder = new();
        private readonly List<GameObject> _objects = new();
        private readonly ReactiveProperty<bool> _isActiveSensor = new(false);

        private Mesh _eyesSensor;
        private IDisposable _disposable;
        
        public IReadOnlyReactiveProperty<bool> IsActiveSensor => _isActiveSensor;
        
        private void OnEnable()
        {
            _disposable = Observable
                .Timer(TimeSpan.FromSeconds(delayScan), TimeSpan.FromSeconds(delayScan))
                .Subscribe(_ => Scan());
        }

        private void OnDisable()
        {
            _disposable.Dispose();
        }

        private void Scan()
        {
            _objects.Clear();
            
            var countObject = Physics.OverlapSphereNonAlloc(transform.position, distance, _colliders, playerMask,
                QueryTriggerInteraction.Collide);

            for (int i = 0; i < countObject; i++)
            {
                if (IsInSight(_colliders[i].gameObject))
                {
                    _objects.Add(_colliders[i].gameObject);
                }
            }
            
            SetTarget();
        }

        private void SetTarget()
        {
            Target = Objects.Count > 0 && Objects[0]?.TryGetComponent(out PlayerComponents component) == true
                ? Objects[0].transform.position 
                : Vector3.zero;

            _isActiveSensor.Value = Target != Vector3.zero;
        }

        private bool IsInSight(GameObject obj)
        {
            var origin = transform.position;
            var dest = obj.transform.position;
            var direction = dest - origin;

            if (direction.y > height || direction.y < -1) return false;

            direction.y = 0;
            var deltaAngle = Vector3.Angle(direction, transform.forward);

            if (deltaAngle > angle) return false;

            origin.y += height / 2;
            dest.y = origin.y;

            return !Physics.Linecast(origin, dest, occlusionLayers);
        }

        private void OnValidate()
        {
            _eyesSensor = _wedgeMeshBuilder
                .WithAngle(angle)
                .WithDistance(distance)
                .WithHeight(height)
                .WithSegment(10)
                .BuildMesh();
        }

        private void OnDrawGizmos()
        {
            if (!_eyesSensor) return;
            
            Gizmos.color = Color.blue;
            Gizmos.DrawMesh(_eyesSensor, transform.position, transform.rotation);

            foreach (var obj in Objects)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(obj.transform.position, 0.2f);
            }
        }
    }
}