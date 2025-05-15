using System;
using System.Collections.Generic;
using BlackboardScripts;
using Helpers.Constants;
using Helpers.Factory;
using UnityEngine;

namespace GOAP
{
    public class Setuppers
    {
        private readonly HashSet<AgentAction> _actions;
        private readonly Dictionary<string, AgentBelief> _agentBeliefs;
        private readonly HashSet<AgentGoal> _goals;
        private readonly BlackboardController _blackboard;
        private readonly StrategyFactory _strategyFactory = new();
        
        public Setuppers(HashSet<AgentAction> actions, 
            Dictionary<string, AgentBelief> agentBeliefs,
            HashSet<AgentGoal> goals, BlackboardController blackboardController)
        {
            _actions = actions;
            _agentBeliefs = agentBeliefs;
            _goals = goals;
            _blackboard = blackboardController;
        }
        
        public void SetupGoals()
        {
            var factory = new GoalFactory(_goals);
            
            factory.AddGoalAgent("Idle", 1, _agentBeliefs[AgentBeliefsName.Nothing]);
            factory.AddGoalAgent("Walk", 1, _agentBeliefs[AgentBeliefsName.AgentMoving]);
            factory.AddGoalAgent("Health", 2, _agentBeliefs[AgentBeliefsName.AgentIsHealthy]);
            factory.AddGoalAgent("Attack", 3, _agentBeliefs[AgentBeliefsName.AttackingPlayer]); 
        }

        public void SetupActions()
        {
            _actions.Add(new ActionBuilder("Chill")
                .WithActionStrategy(_strategyFactory.CreateIdleStrategy(3f, _blackboard))
                .WithEffect(_agentBeliefs[AgentBeliefsName.Nothing])
                .WithCost(goalPriority: 1, time: 3f, energy: 1f)
                .BuildAgentAction());
            
            _actions.Add(new ActionBuilder("Walk")
                .WithActionStrategy(_strategyFactory.CreatePatrolStrategy(_blackboard, 20f))
                .WithEffect(_agentBeliefs[AgentBeliefsName.AgentMoving])
                .WithCost(goalPriority: 1, time: 10f, energy: 3f)
                .BuildAgentAction());
 
            var foodCort = GetDataOnBlackboard<Transform>(NameAIKeys.FoodPoint).position;
            
            _actions.Add(new ActionBuilder("MoveToEat")
                .WithActionStrategy(_strategyFactory.CreateMoveToPointStrategy(_blackboard, () => foodCort))
                .WithEffect(_agentBeliefs[AgentBeliefsName.AgentAtFoodPosition])
                .WithCost(goalPriority: 2, time: 10f, energy: 5f)
                .BuildAgentAction());

           _actions.Add(new ActionBuilder("Heal")
                .WithActionStrategy(_strategyFactory.CreateIdleStrategy(5f, _blackboard))
                .WithPrecondition(_agentBeliefs[AgentBeliefsName.AgentAtFoodPosition])
                .WithEffect(_agentBeliefs[AgentBeliefsName.AgentIsHealthy])
                .WithCost(goalPriority: 2, time: 3f, energy: 3f, resources: 3f)
                .BuildAgentAction());
            
           _actions.Add(new ActionBuilder("PlayerLook")
                .WithActionStrategy(_strategyFactory.CreateMoveAttack(_blackboard))
                .WithPrecondition(_agentBeliefs[AgentBeliefsName.PlayerInEyeSensor])
                .WithEffect(_agentBeliefs[AgentBeliefsName.EnemySearch])
                .WithCost(goalPriority: 3, time: 2f, energy: 3f)
                .BuildAgentAction());
            
           _actions.Add(new ActionBuilder("PlayerHit")
                .WithActionStrategy(_strategyFactory.CreateMoveAttack(_blackboard))
                .WithPrecondition(_agentBeliefs[AgentBeliefsName.PlayerInHitSensor])
                .WithEffect(_agentBeliefs[AgentBeliefsName.EnemySearch])
                .WithCost(goalPriority: 3, time: 2f, energy: 3f)
                .BuildAgentAction());
            
           _actions.Add(new ActionBuilder("PlayerEscaped")
                .WithActionStrategy(_strategyFactory.CreateEnemySearch(_blackboard))
                .WithPrecondition(_agentBeliefs[AgentBeliefsName.EnemySearch])
                .WithEffect(_agentBeliefs[AgentBeliefsName.PlayerToAttackSensor])
                .WithCost(goalPriority: 3, time: 3f, energy: 3f)
                .BuildAgentAction());

           _actions.Add(new ActionBuilder("PlayerAttack")
                .WithActionStrategy(_strategyFactory.CreateAttackStrategy())
                .WithPrecondition(_agentBeliefs[AgentBeliefsName.PlayerToAttackSensor])
                .WithEffect(_agentBeliefs[AgentBeliefsName.AttackingPlayer])
                .WithCost(goalPriority: 3, time: 3f, energy: 6f)
                .BuildAgentAction());
        }

        public void SetupBeliefs()
        {
            var factory = new BeliefFactory(_agentBeliefs);
            
            factory.AddBelief(AgentBeliefsName.Nothing, GetFuncDelegate(NameExperts.NothingPredicate));
            factory.AddBelief(AgentBeliefsName.AttackingPlayer, GetFuncDelegate(NameExperts.AttackPredicate));
                
            factory.AddBelief(AgentBeliefsName.AgentIdle, GetFuncDelegate(NameExperts.IdlePredicate));
            factory.AddBelief(AgentBeliefsName.AgentMoving, GetFuncDelegate(NameExperts.MovementPredicate));
            
            factory.AddBelief(AgentBeliefsName.AgentIsHealthLow, GetFuncDelegate(NameExperts.HealthLowPredicate));
            factory.AddBelief(AgentBeliefsName.AgentIsHealthy, GetFuncDelegate(NameExperts.HealthPredicate));
            
            var foodCort = GetDataOnBlackboard<Transform>(NameAIKeys.FoodPoint).position;
            var chillZone = GetDataOnBlackboard<Transform>(NameAIKeys.ChillPoint).position;
            
            factory.AddLocationBelief(AgentBeliefsName.AgentAtFoodPosition, foodCort, GetFuncDelegate(NameExperts.LocationFoodPredicate));
            factory.AddLocationBelief(AgentBeliefsName.AgentAtRestingPosition, chillZone, GetFuncDelegate(NameExperts.LocationChillPredicate));
            
            factory.AddSensorBelief(AgentBeliefsName.PlayerInEyeSensor, GetSensor(NameExperts.EyesSensor));
            
            factory.AddSensorBelief(AgentBeliefsName.PlayerInHitSensor, GetSensor(NameExperts.HitSensor));
            
            factory.AddSensorBelief(AgentBeliefsName.PlayerToAttackSensor, GetSensor(NameExperts.AttackSensor));
            
            factory.AddSensorBelief(AgentBeliefsName.EnemySearch, GetSensor(NameExperts.EnemyVisionSensor));
        }
        
        private T GetDataOnBlackboard<T>(string name) where T : class
        {
            return _blackboard.GetValue<T>(name);
        }
        
        private Func<bool> GetFuncDelegate(string keyName)
        {
            return _blackboard.GetValue<Func<bool>>(keyName);
        }
        
        private ISensor GetSensor(string keyName)
        {
            return _blackboard.GetValue<ISensor>(keyName);
        }
    }
}