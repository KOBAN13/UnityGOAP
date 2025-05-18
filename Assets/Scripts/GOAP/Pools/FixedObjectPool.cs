using System;
using System.Collections.Generic;

namespace GOAP.Pools
{
    public class FixedObjectPool<T>
    {
        private readonly Stack<T> _pool = new();
        private readonly Func<T> _factory;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;

        public FixedObjectPool(List<T> prewarm, Func<T> factory, Action<T> onGet = null, Action<T> onRelease = null)
        {
            _factory = factory;
            _onGet = onGet;
            _onRelease = onRelease;

            InitializePool(prewarm);
        }

        public virtual T Get()
        {
            var item = _pool.Count > 0 ? _pool.Pop() : _factory();
            _onGet?.Invoke(item);
            return item;
        }
        
        public virtual void Release(T item)
        {
            _onRelease?.Invoke(item);
            _pool.Push(item);
        }
        
        public virtual void Clear()
        {
            _pool.Clear();
        }
        
        private void InitializePool(List<T> prewarm)
        {
            foreach (var obj in prewarm)
            {
                _pool.Push(obj);
            }
        }
    }
}