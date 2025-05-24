using Customs;
using Stats.Interface;
using UnityEngine;

namespace Stats
{
    public class StaminaCharacter : IStaminaStats
    {
        public float MaxStamina { get; private set; }
        public float CurrentStamina { get; private set; }
        
        public StaminaCharacter(float maxStamina) 
            => MaxStamina = CurrentStamina = maxStamina;
        
        public void SetFatigue(float value)
        {
            Preconditions.CheckValidateData(value);
            
            CurrentStamina = Mathf.Clamp(CurrentStamina - value, 0f, MaxStamina);
        }

        public void AddStamina(float value)
        {
            Preconditions.CheckValidateData(value);
            
            CurrentStamina = Mathf.Clamp(CurrentStamina + value, 0f, MaxStamina);
        }
    }
}