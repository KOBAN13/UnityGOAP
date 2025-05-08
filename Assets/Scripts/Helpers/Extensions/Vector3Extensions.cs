using UnityEngine;

namespace Customs
{
    public static class Vector3Extensions
    {
        public static bool InRangeOf(this Vector3 pos, Vector3 position, float range) => Vector3.Distance(position, pos) < range;
    }
}