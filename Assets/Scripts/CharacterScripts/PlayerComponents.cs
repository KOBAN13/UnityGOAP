using UnityEngine;

namespace CharacterScripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerComponents : MonoBehaviour
    {
        [field: SerializeField] public CharacterController CharacterController { get; private set; }
        [field: SerializeField] public Transform PlayerTransform { get; private set; }
        [field: SerializeField] public Camera Camera { get; private set; }
        
        public float TargetDirectionY { get; set; }
    }
}