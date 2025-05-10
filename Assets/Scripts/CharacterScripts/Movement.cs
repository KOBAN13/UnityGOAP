using UnityEngine;
using VContainer.Unity;

namespace CharacterScripts
{
    public class Movement : ITickable, IMovable
    {
        private readonly PlayerComponents _playerComponents;
        private Vector3 _targetDirection;
        private Vector2 _input;
        private float _speed;

        public Movement(PlayerComponents playerComponents)
        {
            _playerComponents = playerComponents;
        }
        
        public void Move(Vector2 input, float speed)
        {
            _input = input;
            _speed = speed;
        }

        public void Tick()
        {
            _targetDirection.x = _speed * _input.x;
            _targetDirection.z = _speed * _input.y;
            _targetDirection.y = _playerComponents.TargetDirectionY;
            _playerComponents.CharacterController.Move(_targetDirection * Time.deltaTime);
        }
    }
}