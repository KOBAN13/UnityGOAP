using CharacterScripts;
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

            builder.Register<NewInputSystem>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<InputSystemPC>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<Rotate>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<Movement>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<Player>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}