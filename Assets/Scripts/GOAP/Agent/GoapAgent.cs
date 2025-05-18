using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTree;
using BlackboardScripts;
using R3;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using Customs;
using GOAP.Pools;
using Helpers.Constants;

namespace GOAP
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
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

        [Header("HealthStats")] 
        [SerializeField] private float _health;
        [SerializeField] private float _stamina;
        
        [Header("Agent Links")]
        [SerializeField] private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        
        [Header("Goap Scripts")]
        private readonly Dictionary<string, AgentBelief> _agentBeliefs = new();
        private readonly HashSet<AgentAction> _actions = new();
        private readonly HashSet<AgentGoal> _goals = new();
        private readonly CompositeDisposable _disposable = new();
        private IGoapPlanner _goapPlanner;
        private AgentPlan _actionPlan;
        private Setuppers _setuppers;
        private BehaviourTree.BehaviourTree _behaviourTree;
        private BlackboardController _blackboardController;
        private AgentGoal _agentGoal;
        private IBTDebugger _debugger;
        
        [Header("Pools")]
        private InstantiablePool<TempLeaf> _tempLeafPool;
        private CollectionPool<HashSet<AgentBelief>> _poolHashSet;
        private FixedObjectPool<INode> _nodesBehaviourTree;
        
        [Inject]
        public void Construct(IBTDebugger debugger)
        {
            _debugger = debugger;
        }

        private void Awake()
        {
            _blackboardController = new BlackboardController();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            SetupDataToBlackboard();
            _behaviourTree = new BehaviourTree.BehaviourTree("Agent Tree", 0, _debugger);
            
            _setuppers = new Setuppers(_actions, _agentBeliefs, 
                _goals, _blackboardController);
            
            var listSelectors = new List<INode>();

            for (var i = 0; i < 10; i++)
            {
                listSelectors.Add(new Selector("Selector Leafs", 0, _debugger));
            }
            
            _poolHashSet = new CollectionPool<HashSet<AgentBelief>>(null, null, 10);
            _tempLeafPool = new InstantiablePool<TempLeaf>(null, null, 10);
            _nodesBehaviourTree = new FixedObjectPool<INode>(listSelectors, () => new Selector("Selector Leafs", 0, _debugger));
            
            _goapPlanner = new GoapPlannerAStar(_poolHashSet, _tempLeafPool);
            
            _setuppers.SetupBeliefs();
            _setuppers.SetupActions();
            _setuppers.SetupGoals();
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
        }

        private void SetupDataToBlackboard()
        {
            _blackboardController.SetValue<ISensor>(NameExperts.EyesSensor, _eyesSensor);
            _blackboardController.SetValue<ISensor>(NameExperts.HitSensor, _hitSensor);
            _blackboardController.SetValue<ISensor>(NameExperts.AttackSensor, _attackSensor);
            _blackboardController.SetValue<ISensor>(NameExperts.EnemyVisionSensor, _enemyVisionSensor);
            _blackboardController.SetValue(NameAIKeys.Animator, _animator);
            _blackboardController.SetValue(NameAIKeys.Agent, _navMeshAgent);
            _blackboardController.SetValue(NameAIKeys.HealthAI, _health);
            _blackboardController.SetValue(NameAIKeys.FoodPoint, _foodCort);
            _blackboardController.SetValue(NameAIKeys.ChillPoint, _chilZone);
            _blackboardController.SetValue(NameAIKeys.TransformAI, transform);
            _blackboardController.SetValue(NameAIKeys.PlayerTarget, _target);
            _blackboardController.SetValue(NameAIKeys.NavGrid, _navGrid);
            _blackboardController.SetValue(NameAIKeys.SearchEnemyRadius, 5f);
            _blackboardController.SetValue(NameAIKeys.TimeToSearchEnemy, 3f);
            _blackboardController.SetValue(NameAIKeys.CountIterationSearchEnemy, 3);
            
            _blackboardController.SetValue<Func<bool>>(NameExperts.NothingPredicate, () => false);
            _blackboardController.SetValue<Func<bool>>(NameExperts.AttackPredicate, () => false);
            _blackboardController.SetValue<Func<bool>>(NameExperts.IdlePredicate, () => !_navMeshAgent.hasPath);
            _blackboardController.SetValue<Func<bool>>(NameExperts.HealthLowPredicate, () => _health < 50);
            _blackboardController.SetValue<Func<bool>>(NameExperts.HealthPredicate, () => _health > 50);
            _blackboardController.SetValue<Func<bool>>(NameExperts.LocationFoodPredicate, () => _foodCort.position.InRangeOf(transform.position, 3f));
            _blackboardController.SetValue<Func<bool>>(NameExperts.LocationChillPredicate, () => _chilZone.position.InRangeOf(transform.position, 3f));
            _blackboardController.SetValue<Func<bool>>(NameExperts.MovementPredicate, () => _navMeshAgent.hasPath);
        }
        
        private void SetupTimers()
        {
            Observable
                .EveryUpdate()
                .Subscribe(_ => EntryPoint())
                .AddTo(_disposable); 
            
            Observable
                .Timer(TimeSpan.FromSeconds(2f), TimeSpan.FromSeconds(2f))
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
        }

        private void UpdateStats()
        {
            _stamina += _chilZone.position.InRangeOf(transform.position, 3f) ? 20 : -10;
            _health += _foodCort.position.InRangeOf(transform.position, 3f) ? 20 : -10;

            _stamina = Math.Clamp(_stamina, 0f, 150f);
            _health = Math.Clamp(_health, 0f, 150f);
        }

        private void InitBehaviourTree(Stack<TempLeaf> leafs)
        {
            var sequencePlan = _nodesBehaviourTree.Get();
            var countLeafs = leafs.Count;
            
            for (var i = 0; i < countLeafs; i++)
            {
                var leafNative = leafs.Pop();
                var leaf = new Leaf(leafNative.AgentAction,
                    leafNative.RequiredEffects.ToHashSet(), 
                    leafNative.Cost, leafNative.Name, 
                    _debugger);
                
                _tempLeafPool.Release(leafNative);
                leafNative.RequiredEffects.Clear();
                _poolHashSet.Release(leafNative.RequiredEffects);
                sequencePlan.AddChild(leaf);
            }
            
            _behaviourTree.AddChild(sequencePlan);
            _behaviourTree.Start();
        }
        
        private void FindMostPriorityGoal()
        {
            if (_actionPlan == null)
                return;
            
            _behaviourTree.Stop();
            _behaviourTree.Reset();
            
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
            _behaviourTree.Process();

            if (_behaviourTree.Status == BTNodeStatus.Running) return;
            
            _behaviourTree.Reset();
            _behaviourTree.SetGoalsState(default, _agentGoal);

            foreach (var node in _behaviourTree.Nodes)
            {
                _nodesBehaviourTree.Release(node);
            }
            
            _actionPlan = default;
        }

        private void CreatePlan()
        {
            var (planResult, goalResult) = _goapPlanner.GetPlan(_actions, _goals, _behaviourTree.LastGoal);
            
            if (planResult == null) return;
            
            _actionPlan = planResult;
            _agentGoal = goalResult;
            _behaviourTree.SetGoalsState(_agentGoal, default);
            InitBehaviourTree(_actionPlan.Actions);
        }
    }
}