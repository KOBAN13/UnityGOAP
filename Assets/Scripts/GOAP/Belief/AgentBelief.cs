using System;
using UnityEngine;

namespace GOAP
{
    public struct AgentBelief
    {
        public string Name { get; private set;  }
        private Func<bool> Condition { get; set; }
        private Vector3 ObservedLocation { get; set; }
        
        public AgentBelief(string name)
        {
            Name = name;
            ObservedLocation = Vector3.zero;
            Condition = null;
        }

        public bool CheckCondition() => Condition.Invoke();
        public Vector3 GetLocation() => ObservedLocation;
        public void SetCondition(Func<bool> condition) => Condition = condition;
        public void SetObservedLocation(Vector3 vector) => ObservedLocation = vector;
    }
}