using UnityEngine;

namespace Health.Configs
{
    public class HealthConfig : ScriptableObject
    {
        [field: SerializeField]
        [field: Range(10, 1000)]
        public float MaxHealth { get; protected set; }
    }
}