using Helpers.SerializedDictionary;
using UnityEngine;

namespace GOAP.Animation
{
    [CreateAssetMenu(fileName = nameof(CharacterAnimanсerParameters), menuName = nameof(CharacterAnimanсerParameters))]
    public class CharacterAnimanсerParameters : ScriptableObject
    {
        [field: SerializeField] public DictionaryAnimationCharacter AnimationCharacter { get; private set; }
    }
}