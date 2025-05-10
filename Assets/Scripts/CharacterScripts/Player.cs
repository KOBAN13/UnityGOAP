using System;
using InputSystem;
using R3;
using VContainer.Unity;

namespace CharacterScripts
{
    public class Player : IDisposable, ITickable
    {
        private readonly IMovable _movable;
        private readonly IInputSystem _input;
        private readonly IRotate _rotate;
        private readonly PlayerComponents _playerComponents;
        private readonly CompositeDisposable _compositeDisposable = new();

        public Player(IMovable movable, IInputSystem input, IRotate rotate, PlayerComponents playerComponents)
        {
            _movable = movable;
            _input = input;
            _rotate = rotate;
            _playerComponents = playerComponents;
        }

        public void Dispose()
        {
            _compositeDisposable.Clear();
            _compositeDisposable.Dispose();
        }

        public void Tick()
        {
            _movable.Move(_input.Input, _playerComponents.SpeedMove);
            _rotate.RotateCharacter(_input.Input);
        }
    }
}