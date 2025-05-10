using CharacterScripts;
using InputSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class CharacterLifeTimeScope : LifetimeScope
    {
        [SerializeField] private PlayerComponents _playerComponents;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_playerComponents);

            builder.Register<NewInputSystem>(Lifetime.Singleton);
            builder.Register<IInputSystem, InputSystemPC>(Lifetime.Singleton);
            builder.Register<IRotate, Rotate>(Lifetime.Singleton);
            builder.Register<IMovable, Movement>(Lifetime.Singleton);
            builder.Register<Player>(Lifetime.Singleton);
        }
    }
}