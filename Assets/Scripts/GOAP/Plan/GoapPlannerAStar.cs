using System.Collections.Generic;
using System.Linq;
using BehaviourTree;
using Customs;
using GOAP.Pools;
using Unity.VisualScripting;
using UnityEngine;
using ZLinq;

namespace GOAP
{
    public class GoapPlannerAStar : IGoapPlanner
    {
        private readonly Dictionary<AgentBelief, HashSet<AgentAction>> _effectToActionsCache = new();
        private readonly HashSet<AgentBelief> _requiredEffects = new();
        private readonly Dictionary<AgentBelief, bool> _conditionsCache = new();
        private readonly HashSet<AgentBelief> _worldBeliefs = new();
        private readonly Dictionary<HashSet<AgentBelief>, float> _visited = new(HashSetComparer<AgentBelief>.Instance);
        private readonly Stack<TempLeaf> _actionStack = new();
        private readonly PriorityQueue<TempLeaf, float> _openSet = new();
        
        private readonly CollectionPool<HashSet<AgentBelief>> _poolHashSet;
        private readonly InstantiablePool<TempLeaf> _tempLeafPool;
        
        public GoapPlannerAStar(CollectionPool<HashSet<AgentBelief>> poolHashSet, InstantiablePool<TempLeaf> tempLeafPool)
        {
            _tempLeafPool = tempLeafPool;
            _poolHashSet = poolHashSet;
        }

        public (AgentPlan plan, AgentGoal goal) GetPlan(
            HashSet<AgentAction> availableActions,
            HashSet<AgentGoal> goals,
            AgentGoal mostRecentGoal = default
        )
        {
            BuildEffectToActionsMap(availableActions);
            GetAllWorldBeliefs(availableActions, goals);
            BuildConditionsCache(_worldBeliefs);
            
            var orderedGoals = goals
                .AsValueEnumerable()
                .Where(
                    goal => goal.DesiredEffects
                    .AsValueEnumerable()
                    .Any(b => !_conditionsCache[b]))
                .OrderByDescending(goal => goal == mostRecentGoal ? goal.Priority - 0.01f : goal.Priority);
            
            foreach (var goal in orderedGoals)
            {
                _requiredEffects.Clear();
                _requiredEffects
                    .AddRange(goal.DesiredEffects
                        .AsValueEnumerable()
                        .Where(b => !_conditionsCache[b])
                        .AsEnumerable());

                var startNode = _tempLeafPool.Get();
                startNode.InitializeLeaf(default, _requiredEffects, 0f, "Start");

                if (FindPathAStar(startNode))
                {
                    BuildActionStack(startNode);
                    
                    _tempLeafPool.Release(startNode);
                    
                    return (new AgentPlan(startNode.Cost, _actionStack), goal);
                }

                BuildAllLeafs(startNode);
                
                var countLeafs = _actionStack.Count;
                    
                for (var i = 0; i < countLeafs; i++)
                {
                    var leafNative = _actionStack.Pop();
                        
                    _tempLeafPool.Release(leafNative);
                    leafNative.RequiredEffects.Clear();
                    _poolHashSet.Release(leafNative.RequiredEffects);
                }
                
                _tempLeafPool.Release(startNode);
            }

            Debug.LogWarning("No plan found for any goal");
            return (null, default); 
        }

        private bool FindPathAStar(TempLeaf startNode)
        {
            _openSet.Clear();
            _openSet.Enqueue(startNode, Heuristic(startNode.RequiredEffects));

            _visited.Clear();

            while (_openSet.Count > 0)
            {
                var currentNode = _openSet.Dequeue();
                var currentEffect = currentNode.RequiredEffects;
                
                currentEffect.RemoveWhere(belief => belief.CheckCondition());
                
                if (currentEffect.Count == 0)
                {
                    return true; 
                }

                if (_visited.TryGetValue(currentEffect, out var existingCost) && currentNode.Cost >= existingCost)
                {
                    continue;
                }

                _visited[currentEffect] = currentNode.Cost;

                foreach (var action in GetRelevantActions(currentEffect).AsValueEnumerable().OrderBy(a => a.Cost))
                {
                    var newEffects = _poolHashSet.Get();
                    newEffects.AddRange(currentEffect);
                    
                    newEffects.ExceptWith(action.Effects);
                    newEffects.UnionWith(action.Precondition);

                    var newCost = currentNode.Cost + action.Cost;
                    
                    var newNode = _tempLeafPool.Get();
                    newNode.InitializeLeaf(action, newEffects, newCost, action.Name);
                    
                    currentNode.AddChild(newNode);

                    var priority = newCost + Heuristic(newEffects);
                    _openSet.Enqueue(newNode, priority);
                }
            }

            return false;
        }

        private void GetAllWorldBeliefs(HashSet<AgentAction> actions, HashSet<AgentGoal> goals)
        {
            _worldBeliefs.Clear();
            
            foreach (var action in actions)
            {
                foreach (var effect in action.Effects)
                {
                    _worldBeliefs.Add(effect);
                }
                foreach (var precondition in action.Precondition)
                {
                    _worldBeliefs.Add(precondition);
                }
            }
            
            foreach (var goal in goals)
            {
                foreach (var effect in goal.DesiredEffects)
                {
                    _worldBeliefs.Add(effect);
                }
            }
        }

        private void BuildEffectToActionsMap(HashSet<AgentAction> actions)
        {
            _effectToActionsCache.Clear();
            
            foreach (var action in actions)
            {
                foreach (var effect in action.Effects)
                {
                    if (!_effectToActionsCache.ContainsKey(effect)) 
                        _effectToActionsCache.Add(effect, new HashSet<AgentAction>());
                    
                    _effectToActionsCache[effect].Add(action);
                }
            }
        }

        private void BuildConditionsCache(HashSet<AgentBelief> worldBeliefs)
        {
            _conditionsCache.Clear();
            
            foreach (var belief in worldBeliefs)
            {
                _conditionsCache[belief] = belief.CheckCondition();
            }
        }

        private IEnumerable<AgentAction> GetRelevantActions(HashSet<AgentBelief> effects)
        {
            return effects
                .AsValueEnumerable()
                .SelectMany(b =>
                    _effectToActionsCache.TryGetValue(b, out var actions) 
                        ? actions 
                        : Enumerable.Empty<AgentAction>())
                .AsEnumerable();
        }

        private float Heuristic(HashSet<AgentBelief> effects)
        {
            return effects
                .AsEnumerable()
                .Sum(b =>
                _effectToActionsCache.TryGetValue(b, out var actions)
                    ? actions.Min(a => a.Cost)
                    : float.MaxValue
            );
        }

        private void BuildActionStack(TempLeaf goalNode)
        {
            _actionStack.Clear();
            
            while (goalNode.Children.Count > 0)
            {
                var cheapestLeaf = goalNode.Children
                    .AsValueEnumerable()
                    .OrderBy(leaf => leaf.Cost)
                    .First();

                goalNode = cheapestLeaf;
                _actionStack.Push(cheapestLeaf);
            }
        }
        
        private void BuildAllLeafs(TempLeaf goalNode)
        {
            _actionStack.Clear();
            CollectAllDescendants(goalNode);
        }

        private void CollectAllDescendants(TempLeaf node)
        {
            foreach (var child in node.Children)
            {
                _actionStack.Push(child);
                CollectAllDescendants(child);
            }
        }
    }
}