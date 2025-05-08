using System;
using R3;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace GOAP
{
    public class EnemyVisionSensor : MonoBehaviour, ISensor
    {
        [field: SerializeField] private EyesSensor _eyesSensor;
        public Vector3 Target { get; }
        public bool IsActivate => _isActiveSensor.Value;
        private readonly ReactiveProperty<bool> _isActiveSensor = new();

        private bool _isFindEnemy;
        private IDisposable _disposable;

        private void OnEnable()
        {
            _eyesSensor.IsActiveSensor.Subscribe(CheckEnemyVision).AddTo(this);
        }

        private void CheckEnemyVision(bool isActive)
        {
            if (isActive)
            {
                _isFindEnemy = true;
            }

            if (!_isFindEnemy || isActive != false) return;
            Debug.LogWarning("Find Enemy");
            _isActiveSensor.Value = true;
            _isFindEnemy = false;

            _disposable?.Dispose();
            _disposable = Observable.Timer(TimeSpan.FromSeconds(9f))
                .Subscribe(_ =>
                {
                    _isActiveSensor.Value = false;
                    Debug.LogWarning("Stop Find Enemy");
                });
        }
    }
}