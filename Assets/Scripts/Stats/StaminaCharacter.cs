using Customs;
using R3;
using Stats.Interface;
using UnityEngine;

namespace Stats
{
    public class StaminaCharacter : IStaminaStats
    {
        public float MaxStamina { get; private set; }
        public ReadOnlyReactiveProperty<float> CurrentStamina => _currentStamina;
        
        private readonly ReactiveProperty<float> _currentStamina = new();
        
        public StaminaCharacter(float maxStamina) 
            => MaxStamina = _currentStamina.Value = maxStamina;
        
        public void SetFatigue(float value)
        {
            Preconditions.CheckValidateData(value);
            
            _currentStamina.Value = Mathf.Clamp(_currentStamina.Value - value, 0f, MaxStamina);
        }

        public void AddStamina(float value)
        {
            Preconditions.CheckValidateData(value);
            
            _currentStamina.Value = Mathf.Clamp(_currentStamina.Value + value, 0f, MaxStamina);
        }
    }
}