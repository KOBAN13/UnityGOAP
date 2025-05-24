using R3;

namespace Stats.Interface
{
    public interface IStaminaStats
    {
        float MaxStamina { get; }
        ReadOnlyReactiveProperty<float> CurrentStamina { get; }
        void SetFatigue(float value);
        void AddStamina(float value);
    }
}