using System;
using BlackboardScripts;
using GOAP.Animation;
using GOAP.Animation.Enums;
using Helpers.Constants;
using R3;
using UnityEngine;

namespace GOAP
{
    public class RestStrategy : IActionStrategy
    {
        public bool CanPerform  => !Complete;
        public bool Complete { get; private set; }
        
        private readonly AnimationBrain _animationBrain;
        private readonly float _duration;
        private CompositeDisposable _disposable = new();

        public RestStrategy(BlackboardController blackboardController, float duration)
        {
            _duration = duration;
            _animationBrain = blackboardController.GetValue<AnimationBrain>(NameAIKeys.AnimationBrain);
        }

        public void Start()
        {
            Debug.Log("Start Rest");
            
            _disposable = new CompositeDisposable();
            
            Observable.Timer(TimeSpan.FromSeconds(_duration))
                .Subscribe(_ => Complete = true)
                .AddTo(_disposable);
            
            RestStart();
        }

        public void Stop()
        {
            RestStop();
            
            _disposable?.Clear();
        }

        private void RestStart()
        {
            _animationBrain.SetDefaultAnimation(EMovementAnimationType.SitDown);
            _animationBrain.PlayForce(new AnimationRequest(EMovementAnimationType.Sit, false, EAnimationLayer.Default, 0.8f));
        }
        
        private void RestStop()
        {
            _animationBrain.SetDefaultAnimation(EMovementAnimationType.Idle);
            _animationBrain.PlayForce(new AnimationRequest(EMovementAnimationType.StandUp, false, EAnimationLayer.Default, 0.8f));
        }
    }
}