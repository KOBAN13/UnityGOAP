﻿using System;
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
        
        private CancellationTokenSource _cancellationTokenSource = new();

        public IdleStrategy(float duration, BlackboardController blackboardController)
        {
            _duration = duration + 3f;
            _enemy = blackboardController.GetValue<Transform>(NameAIKeys.AgentTransform);
            _animationBrain = blackboardController.GetValue<AnimationBrain>(NameAIKeys.AnimationBrain);
        }

        public async void Start()
        {
            Debug.Log("Start Idle");
            
            Complete = false;
            _cancellationTokenSource = new CancellationTokenSource();
            
            _animationBrain.SetDefaultAnimation(EMovementAnimationType.Idle);
            
            try
            {
                for (var i = 0; i < ITERATION; i++)
                {
                    _animationBrain.PlayForce(new AnimationRequest(GetRandomMovementAnimationType(), true, EAnimationLayer.Default, 0.8f));
                    
                    await UniTask.Delay(TimeSpan.FromSeconds(_duration / ITERATION),
                        cancellationToken: _cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Idle was cancelled");
            }
            finally
            {
                Complete = true;
            }
        }

        public void Stop()
        {
            Debug.Log("Stop Idle");
            
            _cancellationTokenSource?.Cancel();
            
            Complete = true;
        }
        
        private EMovementAnimationType GetRandomMovementAnimationType()
        {
            var randomValue = Random.Range(0, 2);
            
            return randomValue == 0 ? EMovementAnimationType.TurnRight : EMovementAnimationType.TurnLeft;
        }
    }
}