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
    public class EnemySearchStrategy : IActionStrategy
    {
        public bool CanPerform => !Complete;
        public bool Complete { get; private set; }
        
        private readonly float _searchRadius;
        private readonly float _timeToSearch;
        private readonly float _timeToStopSearch;
        private readonly int _countIteration;
        private readonly Transform _unit;
        private readonly Transform _playerTransform;
        private readonly NavMeshAgent _navMesh;
        private readonly Subject<Unit> _timerCompletedSubject = new();
        private CompositeDisposable _disposable = new();
        
        private bool _playerKnown;
        private Vector3 _lastKnownPlayerPosition;
        private readonly AnimationBrain _animationBrain;

        public EnemySearchStrategy(BlackboardController blackboardController)
        {
            _playerKnown = true;
            _searchRadius = blackboardController.GetValue<float>(NameAIKeys.SearchEnemyRadius);
            _timeToSearch = blackboardController.GetValue<float>(NameAIKeys.TimeToSearchEnemy);
            _unit = blackboardController.GetValue<Transform>(NameAIKeys.AgentTransform);
            _navMesh = blackboardController.GetValue<NavMeshAgent>(NameAIKeys.Agent);
            _countIteration = blackboardController.GetValue<int>(NameAIKeys.CountIterationSearchEnemy);
            _playerTransform = blackboardController.GetValue<Transform>(NameAIKeys.PlayerTarget);
            _animationBrain = blackboardController.GetValue<AnimationBrain>(NameAIKeys.AnimationBrain);
            _timeToStopSearch = _timeToSearch * _countIteration;
        }

        public void Start()
        {
            Complete = false;
            _disposable = new CompositeDisposable();
            
            Observable.Interval(TimeSpan.FromSeconds(_timeToSearch))
                .Take(_countIteration)
                .Subscribe(_ => EnemySearch())
                .AddTo(_disposable);
            
            Observable.Timer(TimeSpan.FromSeconds(_timeToStopSearch))
                .Subscribe(_ => _timerCompletedSubject.OnNext(Unit.Default))
                .AddTo(_disposable);
            
            _timerCompletedSubject.Subscribe(_ =>
                {
                    Debug.LogWarning("Stop Search");
                    Complete = true;
                } )
                .AddTo(_disposable);
        }

        private void EnemySearch()
        {
            if (_playerKnown)
            {
                var directionToLastKnownPosition = (_playerTransform.position - _unit.position).normalized;
                    
                var randomOffset = UnityEngine.Random.insideUnitSphere * 0.5f;
                var searchDirection = (directionToLastKnownPosition + randomOffset).normalized;
                    
                var targetPosition = _unit.position + searchDirection * _searchRadius;

                if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, _searchRadius, NavMesh.AllAreas))
                {
                    _animationBrain.PlayAnimation(EMovementAnimationType.ForwardRun, 0.2f);
                    _navMesh.destination = hit.position;
                }

                _playerKnown = false;
            }
            else
            {
                var randomDirection = UnityEngine.Random.insideUnitSphere * _searchRadius;
                randomDirection += _unit.position;

                if (!NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _searchRadius, NavMesh.AllAreas))
                    return;
                
                _animationBrain.PlayAnimation(EMovementAnimationType.ForwardRun, 0.2f);
                _navMesh.destination = hit.position;
            }
        }

        public void Stop()
        {
            _playerKnown = true;
            ClearDisposable();
        }
        
        private void ClearDisposable()
        {
            _disposable?.Clear();
        }
    }
}