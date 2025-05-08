using UnityEngine;
using UnityEngine.Jobs;

namespace BlackboardScripts
{
    public static class TransformExtension
    {
        public static TransformAccess ToTransformAccess(this Transform transform) => new TransformAccess
        {
            position = transform.position,
            rotation = transform.rotation,
            localRotation = transform.localRotation,
            localPosition = transform.localPosition
        };
    }
}