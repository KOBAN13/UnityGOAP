using UnityEngine;

namespace Game.Core.Health
{
    [CreateAssetMenu(fileName = nameof(PlayerHealthConfig), menuName = "Configs" + "/" + nameof(PlayerHealthConfig))]
    public class PlayerHealthConfig : HealthConfig
    {
        [field: SerializeField]
        [field: Range(0.01f, 1f)]
        public float CoefficientRecoveryHealth { get; private set; }
        [field: SerializeField]
        [field: Range(0f, 100f)]
        public float TimeRecoveryHealth { get; private set; }
        [field: SerializeField]
        [field: Range(0.01f, 1f)]
        public float CoefficientRecoveryHealthAfterEnemyDead { get; private set; }
    }
}