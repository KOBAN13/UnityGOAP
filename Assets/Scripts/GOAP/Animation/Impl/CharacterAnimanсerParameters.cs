using GOAP.Animation.Interface;
using Helpers.SerializedDictionary;
using UnityEngine;

namespace GOAP.Animation.Impl
{
    [CreateAssetMenu(fileName = nameof(CharacterAnimanсerParameters), menuName = nameof(CharacterAnimanсerParameters))]
    public class CharacterAnimanсerParameters : ScriptableObject, ICharacterAnimanсerParameters
    {
        [field: SerializeField] public DictionaryAnimationCharacter AnimationCharacter { get; private set; }
    }
}