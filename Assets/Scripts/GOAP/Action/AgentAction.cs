using System.Collections.Generic;
using BehaviourTree;
using Customs;

namespace GOAP
{
    public class AgentAction
    {
        public string Name { get; private set; }
        public float Cost { get; private set; }

        private BTNodeStatus _status;

        public HashSet<AgentBelief> Precondition { get; }
        public HashSet<AgentBelief> Effects { get; }

        private IActionStrategy _actionStrategy;

        public AgentAction(string name)
        {
            Name = name;
            
            Precondition = new HashSet<AgentBelief>();
            Effects = new HashSet<AgentBelief>();
            _status = BTNodeStatus.Success;
            _actionStrategy = null;
            Cost = 0f;
        }

        public void SetCost(float cost)
        {
            Preconditions.CheckValidateData(cost);
            Cost = cost;
        }

        public void SetActionStrategy(IActionStrategy strategy)
        {
            Preconditions.CheckNotNull(strategy);
            _actionStrategy = strategy;
        }
        
        public bool Complete => _actionStrategy.Complete;
        public void Start() => _actionStrategy.Start();

        public BTNodeStatus Update(float deltaTime)
        { 
            if (_actionStrategy.CanPerform)
            { 
                _actionStrategy.Update(deltaTime);
                _status = BTNodeStatus.Running;
            }
            
            if(_actionStrategy.Complete == false) return _status = BTNodeStatus.Running;
            
            foreach (var effect in Effects)
            {
                effect.CheckCondition();
            }
            
            _status = BTNodeStatus.Success;
            return _status;
        }

        public void Stop() => _actionStrategy.Stop();

        public void CalculateCost(int goalPriority, float time, float resources = 1f, 
            float energy = 1f, float baseCost = 10f, float timeFactor = 1f, 
            float resourceFactor = 1f, float energyFactor = 1f)
        {
            Cost = (baseCost / goalPriority) + (time * timeFactor) + (resources * resourceFactor) + (energy * energyFactor);
        }
    }
}