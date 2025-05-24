using Cysharp.Threading.Tasks;

namespace Health.Interface
{
    public interface IHealthStats
    {
        float MaxHealth { get; }
        float CurrentHealth { get; }
        void SetDamage(float value);
        UniTaskVoid AddHealth(float value);
        void Unsubscribe();
        void Subscribe();
    }
}