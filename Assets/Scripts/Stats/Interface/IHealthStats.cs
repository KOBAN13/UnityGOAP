using Cysharp.Threading.Tasks;
using R3;

namespace Stats.Interface
{
    public interface IHealthStats
    {
        float MaxHealth { get; } 
        ReadOnlyReactiveProperty<float> CurrentHealth { get; }
        ReadOnlyReactiveProperty<float> CurrentHealthPercentage { get; }
        void SetDamage(float value);
        UniTaskVoid AddHealth(float value);
    }
}