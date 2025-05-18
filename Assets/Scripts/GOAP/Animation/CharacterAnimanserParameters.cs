using Blackboard.SerializeableDictionary;
using UnityEngine;

namespace GOAP.Animation
{
    [CreateAssetMenu(fileName = nameof(CharacterAnimanserParameters), menuName = nameof(CharacterAnimanserParameters))]
    public class CharacterAnimanserParameters : ScriptableObject
    {
        [SerializeField] public DictionaryAnimationCharacter AnimationCharacter { get; private set; }
    }
}