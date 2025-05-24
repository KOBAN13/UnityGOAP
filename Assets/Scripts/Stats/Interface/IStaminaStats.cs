namespace Stats.Interface
{
    public interface IStaminaStats
    {
        float MaxStamina { get; }
        float CurrentStamina { get; }
        void SetFatigue(float value);
        void AddStamina(float value);
    }
}