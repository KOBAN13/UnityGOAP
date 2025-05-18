using BlackboardScripts;
using Helpers.Constants;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP
{
    public class MoveAttackStrategy : IActionStrategy
    {
        public bool CanPerform => !Complete;
        public bool Complete => _agent.remainingDistance <= 1f && _agent.pathPending == false;

        private readonly NavMeshAgent _agent;
        private readonly Transform _playerTransform;

        public MoveAttackStrategy(BlackboardController blackboardController)
        {
            _agent = blackboardController.GetValue<NavMeshAgent>(NameAIKeys.Agent);
            _playerTransform = blackboardController.GetValue<Transform>(NameAIKeys.PlayerTarget);
        }

        public void Start()
        {
            _agent.destination = _playerTransform.position;
        }

        public void Update(float timeDelta)
        {
            _agent.destination = _playerTransform.position;
        }

        public void Stop()
        {
            _agent.ResetPath();
        }
    }
}