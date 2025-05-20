using GOAP.Animation;
using GOAP.Animation.Impl;
using GOAP.Animation.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Config.Installers
{
    public class GameSettingsInstallerScope : LifetimeScope
    {
        [SerializeField] private CharacterAnimanсerParameters _characterAnimanсerParameters;

        protected override void Configure(IContainerBuilder builder)
        {
            BindCharacterAnimancerParameters(builder);
        }
        
        private void BindCharacterAnimancerParameters(IContainerBuilder container)
        {
            container
                .RegisterInstance(_characterAnimanсerParameters)
                .As<ICharacterAnimanсerParameters>();
            
            container
                .Register<AnimationBrain>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}