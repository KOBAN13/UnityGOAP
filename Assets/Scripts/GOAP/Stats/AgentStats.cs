using System;
using R3;
using Stats;
using Stats.Interface;

namespace GOAP.Stats
{
    public class AgentStats : IDisposable
    {
        public float CurrentHealth => _healthCharacterStats.CurrentHealth.CurrentValue;
        public float CurrentStamina => _staminaCharacter.CurrentStamina.CurrentValue;
        
        private readonly RestoringHealthCharacter _restoringHealthCharacter;
        private readonly HealthCharacter _healthCharacterStats;
        private readonly OperationWithHealth _operationWithHealth;
        private readonly StaminaCharacter _staminaCharacter;
        
        private readonly Subject<Unit> _hit = new();
        private readonly Subject<Unit> _die = new();

        public AgentStats(IHealthConfig playerHealthConfig, IStaminaConfig staminaConfig, Action hitAction, Action dieAction) 
        {
            _staminaCharacter = new StaminaCharacter(staminaConfig.MaxStamina);
            _operationWithHealth = new OperationWithHealth(_hit, _die);
            _healthCharacterStats = new HealthCharacter(_operationWithHealth, playerHealthConfig.MaxHealth);
            _restoringHealthCharacter = new RestoringHealthCharacter(_healthCharacterStats, playerHealthConfig);
            
            _operationWithHealth.SubscribeHit(hitAction);
            _operationWithHealth.SubscribeDead(dieAction);
        }
        
        public void SetDamage(float value) => _restoringHealthCharacter.SetDamage(value);
        
        public void AddHealth(float value) => _restoringHealthCharacter.AddHealth(value).Forget();
        
        public bool IsHealthLow(float healthMin) => _healthCharacterStats.CurrentHealth.CurrentValue <= healthMin;
        
        public bool IsStaminaLow(float staminaMin) => _staminaCharacter.CurrentStamina.CurrentValue <= staminaMin;
        
        public void SetFatigue(float value) => _staminaCharacter.SetFatigue(value);
        
        public void AddStamina(float value) => _staminaCharacter.AddStamina(value);

        public void Dispose()
        {
            _restoringHealthCharacter.Dispose();
        }
    }
}