using GOAP.Animation.Enums;

namespace GOAP.Animation
{
    public struct AnimationRequest
    {
        public readonly EMovementAnimationType Type;
        public readonly bool ApplyRootMotion;
        public readonly EAnimationLayer AnimationLayer;
        public readonly float FadeDuration;
        public readonly float Delta;
        public readonly float Speed;

        public AnimationRequest(EMovementAnimationType type, bool applyRootMotion, EAnimationLayer animationLayer, float delta, float fadeDuration = 0.2f, float speed = 1f)
        {
            Type = type;
            ApplyRootMotion = applyRootMotion;
            AnimationLayer = animationLayer;
            Delta = delta;
            FadeDuration = fadeDuration;
            Speed = speed;
        }
    }
}