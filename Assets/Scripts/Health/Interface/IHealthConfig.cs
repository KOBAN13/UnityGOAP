namespace Health.Interface
{
    public interface IHealthConfig
    {
        float MaxHealth { get; }
        float CoefficientRecoveryHealth { get; }
        float TimeRecoveryHealth { get; }
        float CoefficientRecoveryHealthAfterEnemyDead { get; }
    }
}