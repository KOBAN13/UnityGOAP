using System;
using System.Threading;
using BlackboardScripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GOAP.Animation;
using GOAP.Animation.Enums;
using Helpers.Constants;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GOAP
{
    public class IdleStrategy : IActionStrategy
    {
        public bool CanPerform => true;
        public bool Complete { get; private set; }
        
        private readonly float _duration;
        private readonly Transform _enemy;
        private readonly AnimationBrain _animationBrain;

        private const int ITERATION = 4;
        
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public IdleStrategy(float duration, BlackboardController blackboardController)
        {
            _duration = duration + 3f;
            _enemy = blackboardController.GetValue<Transform>(NameAIKeys.AgentTransform);
            _animationBrain = blackboardController.GetValue<AnimationBrain>(NameAIKeys.AnimationBrain);
        }

        public async void Start()
        {
            Complete = false;
            
            _animationBrain.SetDefaultAnimation(EMovementAnimationType.Idle);
            
            for (var i = 0; i < ITERATION; i++)
            {
                _animationBrain.PlayForce(new AnimationRequest(GetRandomMovementAnimationType(), true, EAnimationLayer.Default, 0.8f));
                
                await UniTask.Delay(TimeSpan.FromSeconds(_duration / ITERATION), cancellationToken: _cancellationTokenSource.Token).SuppressCancellationThrow();
            }

            Complete = true;
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            Complete = true;
        }
        
        public EMovementAnimationType GetRandomMovementAnimationType()
        {
            var randomValue = Random.Range(0, 2);
            
            return randomValue == 0 ? EMovementAnimationType.TurnRight : EMovementAnimationType.TurnLeft;
        }
    }
}