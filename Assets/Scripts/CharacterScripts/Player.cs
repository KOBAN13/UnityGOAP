using System;
using InputSystem;
using R3;
using UnityEditor;
using VContainer.Unity;

namespace CharacterScripts
{
    public class Player : IInitializable, IDisposable, ITickable
    {
        private readonly IMovable _movable;
        private readonly IInputSystem _input;
        private readonly IRotate _rotate;
        private readonly CompositeDisposable _compositeDisposable = new();
        private PlayerSettings _playerSettings;

        public Player(IMovable movable, IInputSystem input, IRotate rotate)
        {
            _movable = movable;
            _input = input;
            _rotate = rotate;
        }

        public void Dispose()
        {
            _compositeDisposable.Clear();
            _compositeDisposable.Dispose();
        }

        public void Initialize()
        {
            _input.MouseClick.Skip(1)
                .Subscribe(async vector =>
                {
                    await _rotate.RotateCharacter(vector);
                })
                .AddTo(_compositeDisposable);
            
        }

        public void Tick()
        {
            _movable.Move(_input.Input, 5f);
        }
    }
}