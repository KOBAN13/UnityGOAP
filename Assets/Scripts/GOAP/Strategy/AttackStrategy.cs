using System;
using System.Threading;
using R3;
using UnityEngine;

namespace GOAP
{
    public class AttackStrategy : IActionStrategy
    {
        public bool CanPerform => true;
        public bool Complete { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; } = null;

        private CompositeDisposable _disposable;
        private readonly Subject<Unit> _timerCompletedSubject = new();

        public void Start()
        {
            Complete = false;
            _disposable = new CompositeDisposable();
            
            Observable.Interval(TimeSpan.FromSeconds(1f))
                .Subscribe(_ => Attack()).AddTo(_disposable);
            
            Observable.Timer(TimeSpan.FromSeconds(10f))
                .Do(_ => Complete = true)
                .Subscribe(_ => _timerCompletedSubject.OnNext(Unit.Default))
                .AddTo(_disposable);
            
            _timerCompletedSubject.Subscribe(_ => Complete = true)
                .AddTo(_disposable);
        }

        public void Stop()
        {
            ClearDisposable();
        }
        
        private void ClearDisposable()
        {
            _disposable?.Clear();
            _disposable?.Dispose();
        }

        private void Attack()
        {
            Debug.LogWarning("IM ATTACK");
        }
    }
}