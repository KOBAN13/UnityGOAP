using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using Stats.Interface;
using UnityEngine;

namespace Stats
{
    public class RestoringHealthCharacter : IHealthStats, IHealthRestoring, IDisposable
    {
        public float MaxHealth { get; private set; }
        public ReadOnlyReactiveProperty<float> CurrentHealth => _currentHealth;
        public bool IsHealthRestoringAfterHitEnemy { get; set; } = false;
        public bool IsHealthRestoringAfterDieEnemy { get; set; } = false;

        private float _previouslyValue;
        private float _currencyValue;
        private Tween _tween;
        
        private readonly IHealthStats _healthStats;
        private readonly IHealthConfig _healthConfig;
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly ReactiveProperty<float> _currentHealth = new();
        private const float TransferToInterest = 0.01f;
        private CancellationTokenSource _cancellationTokenSource;

        public RestoringHealthCharacter(IHealthStats healthStats, IHealthConfig healthConfig)
        {
            MaxHealth = _currentHealth.Value = healthConfig.MaxHealth;
            _healthStats = healthStats;
            _healthConfig = healthConfig;
        }

        public void SetDamage(float value) => _healthStats.SetDamage(value);

        public async UniTaskVoid AddHealth(float value = 0f)
        {
            var restoringHealth = MaxHealth * _healthConfig.CoefficientRecoveryHealth * TransferToInterest;
            IsHealthRestoringAfterHitEnemy = true;
            _previouslyValue = _healthStats.CurrentHealth.CurrentValue;
            _cancellationTokenSource = new CancellationTokenSource();

            if (IsHealthRestoringAfterDieEnemy)
                _healthStats.AddHealth(MaxHealth * _healthConfig.CoefficientRecoveryHealthAfterEnemyDead *
                                       TransferToInterest);

            var tcs = new UniTaskCompletionSource<bool>();
            
            _tween = DOTween.To(
                    () => 0f,
                    x =>
                    {
                        _healthStats.AddHealth(x + _previouslyValue);
                        _currencyValue = _healthStats.CurrentHealth.CurrentValue;
                    },
                    restoringHealth,
                    _healthConfig.TimeRecoveryHealth
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