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
        private CompositeDisposable _compositeDisposable = new();
        private IDisposable _disposable;
        

        private void OnEnable()
        {
            _eyesSensor.IsActiveSensor.Subscribe(CheckEnemyVision).AddTo(_compositeDisposable);
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
            _disposable?.Dispose();
            _compositeDisposable?.Clear();
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