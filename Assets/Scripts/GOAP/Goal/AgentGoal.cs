using System;
using System.Collections.Generic;
using BlackboardScripts;
using Customs;

namespace GOAP
{
    public struct AgentGoal : IEquatable<AgentGoal>, IComparable<AgentGoal>
    {
        public string Name { get; }
        public float Priority { get; private set; }
        public List<AgentBelief> DesiredEffects { get; }

        public AgentGoal(string name)
        {
            Name = name;
            DesiredEffects = new List<AgentBelief>();
            Priority = 0;
        }
        
        public void SetPriority(float priority)
        {
            Preconditions.CheckValidateData(priority);
            Priority = priority;
        }

        #region IEquatableAndIComparable
        
        public int CompareTo(AgentGoal other) => string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        
        public static bool operator >(AgentGoal a, AgentGoal b) => a.CompareTo(b) > 0;
        public static bool operator <(AgentGoal a, AgentGoal b) => a.CompareTo(b) < 0;
        public static bool operator >=(AgentGoal a, AgentGoal b) => a.CompareTo(b) >= 0;
        public static bool operator <=(AgentGoal a, AgentGoal b) => a.CompareTo(b) <= 0;
        
        public bool Equals(AgentGoal other) => Name == other.Name;

        public override bool Equals(object obj) => obj is AgentGoal other && Equals(other);

        public override int GetHashCode() => Name.ComputeHash();

        public static bool operator ==(AgentGoal a, AgentGoal b) => a.Equals(b);
        public static bool operator !=(AgentGoal a, AgentGoal b) => !a.Equals(b);
        
        #endregion
    }
}