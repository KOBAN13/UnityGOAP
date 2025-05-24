using Animancer;
using GOAP.Animation;
using GOAP.Animation.Impl;
using GOAP.Animation.Interface;
using Health;
using Health.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Config.Installers
{
    public class GameSettingsInstallerScope : LifetimeScope
    {
        [SerializeField] private CharacterAnimanсerParameters _characterAnimanсerParameters;
        [SerializeField] private HealthConfig _healthConfig;
        [SerializeField] private AnimancerComponent _animancerComponent;

        protected override void Configure(IContainerBuilder builder)
        {
            BindCharacterAnimancerParameters(builder);
            BindHealthConfig(builder);
        }
        
        private void BindCharacterAnimancerParameters(IContainerBuilder container)
        {
            container
                .RegisterInstance(_characterAnimanсerParameters)
                .As<ICharacterAnimanсerParameters>();
            
            container
                .RegisterInstance(_animancerComponent)
                .AsImplementedInterfaces()
                .AsSelf();
            
            container
                .Register<AnimationBrain>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
        }
        
        private void BindHealthConfig(IContainerBuilder container)
        {
            container
                .RegisterInstance(_healthConfig)
                .As<IHealthConfig>();
        }
    }
}