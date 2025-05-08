using System;

namespace BlackboardScripts
{
    [Serializable]
    public readonly struct BlackboardKey : IEquatable<BlackboardKey>, IComparable<BlackboardKey>
    {
        private readonly string _name;
        private readonly int _hashedKey;

        public BlackboardKey(string name)
        {
            _name = name;
            _hashedKey = _name.ComputeHash();
        }

        public bool Equals(BlackboardKey other) => _hashedKey == other._hashedKey;
        
        public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
        
        public override int GetHashCode() => _hashedKey;

        public override string ToString() => $"Key is: {_name}";

        public static bool operator ==(BlackboardKey first, BlackboardKey second) =>
            first._hashedKey == second._hashedKey;
        public static bool operator !=(BlackboardKey first, BlackboardKey second) =>
            first._hashedKey != second._hashedKey;

        public int CompareTo(BlackboardKey other)
        {
            var hashCompare = _hashedKey.CompareTo(other._hashedKey);
            return hashCompare != 0 ? hashCompare : _name.CompareTo(other._name);
        }
    }
}