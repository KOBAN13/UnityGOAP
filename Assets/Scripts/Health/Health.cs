using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Game.Core.Health
{
    public class Health : IHealthStats, IDisposable
    {
        public float MaxHealth { get; }
        public float CurrentHealth { get; private set; }
        
        private Subject<Unit> _onCharacterDie;
        private float _amountHealthPercentage = 1f;
        private const float TransferFromInterest = 100f;
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        public Health(float health)
        {
            MaxHealth = CurrentHealth = health;
        }

        public void SetDamage(float value)
        {
            if (value < 0) throw new ArgumentException($"The Argument {nameof(value)} cannot be <0");

            CurrentHealth = Mathf.Clamp(CurrentHealth - value, 0f, MaxHealth);

            _amountHealthPercentage -= value / MaxHealth;
            
            if (CurrentHealth != 0f) return;
            
            _onCharacterDie.OnNext(Unit.Default);
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
            CancellationTokenSource?.Dispose();
        }
    }
}