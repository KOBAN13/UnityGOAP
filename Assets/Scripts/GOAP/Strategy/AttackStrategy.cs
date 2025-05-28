using System;
using BlackboardScripts;
using GOAP.Animation;
using GOAP.Animation.Enums;
using Helpers.Constants;
using R3;

namespace GOAP
{
    public class AttackStrategy : IActionStrategy
    {
        public bool CanPerform => true;
        public bool Complete { get; private set; }

        private CompositeDisposable _disposable = new();
        private readonly Subject<Unit> _timerCompletedSubject = new();
        private readonly AnimationBrain _animationBrain;

        public AttackStrategy(BlackboardController blackboardController)
        {
            _animationBrain = blackboardController.GetValue<AnimationBrain>(NameAIKeys.AnimationBrain);
        }

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
            _animationBrain.PlayAnimation(EMovementAnimationType.Attack, 0.2f);
        }
    }
}