using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Health.Configs;
using R3;
using UnityEngine;

namespace Health
{
    public class RestoringHealth<T> : IHealthStats, IHealthRestoring, IDisposable where T : MonoBehaviour
    {
        public float MaxHealth { get; private set; }
        public float CurrentHealth { get; private set; }

        public bool IsHealthRestoringAfterHitEnemy { get; set; } = false;
        public bool IsHealthRestoringAfterDieEnemy { get; set; } = false;

        private float _previouslyValue;
        private float _currencyValue;
        private Tween _tween;
        
        private readonly IHealthStats _healthStats;
        private readonly PlayerHealthConfig _playerHealthConfig;
        private readonly CompositeDisposable _compositeDisposable = new();
        private const float TransferToInterest = 0.01f;
        private CancellationTokenSource _cancellationTokenSource;

        public RestoringHealth(IHealthStats healthStats, PlayerHealthConfig playerHealthConfig)
        {
            MaxHealth = CurrentHealth = playerHealthConfig.MaxHealth;
            _healthStats = healthStats;
            _playerHealthConfig = playerHealthConfig;
        }

        public void SetDamage(float value) => _healthStats.SetDamage(value);

        public async UniTaskVoid AddHealth(float value = 0f)
        {
            var restoringHealth = MaxHealth * _playerHealthConfig.CoefficientRecoveryHealth * TransferToInterest;
            IsHealthRestoringAfterHitEnemy = true;
            _previouslyValue = _healthStats.CurrentHealth;
            _cancellationTokenSource = new CancellationTokenSource();

            if (IsHealthRestoringAfterDieEnemy)
                _healthStats.AddHealth(MaxHealth * _playerHealthConfig.CoefficientRecoveryHealthAfterEnemyDead *
                                       TransferToInterest);

            var tcs = new UniTaskCompletionSource<bool>();
            
            _tween = DOTween.To(
                    () => 0f,
                    x =>
                    {
                        _healthStats.AddHealth(x + _previouslyValue);
                        _currencyValue = _healthStats.CurrentHealth;
                    },
                    restoringHealth,
                    _playerHealthConfig.TimeRecoveryHealth
                )
                .SetEase(Ease.Linear)
                .OnComplete(() => tcs.TrySetResult(true))
                .OnKill(() => tcs.TrySetCanceled());

            try
            {
                await tcs.Task.AttachExternalCancellation(_cancellationTokenSource.Token);
                
                await UniTask.Delay(TimeSpan.FromSeconds(0.7f), cancellationToken: _cancellationTokenSource.Token);
            }
            
            catch (OperationCanceledException)
            {
                Debug.Log("Восстановление здоровья отменено");
            }
            
            finally
            {
                IsHealthRestoringAfterHitEnemy = false;
                _tween?.Kill();
            }
        }

        public void Unsubscribe()
        {
            _healthStats.Unsubscribe();
            Dispose();
        }

        public void Subscribe()
        {
            _healthStats.Subscribe();
        }

        public void Dispose()
        {
            _compositeDisposable.Clear();
            _compositeDisposable.Dispose();
        }
    }
}