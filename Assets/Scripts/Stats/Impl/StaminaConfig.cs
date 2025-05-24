using Stats.Interface;
using UnityEngine;

namespace Stats.Impl
{
    [CreateAssetMenu(fileName = nameof(StaminaConfig), menuName = "Stats" + "/" + nameof(StaminaConfig))]
    public class StaminaConfig : ScriptableObject, IStaminaConfig
    {
        [field: SerializeField] public float MaxStamina { get; private set; }
    }
}