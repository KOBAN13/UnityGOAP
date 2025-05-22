using UnityEngine;

namespace Game.Core.Health
{
    public class HealthConfig : ScriptableObject
    {
        [field: SerializeField]
        [field: Range(10, 1000)]
        public float MaxHealth { get; protected set; }
    }
}