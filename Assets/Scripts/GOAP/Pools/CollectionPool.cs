using System;

namespace GOAP.Pools
{
    public class CollectionPool<T> : APool<T> where T : class, new()
    {
        public CollectionPool(Action<T> onGet = null, Action<T> onRelease = null, int prewarmCount = 0) 
            : base(onGet, onRelease, prewarmCount) { }
    }
}