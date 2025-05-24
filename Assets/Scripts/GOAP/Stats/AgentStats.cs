using System;
using Health;
using Health.Interface;
using R3;

namespace GOAP.Stats
{
    public class AgentStats : IDisposable
    {
        private readonly RestoringHealthCharacter _restoringHealthCharacter;
        private readonly HealthCharacter _healthCharacterStats;
        private readonly OperationWithHealth _operationWithHealth;
        
        private readonly Subject<Unit> _hit = new();
        private readonly Subject<Unit> _die = new();

        public AgentStats(IHealthConfig playerHealthConfig, Action hitAction, Action dieAction) 
        {
            _operationWithHealth = new OperationWithHealth(_hit, _die);
            _healthCharacterStats = new HealthCharacter(_operationWithHealth, playerHealthConfig.MaxHealth);
            _restoringHealthCharacter = new RestoringHealthCharacter(_healthCharacterStats, playerHealthConfig);
            
            _operationWithHealth.SubscribeHit(hitAction);
            _operationWithHealth.SubscribeDead(dieAction);
        }
        
        public void SetDamage(float value) => _restoringHealthCharacter.SetDamage(value);
        public void AddHealth(float value) => _restoringHealthCharacter.AddHealth(value).Forget();

        public void Dispose()
        {
            _restoringHealthCharacter.Dispose();
        }
    }
}