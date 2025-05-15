using System;
using System.Threading;
using BlackboardScripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Helpers.Constants;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GOAP
{
    public class IdleStrategy : IActionStrategy
    {
        private readonly float _duration;
        private readonly Transform _enemy;
        public bool CanPerform => true;
        public bool Complete { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        public IdleStrategy(float duration, BlackboardController blackboardController)
        {
            _duration = duration + 3f;
            _enemy = blackboardController.GetValue<Transform>(NameAIKeys.TransformAI);;
        }

        public async void Start()
        {
            CancellationTokenSource = new CancellationTokenSource();
            Complete = false;
            for (int i = 0; i < 3; i++)
            {
                await _enemy.DORotateQuaternion(Quaternion.Euler(0f, Random.Range(0f, 45f), 0f), _duration / 3)
                    .WithCancellation(CancellationTokenSource.Token).SuppressCancellationThrow();
                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: CancellationTokenSource.Token).SuppressCancellationThrow();
            }

            Complete = true;
        }

        public void Stop()
        {
            CancellationTokenSource?.Cancel();
            Complete = true;
        }
    }
}