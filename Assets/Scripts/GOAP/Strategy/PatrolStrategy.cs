using System;
using BlackboardScripts;
using GOAP.Animation;
using GOAP.Animation.Enums;
using Helpers.Constants;
using R3;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP
{
    public class PatrolStrategy : IActionStrategy
    {
        public bool CanPerform => !Complete;
        public bool Complete { get; private set; }
        
        private readonly NavMeshAgent _agent;
        private readonly Transform _entry;
        private readonly float _duration;
        private bool _isPathCalculated;
        private CompositeDisposable _disposable = new();
        private readonly NavGrid _navGrid;
        private readonly Subject<Unit> _timerCompletedSubject = new();
        private readonly AnimationBrain _animationBrain;

        public PatrolStrategy(BlackboardController blackboardController, float duration)
        {
            _agent = blackboardController.GetValue<NavMeshAgent>(NameAIKeys.Agent);
            _entry = blackboardController.GetValue<Transform>(NameAIKeys.AgentTransform);
            _navGrid = blackboardController.GetValue<NavGrid>(NameAIKeys.NavGrid);
            _animationBrain = blackboardController.GetValue<AnimationBrain>(NameAIKeys.AnimationBrain);
            _duration = duration;
        }

        public void Start()
        {
            Complete = false;
            _disposable = new CompositeDisposable();

            Observable.Timer(TimeSpan.FromSeconds(_duration))
                .Do(_ => Complete = true)
                .Subscribe(_ => _timerCompletedSubject.OnNext(Unit.Default))
                .AddTo(_disposable);
            
            _timerCompletedSubject.Subscribe(_ => Complete = true)
                .AddTo(_disposable);
        }

        public void Update(float deltaTime)
        {
            if (_isPathCalculated && _agent.remainingDistance < 1f)
            {
                ChangePosition();
                _isPathCalculated = false;
            }

            if (_agent.pathPending == false) _isPathCalculated = true;
        }

        public void Stop()
        {
            ClearDisposable();
        }

        private void ChangePosition()
        {
            var pointMap = _navGrid.GetPointMap();
            var point = pointMap.GetPointAtPosition(_entry.position + Vector3.up * 2);

            if (point == null) return;
            
            _agent.destination = pointMap.FindFreePointAtPointRandomDist(point, 3, 10).Position;
            _animationBrain.PlayAnimation(EMovementAnimationType.ForwardRun, 0.2f);
        }
        
        private void ClearDisposable()
        {
            _disposable?.Clear();
        }
    }
}