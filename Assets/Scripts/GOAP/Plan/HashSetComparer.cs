using System.Collections.Generic;

namespace GOAP
{
    public struct HashSetComparer<T> : IEqualityComparer<HashSet<T>>
    {
        public static readonly HashSetComparer<T> Instance = new HashSetComparer<T>();
        
        public bool Equals(HashSet<T> x, HashSet<T> y) => x.SetEquals(y);

        public int GetHashCode(HashSet<T> obj)
        {
            unchecked
            {
                var hash = 17;
                foreach (var item in obj)
                {
                    hash = hash * 23 + (item?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }
    }
}