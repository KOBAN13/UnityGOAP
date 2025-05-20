using System;
using System.Threading;
using Animancer;
using Cysharp.Threading.Tasks;
using GOAP.Animation.Enums;
using GOAP.Animation.Interface;
using UnityEngine;

namespace GOAP.Animation
{
    public class AnimationBrain : IAnimationBrain
    {
        private readonly AnimancerComponent _animancerComponent;
        private readonly ICharacterAnimanсerParameters _characterAnimancerParameters;
        
        private CancellationTokenSource _cancellationToken;
        private EMovementAnimationType _currentForceAnimationType;
        private EMovementAnimationType _defaultAnimationType;

        public AnimationBrain(
            ICharacterAnimanсerParameters characterAnimancerParameters, 
            AnimancerComponent animancerComponent
        )
        {
            _characterAnimancerParameters = characterAnimancerParameters;
            _animancerComponent = animancerComponent;
        }

        public void PlayAnimation(EMovementAnimationType type, float fadeTime = 0.2f)
        {
            var clip = GetClip(type);
            
            _animancerComponent.Play(clip, fadeTime);
        }

        public void SetDefaultAnimation(EMovementAnimationType type, float fadeTime = 0.2f)
        {
            _defaultAnimationType = type;
            
            if(_currentForceAnimationType == EMovementAnimationType.None)
                return;
            
            _cancellationToken?.Cancel();
            _currentForceAnimationType = EMovementAnimationType.None;

            _animancerComponent.Play(GetClip(type), fadeTime);
        }

        public void PlayForce(AnimationRequest request)
        {
            _cancellationToken?.Cancel();
            _currentForceAnimationType = request.Type;

            _cancellationToken = new CancellationTokenSource();
            
            PlayForceAnimation(request).Forget();
        }

        private MotionClipTransition GetClip(EMovementAnimationType type)
        {
            if(!_characterAnimancerParameters.AnimationCharacter.TryGetValue(type, out var animationClip))
                Debug.LogError($"Dont find animation clip by type: {type}");

            return animationClip;
        }

        private async UniTaskVoid PlayForceAnimation(AnimationRequest request)
        {
            var clip = GetClip(request.Type);

            var state = _animancerComponent.Play(clip, request.FadeDuration);
            
            if(request.Speed != 0f)
                state.Speed = request.Speed;
            
            state.Root.Component.Animator.applyRootMotion = request.ApplyRootMotion;
            
            state.LayerIndex = (int)request.AnimationLayer;
            
            await UniTask.Delay(TimeSpan.FromSeconds(clip.Clip.length * request.Delta), cancellationToken: _cancellationToken.Token)
                .SuppressCancellationThrow();

            _animancerComponent.Play(GetClip(_defaultAnimationType), clip.Clip.length * (1 - request.Delta));
            
            _currentForceAnimationType = EMovementAnimationType.None;
            _cancellationToken = null;
        }
    }
}