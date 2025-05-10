using UnityEngine;

namespace CharacterScripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerComponents : MonoBehaviour
    {
        [field: SerializeField] public CharacterController CharacterController { get; private set; }
        [field: SerializeField] public Transform PlayerTransform { get; private set; }
        
        [field: Header("Movement")]
        [field: SerializeField] public float SpeedMove { get; private set; }
        [field: SerializeField] public float SpeedRotate { get; private set; }
        
        public float TargetDirectionY { get; set; }
    }
}