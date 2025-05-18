using System;

namespace GOAP.Pools
{
    public class InstantiablePool<T> : APool<T> where T : class, IPool, new()
    {
        public InstantiablePool(Action<T> onGet = null, Action<T> onRelease = null, int prewarmCount = 0) 
            : base(onGet, onRelease, prewarmCount) { }

        public override void Release(T item)
        {
            base.Release(item);
            item.Clear();
        }
    }
}