using System;
using System.Collections.Generic;
using BlackboardScripts;
using Helpers.Constants;
using Helpers.Factory;
using UnityEngine;

namespace GOAP
{
    public class AgentBehaviorInitializer
    {
        private readonly HashSet<AgentAction> _actions;
        private readonly Dictionary<string, AgentBelief> _agentBeliefs;
        private readonly HashSet<AgentGoal> _goals;
        private readonly BlackboardController _blackboard;
        private readonly StrategyFactory _strategyFactory = new();
        
        public AgentBehaviorInitializer(HashSet<AgentAction> actions, 
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
            factory.AddGoalAgent("Rest", 2, _agentBeliefs[AgentBeliefsName.AgentIsRested]);
            factory.AddGoalAgent("Health", 3, _agentBeliefs[AgentBeliefsName.AgentIsHealthy]); 
            factory.AddGoalAgent("Attack", 4, _agentBeliefs[AgentBeliefsName.AttackingPlayer]); 
        }

        public void SetupActions()
        {
            var foodCort = GetDataOnBlackboard<Transform>(NameAIKeys.FoodLocation).position;
            var restZone = GetDataOnBlackboard<Transform>(NameAIKeys.ChillLocate).position;
            
            _actions.Add(new ActionBuilder("Chill")
                .WithActionStrategy(_strategyFactory.CreateIdleStrategy(6f, _blackboard))
                .WithEffect(_agentBeliefs[AgentBeliefsName.Nothing])
                .WithCost(goalPriority: 1, time: 3f, energy: 1f)
                .BuildAgentAction());
            
            _actions.Add(new ActionBuilder("Walk")
                .WithActionStrategy(_strategyFactory.CreatePatrolStrategy(_blackboard, 20f))
                .WithEffect(_agentBeliefs[AgentBeliefsName.AgentMoving])
                .WithCost(goalPriority: 1, time: 10f, energy: 3f)
                .BuildAgentAction());
            
            _actions.Add(new ActionBuilder("MoveToEat")
                .WithActionStrategy(_strategyFactory.CreateMoveToPointStrategy(_blackboard, () => foodCort))
                .WithEffect(_agentBeliefs[AgentBeliefsName.AgentAtFoodPosition])
                .WithCost(goalPriority: 3, time: 10f, energy: 5f)
                .BuildAgentAction());

           _actions.Add(new ActionBuilder("Heal")
                .WithActionStrategy(_strategyFactory.CreateIdleStrategy(5f, _blackboard))
                .WithPrecondition(_agentBeliefs[AgentBeliefsName.AgentAtFoodPosition])
                .WithEffect(_agentBeliefs[AgentBeliefsName.AgentIsHealthy])
                .WithCost(goalPriority: 3, time: 3f, energy: 3f, resources: 3f)
                .BuildAgentAction());
           
           _actions.Add(new ActionBuilder("MoveToRest")
               .WithActionStrategy(_strategyFactory.CreateMoveToPointStrategy(_blackboard, () => restZone))
               .WithEffect(_agentBeliefs[AgentBeliefsName.AgentAtRestingPosition])
               .WithCost(goalPriority: 2, time: 10f, energy: 5f)
               .BuildAgentAction());
           
           _actions.Add(new ActionBuilder("Rest")
               .WithActionStrategy(_strategyFactory.CreateRestStrategy(_blackboard, 5f))
               .WithPrecondition(_agentBeliefs[AgentBeliefsName.AgentAtRestingPosition])
               .WithEffect(_agentBeliefs[AgentBeliefsName.AgentIsRested])
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
                .WithActionStrategy(_strategyFactory.CreateAttackStrategy(_blackboard))
                .WithPrecondition(_agentBeliefs[AgentBeliefsName.PlayerToAttackSensor])
                .WithEffect(_agentBeliefs[AgentBeliefsName.AttackingPlayer])
                .WithCost(goalPriority: 3, time: 3f, energy: 6f)
                .BuildAgentAction());
        }

        public void SetupBeliefs()
        {
            var factory = new BeliefFactory(_agentBeliefs);
            
            factory.AddBelief(AgentBeliefsName.Nothing, GetFuncDelegate(NameAgentPredicate.NothingPredicate));
            factory.AddBelief(AgentBeliefsName.AttackingPlayer, GetFuncDelegate(NameAgentPredicate.AttackPredicate));
                
            factory.AddBelief(AgentBeliefsName.AgentIdle, GetFuncDelegate(NameAgentPredicate.IdlePredicate));
            factory.AddBelief(AgentBeliefsName.AgentMoving, GetFuncDelegate(NameAgentPredicate.MovementPredicate));
            
            factory.AddBelief(AgentBeliefsName.AgentIsHealthLow, GetFuncDelegate(NameAgentPredicate.HealthLowPredicate));
            factory.AddBelief(AgentBeliefsName.AgentIsHealthy, GetFuncDelegate(NameAgentPredicate.HealthPredicate));
            
            factory.AddBelief(AgentBeliefsName.AgentIsStaminaLow, GetFuncDelegate(NameAgentPredicate.StaminaLowPredicate));
            factory.AddBelief(AgentBeliefsName.AgentIsRested, GetFuncDelegate(NameAgentPredicate.StaminaPredicate));
            
            var foodCort = GetDataOnBlackboard<Transform>(NameAIKeys.FoodLocation).position;
            var chillZone = GetDataOnBlackboard<Transform>(NameAIKeys.ChillLocate).position;
            
            factory.AddLocationBelief(AgentBeliefsName.AgentAtFoodPosition, foodCort, GetFuncDelegate(NameAgentPredicate.LocationFoodPredicate));
            factory.AddLocationBelief(AgentBeliefsName.AgentAtRestingPosition, chillZone, GetFuncDelegate(NameAgentPredicate.LocationChillPredicate));
            
            factory.AddSensorBelief(AgentBeliefsName.PlayerInEyeSensor, GetSensor(NameAgentPredicate.EyesSensorPredicate));
            
            factory.AddSensorBelief(AgentBeliefsName.PlayerInHitSensor, GetSensor(NameAgentPredicate.HitSensorPredicate));
            
            factory.AddSensorBelief(AgentBeliefsName.PlayerToAttackSensor, GetSensor(NameAgentPredicate.AttackSensorPredicate));
            
            factory.AddSensorBelief(AgentBeliefsName.EnemySearch, GetSensor(NameAgentPredicate.VisionSensorPredicate));
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