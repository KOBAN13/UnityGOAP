using Stats.Interface;
using UnityEngine;

namespace Stats.Impl
{
    [CreateAssetMenu(fileName = nameof(HealthConfig), menuName = "Stats" + "/" + nameof(HealthConfig))]
    public class HealthConfig : ScriptableObject, IHealthConfig
    {
        [field: SerializeField]
        [field: Range(10, 1000)]
        public float MaxHealth { get; private set; }
        
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