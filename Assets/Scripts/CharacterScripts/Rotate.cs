using UnityEngine;

namespace CharacterScripts
{
    public class Rotate : IRotate
    {
        private readonly PlayerComponents _playerComponents;
        
        private Vector3 _targetDirection;
        
        public Rotate(PlayerComponents playerComponents)
        {
            _playerComponents = playerComponents;
        }

        public void RotateCharacter(Vector2 moveDirection)
        {
            if(Mathf.Approximately(moveDirection.sqrMagnitude, 0f)) return;
            
            var characterTransform = _playerComponents.PlayerTransform;
            _targetDirection = new Vector3(moveDirection.x, 0f, moveDirection.y);

            if (!(Vector3.Angle(characterTransform.forward, moveDirection) > 0f)) return;
            
            var newDirection = Vector3.RotateTowards(
                characterTransform.forward,
                _targetDirection, 
                _playerComponents.SpeedRotate,
                0f
            );
            
            characterTransform.rotation = Quaternion.LookRotation(newDirection);
        }
    }
}