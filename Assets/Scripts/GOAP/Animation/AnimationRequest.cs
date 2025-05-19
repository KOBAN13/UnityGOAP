using GOAP.Animation.Enums;

namespace GOAP.Animation
{
    public struct AnimationRequest
    {
        public readonly MovementAnimationType Type;
        public readonly bool ApplyRootMotion;
        public readonly bool WaitToEnd;
        public readonly EAnimationLayer AnimationLayer;
        public readonly float Delta;
        public readonly float Speed;

        public AnimationRequest(MovementAnimationType type, bool applyRootMotion, EAnimationLayer animationLayer, float delta, bool waitToEnd, float speed = 1f)
        {
            Type = type;
            ApplyRootMotion = applyRootMotion;
            AnimationLayer = animationLayer;
            Delta = delta;
            WaitToEnd = waitToEnd;
            Speed = speed;
        }
    }
}