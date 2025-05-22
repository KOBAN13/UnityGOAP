using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Health
{
    public class Health : IHealthStats, IDisposable
    {
        public float MaxHealth { get; }
        public float CurrentHealth { get; private set; }
        
        private float _amountHealthPercentage = 1f;
        private const float TransferFromInterest = 100f;
        private CancellationTokenSource _cancellationTokenSource;
        
        private readonly OperationWithHealth _operationWithHealth;

        public Health(OperationWithHealth operationWithHealth, float health)
        {
            _operationWithHealth = operationWithHealth;
            MaxHealth = CurrentHealth = health;
        }

        public void SetDamage(float value)
        {
            if (value < 0) throw new ArgumentException($"The Argument {nameof(value)} cannot be <0");

            CurrentHealth = Mathf.Clamp(CurrentHealth - value, 0f, MaxHealth);

            _amountHealthPercentage -= value / MaxHealth;
            
            if (CurrentHealth != 0f) return;
            
            _operationWithHealth.InvokeDead();
        }

        public async UniTaskVoid AddHealth(float value)
        {
            if (value < 0) throw new ArgumentException($"The Argument {nameof(value)} cannot be < 0");

            CurrentHealth = Mathf.Clamp(value * TransferFromInterest, 0f, MaxHealth);

            _amountHealthPercentage = value;
            
            await UniTask.Yield();
        }

        public void Unsubscribe()
        {
            Dispose();
        }

        public void Subscribe()
        {
            //empty
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }
    }
}