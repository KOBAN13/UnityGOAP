using System;

namespace BlackboardScripts
{
    [Serializable]
    public class BlackboardEntry<T>
    {
        public BlackboardKey Key { get; private set; }
        public T Value { get; private set; }
        public Type ValueType { get; private set; }
        
        public BlackboardEntry(BlackboardKey key, T value)
        {
            Key = key;
            Value = value;
            ValueType = typeof(T);
        }

        public void SetProperty(BlackboardKey key, T value)
        {
            Key = key;
            Value = value;
            ValueType = typeof(T);
        }

        public override bool Equals(object obj) => obj is BlackboardEntry<T> other && other.Key == Key;

        public override int GetHashCode() => Key.GetHashCode();
    }
}