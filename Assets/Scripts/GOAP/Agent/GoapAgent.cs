using System;
using System.Collections.Generic;
using System.Linq;
using Animancer;
using BehaviourTree;
using BlackboardScripts;
using R3;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using Customs;
using GOAP.Animation;
using GOAP.Pools;
using GOAP.Stats;
using Helpers.Constants;
using Stats.Interface;

namespace GOAP
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AnimancerComponent))]
    public class GoapAgent : MonoBehaviour
    {
        [Header("Sensors")] 
        [SerializeField] private EyesSensor _eyesSensor;
        [SerializeField] private HitSensor _hitSensor;
        [SerializeField] private AttackSensor _attackSensor;
        [SerializeField] private EnemyVisionSensor _enemyVisionSensor;

        [Header("NavMesh")] 
        [SerializeField] private NavGrid _navGrid;

        [Header("Locations")] 
        [SerializeField] private Transform _foodCort;
        [SerializeField] private Transform _chilZone;
        [SerializeField] private Transform _target;
        
        [Header("Agent Links")]
        private NavMeshAgent _navMeshAgent;
        
        [Header("Goap Scripts")]
        private readonly Dictionary<string, AgentBelief> _agentBeliefs = new();
        private readonly HashSet<AgentAction> _actions = new();
        private readonly HashSet<AgentGoal> _goals = new();
        private readonly CompositeDisposable _disposable = new();
        private IGoapPlanner _goapPlanner;
        private AgentPlan _actionPlan;
        private AgentBehaviorInitializer _agentBehaviorInitializer;
        private BehaviourTreeNode _behaviourTreeNode;
        private BlackboardController _blackboardController;
        private AgentGoal _agentGoal;
        private AnimationBrain _animationBrain;
        private AgentStats _agentStats;
        private IBTDebugger _debugger;
        private AgentPools _agentsPool;
        
        [Header("Debugs")]
        [SerializeField] public float Health;
        [SerializeField] public float Stamina;
        
        private Action OnHit => () => Debug.LogWarning("OnHit");
        private Action OnDie => () => Debug.LogWarning("OnDie");
        
        [Inject]
        public void Construct(IBTDebugger debugger, AnimationBrain animationBrain, IHealthConfig healthConfig, IStaminaConfig staminaConfig)
        {
            _debugger = debugger;
            _animationBrain = animationBrain;
            
            _agentStats = new AgentStats(healthConfig, staminaConfig, OnHit, OnDie);
        }

        private void Awake()
        {
            _blackboardController = new BlackboardController();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            SetupDataToBlackboard();
            _behaviourTreeNode = new BehaviourTreeNode("Agent Tree", 0, _debugger);
            
            _agentBehaviorInitializer = new AgentBehaviorInitializer(_actions, _agentBeliefs, 
                _goals, _blackboardController);
            
            _agentsPool = new AgentPools(_debugger);
            
            _goapPlanner = new GoapPlannerAStar(_agentsPool.PoolHashSet, _agentsPool.TempLeafPool);
            
            _agentBehaviorInitializer.SetupBeliefs();
            _agentBehaviorInitializer.SetupActions();
            _agentBehaviorInitializer.SetupGoals();
        }

        private void Update()
        {
            Health = _agentStats.CurrentHealth;
            Stamina = _agentStats.CurrentStamina;
        }

        private void OnEnable()
        {
            SetupTimers();
            SetupSensors();
        }

        private void OnDisable()
        {
            _disposable.Clear();
            _disposable.Dispose();
            _agentStats.Dispose();
        }

        private void SetupDataToBlackboard()
        {
            _blackboardController.SetValue<ISensor>(NameAgentPredicate.EyesSensorPredicate, _eyesSensor);
            _blackboardController.SetValue<ISensor>(NameAgentPredicate.HitSensorPredicate, _hitSensor);
            _blackboardController.SetValue<ISensor>(NameAgentPredicate.AttackSensorPredicate, _attackSensor);
            _blackboardController.SetValue<ISensor>(NameAgentPredicate.VisionSensorPredicate, _enemyVisionSensor);
            _blackboardController.SetValue(NameAIKeys.Agent, _navMeshAgent);
            _blackboardController.SetValue(NameAIKeys.AgentStats, _agentStats);
            _blackboardController.SetValue(NameAIKeys.FoodLocation, _foodCort);
            _blackboardController.SetValue(NameAIKeys.ChillLocate, _chilZone);
            _blackboardController.SetValue(NameAIKeys.AgentTransform, transform);
            _blackboardController.SetValue(NameAIKeys.PlayerTarget, _target);
            _blackboardController.SetValue(NameAIKeys.NavGrid, _navGrid);
            _blackboardController.SetValue(NameAIKeys.SearchEnemyRadius, 5f);
            _blackboardController.SetValue(NameAIKeys.TimeToSearchEnemy, 3f);
            _blackboardController.SetValue(NameAIKeys.CountIterationSearchEnemy, 3);
            _blackboardController.SetValue(NameAIKeys.AnimationBrain, _animationBrain);
            
            _blackboardController.SetValue<Func<bool>>(NameAgentPredicate.NothingPredicate, () => false);
            _blackboardController.SetValue<Func<bool>>(NameAgentPredicate.AttackPredicate, () => false);
            _blackboardController.SetValue<Func<bool>>(NameAgentPredicate.IdlePredicate, () => !_navMeshAgent.hasPath);
            _blackboardController.SetValue<Func<bool>>(NameAgentPredicate.HealthLowPredicate, () => _agentStats.IsHealthLow(50));
            _blackboardController.SetValue<Func<bool>>(NameAgentPredicate.HealthPredicate, () => !_agentStats.IsHealthLow(50));
            _blackboardController.SetValue<Func<bool>>(NameAgentPredicate.StaminaLowPredicate, () => _agentStats.IsStaminaLow(50));
            _blackboardController.SetValue<Func<bool>>(NameAgentPredicate.StaminaPredicate, () => !_agentStats.IsStaminaLow(50));
            _blackboardController.SetValue<Func<bool>>(NameAgentPredicate.LocationFoodPredicate, () => _foodCort.position.InRangeOf(transform.position, 3f));
            _blackboardController.SetValue<Func<bool>>(NameAgentPredicate.LocationChillPredicate, () => _chilZone.position.InRangeOf(transform.position, 3f));
            _blackboardController.SetValue<Func<bool>>(NameAgentPredicate.MovementPredicate, () => _navMeshAgent.hasPath);
        }
        
        private void SetupTimers()
        {
            Observable
                .EveryUpdate()
                .Subscribe(_ => EntryPoint())
                .AddTo(_disposable); 
            
            Observable
                .Timer(TimeSpan.FromSeconds(3f), TimeSpan.FromSeconds(3f))
                .Subscribe(_ => UpdateStats())
                .AddTo(_disposable);
        }
        
        private void SetupSensors()
        {
            var eyesSensorStream = _eyesSensor.IsActiveSensor.Select(isDetected => (isDetected, "EyesSensor"));
            var hitSensorStream = _hitSensor.IsActiveSensor.Select(isHit => (isHit, "HitSensor"));
            var attackSensorStream = _attackSensor.IsActiveSensor.Select(isAttacking => (isAttacking, "AttackSensor"));
            
            var mergeStream = Observable.Merge(eyesSensorStream, hitSensorStream, attackSensorStream);
            
            mergeStream.Subscribe(sensorData =>
            {
                Debug.LogWarning($"Sensor: {sensorData.Item2} ; IsActive: {sensorData.Item1}");
    
                if (sensorData.Item1)
                    FindMostPriorityGoal();

            }).AddTo(_disposable);
            
            _agentStats.SubscribeStateChange(() =>
            {
                if(_agentStats.IsHealthLow(50) || _agentStats.IsStaminaLow(50))
                    FindMostPriorityGoal();
            });
        }

        private void UpdateStats()
        {
            if (_foodCort.position.InRangeOf(transform.position, 3f))
            {
                _agentStats.AddHealth(15);
            }
            else
            {
                _agentStats.SetDamage(5);
            }
            
            if (_chilZone.position.InRangeOf(transform.position, 3f))
            {
                _agentStats.AddStamina(15);
            }
            else
            {
                _agentStats.SetFatigue(5);
            }
        }

        private void InitBehaviourTree(Stack<TempLeaf> leafs)
        {
            var sequencePlan = _agentsPool.NodesBehaviourTree.Get();
            var tempLeafPool = _agentsPool.TempLeafPool;
            var hashSetPool = _agentsPool.PoolHashSet;
            
            var countLeafs = leafs.Count;
            
            for (var i = 0; i < countLeafs; i++)
            {
                var leafNative = leafs.Pop();
                var leaf = new Leaf(leafNative.AgentAction,
                    leafNative.RequiredEffects.ToHashSet(), 
                    leafNative.Cost, leafNative.Name, 
                    _debugger);
                
                tempLeafPool.Release(leafNative);
                leafNative.RequiredEffects.Clear();
                hashSetPool.Release(leafNative.RequiredEffects);
                sequencePlan.AddChild(leaf);
            }
            
            _behaviourTreeNode.AddChild(sequencePlan);
            _behaviourTreeNode.Start();
        }
        
        private void FindMostPriorityGoal()
        {
            if (_actionPlan == null)
                return;
            
            _behaviourTreeNode.Stop();
            _behaviourTreeNode.Reset();
            
            CreatePlan();
        }

        private void EntryPoint()
        {
            if (_actionPlan == null)
            {
                CreatePlan();
            }
            
            CompletePlan();
        }
        
        private void CompletePlan()
        {
            _behaviourTreeNode.Process();

            if (_behaviourTreeNode.Status == BTNodeStatus.Running)
                return;
            
            _behaviourTreeNode.Reset();
            _behaviourTreeNode.SetGoalsState(default, _agentGoal);
            
            _agentGoal.SetIsCompleted(true);

            foreach (var node in _behaviourTreeNode.Nodes)
            {
                _agentsPool.NodesBehaviourTree.Release(node);
            }
            
            _actionPlan = null;
        }

        private void CreatePlan()
        {
            var mostPriorityGoal = _behaviourTreeNode.LastGoal == default 
                ? _behaviourTreeNode.CurrentGoal 
                : _behaviourTreeNode.LastGoal;
            
            var (planResult, goalResult) = _goapPlanner.GetPlan(_actions, _goals, mostPriorityGoal);
            
            if (planResult == null) return;
            
            Debug.LogWarning($"Create Plan: {goalResult.Name}");
            
            _actionPlan = planResult;
            _agentGoal = goalResult;
            _behaviourTreeNode.SetGoalsState(_agentGoal, default);
            InitBehaviourTree(_actionPlan.Actions);
        }
    }
}