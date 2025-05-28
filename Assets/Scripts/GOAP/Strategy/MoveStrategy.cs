using System;
using BlackboardScripts;
using GOAP.Animation;
using GOAP.Animation.Enums;
using Helpers.Constants;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP
{
    public class MoveStrategy : IActionStrategy
    {
        public bool CanPerform => !Complete;
        public bool Complete => _agent.remainingDistance <= 1f && !_agent.pathPending;
        
        private readonly NavMeshAgent _agent;
        private readonly Func<Vector3> _destination;
        private readonly bool _isUpdate;
        private readonly AnimationBrain _animationBrain;

        public MoveStrategy(BlackboardController blackboardController, Func<Vector3> destination)
        {
            _agent = blackboardController.GetValue<NavMeshAgent>(NameAIKeys.Agent);
            _animationBrain = blackboardController.GetValue<AnimationBrain>(NameAIKeys.AnimationBrain);
            _destination = destination;
        }

        public void Start()
        {
            _animationBrain.PlayAnimation(EMovementAnimationType.ForwardRun, 0.2f);
            _agent.SetDestination(_destination());
        }
        
        public void Stop() => _agent.ResetPath();
    }
}