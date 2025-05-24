using Cysharp.Threading.Tasks;
using R3;

namespace Stats.Interface
{
    public interface IHealthStats
    {
        float MaxHealth { get; } 
        ReadOnlyReactiveProperty<float> CurrentHealth { get; }
        void SetDamage(float value);
        UniTaskVoid AddHealth(float value);
        void Unsubscribe();
        void Subscribe();
    }
}