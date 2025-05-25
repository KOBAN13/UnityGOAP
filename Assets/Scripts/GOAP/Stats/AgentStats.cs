using System;
using R3;
using Stats;
using Stats.Interface;

namespace GOAP.Stats
{
    public class AgentStats : IDisposable
    {
        public float CurrentHealth => _healthCharacter.CurrentHealth.CurrentValue;
        public float CurrentStamina => _staminaCharacter.CurrentStamina.CurrentValue;
        
        private readonly RestoringHealthCharacter _restoringHealthCharacter;
        private readonly HealthCharacter _healthCharacter;
        private readonly OperationWithHealth _operationWithHealth;
        private readonly StaminaCharacter _staminaCharacter;
        
        private readonly Subject<Unit> _onHit = new();
        private readonly Subject<Unit> _onDie = new();
        private readonly Subject<Unit> _onStateChange = new();
        
        private readonly CompositeDisposable _disposable = new();
        

        public AgentStats(IHealthConfig playerHealthConfig, IStaminaConfig staminaConfig, Action hitAction, Action dieAction) 
        {
            _staminaCharacter = new StaminaCharacter(staminaConfig.MaxStamina);
            _operationWithHealth = new OperationWithHealth(_onHit, _onDie);
            _healthCharacter = new HealthCharacter(_operationWithHealth, playerHealthConfig.MaxHealth);
            _restoringHealthCharacter = new RestoringHealthCharacter(_healthCharacter, playerHealthConfig);
            
            _operationWithHealth.SubscribeHit(hitAction);
            _operationWithHealth.SubscribeDead(dieAction);
            
            var healthSensorStream = _healthCharacter.CurrentHealth.Select(isDetected => (isDetected, "HealthSensor"));
            var staminaSensorStream = _staminaCharacter.CurrentStamina.Select(isDetected => (isDetected, "StaminaSensor"));
            
            var mergeStream = Observable.Merge(healthSensorStream, staminaSensorStream);
            
            mergeStream.Subscribe(sensorData => _onStateChange.OnNext(Unit.Default)).AddTo(_disposable);
        }
        
        public void SetDamage(float value) => _healthCharacter.SetDamage(value);
        
        public void AddHealth(float value) => _healthCharacter.AddHealth(value).Forget();
        
        public bool IsHealthLow(float healthMin) => _healthCharacter.CurrentHealth.CurrentValue <= healthMin;
        
        public bool IsStaminaLow(float staminaMin) => _staminaCharacter.CurrentStamina.CurrentValue <= staminaMin;
        
        public void SetFatigue(float value) => _staminaCharacter.SetFatigue(value);
        
        public void AddStamina(float value) => _staminaCharacter.AddStamina(value);
        
        public void SubscribeStateChange(Action stateChange) 
            => _onStateChange.Subscribe(_ => stateChange()).AddTo(_disposable);

        public void Dispose()
        {
            _disposable.Dispose();
            _healthCharacter.Dispose();
            _operationWithHealth.Dispose();
            _restoringHealthCharacter.Dispose();
        }
    }
}