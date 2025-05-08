using System;

namespace BlackboardScripts
{
    public class BlackboardController
    {
        private readonly Blackboard _blackboard = new ();

        public Blackboard GetBlackboard() => _blackboard;
        
        public T GetValue<T>(string key)
        {
            _blackboard.TryGetValue<T>(_blackboard.GetOrRegisterKey(key), out var value);
            return value;
        }

        public void SetValue<T>(string key, T value)
        {
            _blackboard.SetValue(_blackboard.GetOrRegisterKey(key), value);
        }
    }
}