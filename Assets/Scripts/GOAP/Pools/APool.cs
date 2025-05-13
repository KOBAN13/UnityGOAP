using System;
using System.Collections.Generic;

namespace GOAP.Pools
{
    public abstract class APool<T> where T : class, new()
    {
        private readonly Stack<T> _pool = new();
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;

        protected APool(Action<T> onGet = null, Action<T> onRelease = null, int prewarmCount = 0)
        {
            _onGet = onGet;
            _onRelease = onRelease;
            
            InitializePool(prewarmCount);
        }
        
        public virtual T Get()
        {
            var item = _pool.Count > 0 ? _pool.Pop() : new T();
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

        private void InitializePool(int prewarmCount)
        {
            for (var i = 0; i < prewarmCount; i++)
            {
                _pool.Push(new T());
            }
        }
    }
}