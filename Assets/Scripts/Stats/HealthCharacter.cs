using System;
using System.Threading;
using Customs;
using Cysharp.Threading.Tasks;
using R3;
using Stats.Interface;
using UnityEngine;

namespace Stats
{
    public class HealthCharacter : IHealthStats, IDisposable
    {
        public float MaxHealth { get; }
        public ReadOnlyReactiveProperty<float> CurrentHealth => _currentHealth;
        public ReadOnlyReactiveProperty<float> CurrentHealthPercentage => _amountHealthPercentage;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        private readonly ReactiveProperty<float> _amountHealthPercentage = new(1f);
        private readonly ReactiveProperty<float> _currentHealth = new();
        private readonly OperationWithHealth _operationWithHealth;
        
        public HealthCharacter(OperationWithHealth operationWithHealth, float health)
        {
            _operationWithHealth = operationWithHealth;
            MaxHealth = _currentHealth.Value = health;
        }

        public void SetDamage(float value)
        {
            Preconditions.CheckValidateData(value);

            _currentHealth.Value = Mathf.Clamp(_currentHealth.Value - value, 0f, MaxHealth);

            _amountHealthPercentage.Value = Mathf.Clamp(_amountHealthPercentage.Value - value / MaxHealth, 0f, 1f);
            
            if (_currentHealth.Value != 0f) 
                return;
            
            _operationWithHealth.InvokeDead();
        }

        public async UniTaskVoid AddHealth(float value)
        {
            Preconditions.CheckValidateData(value);

            _currentHealth.Value = Mathf.Clamp(value + _currentHealth.Value, 0f, MaxHealth);

            _amountHealthPercentage.Value = Mathf.Clamp(_currentHealth.Value / MaxHealth, 0f, 1f);
            
            await UniTask.Yield();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            _currentHealth?.Dispose();
        }
    }
}