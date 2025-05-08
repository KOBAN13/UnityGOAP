using System;
using System.Threading;
using BlackboardScripts;
using Game.Player.PlayerStateMashine;
using R3;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP
{
    public class EnemySearchStrategy : IActionStrategy
    {
        public bool CanPerform => !Complete;
        public bool Complete { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; }
        
        private float _searchRadius;
        private float _timeToSearch;
        private float _timeToStopSearch;
        private int _countIteration;
        private CompositeDisposable _disposable = new();
        private Transform _unit;
        private Transform _playerTransform;
        private NavMeshAgent _navMesh;
        
        private readonly Subject<Unit> _timerCompletedSubject = new();
        private bool _playerKnown;
        private Vector3 _lastKnownPlayerPosition;

        public EnemySearchStrategy(BlackboardController blackboardController)
        {
            _playerKnown = true;
            _searchRadius = blackboardController.GetValue<float>(NameAIKeys.SearchEnemyRadius);
            _timeToSearch = blackboardController.GetValue<float>(NameAIKeys.TimeToSearchEnemy);
            _unit = blackboardController.GetValue<Transform>(NameAIKeys.TransformAI);
            _navMesh = blackboardController.GetValue<NavMeshAgent>(NameAIKeys.Agent);
            _countIteration = blackboardController.GetValue<int>(NameAIKeys.CountIterationSearchEnemy);
            _playerTransform = blackboardController.GetValue<Transform>(NameAIKeys.PlayerTarget);
            _timeToStopSearch = _timeToSearch * _countIteration;
        }

        public void Start()
        {
            ClearDisposable();
            _disposable = new CompositeDisposable();
            
            Complete = false;
            
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
                    _navMesh.destination = hit.position;
                }

                _playerKnown = false;
            }
            else
            {
                var randomDirection = UnityEngine.Random.insideUnitSphere * _searchRadius;
                randomDirection += _unit.position;

                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _searchRadius, NavMesh.AllAreas))
                {
                    _navMesh.destination = hit.position;
                }
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
            _disposable?.Dispose();
        }
    }
}