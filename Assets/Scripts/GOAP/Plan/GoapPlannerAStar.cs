using System.Collections.Generic;
using System.Linq;
using BehaviourTree;
using UnityEngine;

namespace GOAP
{
    public class GoapPlannerAStar : IGoapPlanner
    {
        private readonly Dictionary<AgentBelief, HashSet<AgentAction>> _effectToActionsCache = new();
        private readonly Dictionary<AgentBelief, bool> _conditionsCache = new();
        private readonly HashSet<AgentBelief> _worldBeliefs = new();
        private readonly Dictionary<HashSet<AgentBelief>, float> _visited = new(HashSetComparer<AgentBelief>.Instance);
        private readonly Stack<TempLeaf> _actionStack = new();
        private readonly PriorityQueue<TempLeaf, float> _openSet = new();

        public (AgentPlan plan, AgentGoal goal) GetPlan(HashSet<AgentAction> availableActions, HashSet<AgentGoal> goals,
            AgentGoal mostRecentGoal = null)
        {
            BuildEffectToActionsMap(availableActions);
            GetAllWorldBeliefs(availableActions, goals);
            BuildConditionsCache(_worldBeliefs);
            
            var orderedGoals = goals
                .Where(goal => goal.DesiredEffects.Any(b => !_conditionsCache[b]))
                .OrderByDescending(goal => goal == mostRecentGoal ? goal.Priority - 0.01f : goal.Priority);
            
            foreach (var goal in orderedGoals)
            {
                var requiredEffects = new HashSet<AgentBelief>(
                    goal.DesiredEffects.Where(b => !_conditionsCache[b])
                );

                var startNode = new TempLeaf(null, requiredEffects, 0f, "Start");

                if (FindPathAStar(startNode))
                {
                    if(startNode.IsLeafDead) continue;
                    
                    BuildActionStack(startNode);
                    
                    return (new AgentPlan(startNode.Cost, _actionStack), goal);
                }
            }

            Debug.LogWarning("No plan found for any goal");
            return (null, null); 
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

                foreach (var action in GetRelevantActions(currentEffect).OrderBy(a => a.Cost))
                {
                    var newEffects = new HashSet<AgentBelief>(currentEffect);
                    newEffects.ExceptWith(action.Effects);
                    newEffects.UnionWith(action.Precondition);

                    var newCost = currentNode.Cost + action.Cost;

                    var newNode = new TempLeaf(action, newEffects, newCost, action.Name);
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
                .SelectMany(b =>
                    _effectToActionsCache.TryGetValue(b, out var actions) 
                        ? actions 
                        : Enumerable.Empty<AgentAction>());
        }

        private float Heuristic(HashSet<AgentBelief> effects)
        {
            return effects.Sum(b =>
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
                var cheapestLeaf = goalNode.Children.OrderBy(leaf => leaf.Cost).First();

                goalNode = cheapestLeaf;
                _actionStack.Push(cheapestLeaf);
            }
        }
    }
}