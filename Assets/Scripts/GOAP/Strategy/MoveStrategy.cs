using System;
using BlackboardScripts;
using Helpers.Constants;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP
{
    public class MoveStrategy : IActionStrategy
    {
        public bool CanPerform => !Complete;
        public bool Complete => _agent.remainingDistance <= 2f && !_agent.pathPending;
        
        private readonly NavMeshAgent _agent;
        private readonly Func<Vector3> _destination;
        private readonly bool _isUpdate;

        public MoveStrategy(BlackboardController blackboardController, Func<Vector3> destination)
        {
            _agent = blackboardController.GetValue<NavMeshAgent>(NameAIKeys.Agent);
            _destination = destination;
        }

        public void Start() => _agent.SetDestination(_destination());
        public void Stop() => _agent.ResetPath();
    }
}