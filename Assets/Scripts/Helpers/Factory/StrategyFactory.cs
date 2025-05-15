using System;
using BlackboardScripts;
using GOAP;
using UnityEngine;

namespace Helpers.Factory
{
    public class StrategyFactory
    {
        public IActionStrategy CreateIdleStrategy(float duration, BlackboardController blackboardController) =>
            new IdleStrategy(duration, blackboardController);

        public IActionStrategy CreatePatrolStrategy(BlackboardController blackboardController, float duration)
            => new PatrolStrategy(blackboardController, duration);

        public IActionStrategy CreateMoveToPointStrategy(BlackboardController blackboardController, Func<Vector3> destination) 
            => new MoveStrategy(blackboardController, destination);
        
        public IActionStrategy CreateMoveAttack(BlackboardController blackboardController) 
            => new MoveAttackStrategy(blackboardController);

        public IActionStrategy CreateEnemySearch(BlackboardController blackboardController)
            => new EnemySearchStrategy(blackboardController);

        public IActionStrategy CreateAttackStrategy() => new AttackStrategy();
    }
}