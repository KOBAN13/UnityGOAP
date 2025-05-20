using GOAP.Animation.Enums;

namespace GOAP.Animation.Interface
{
    public interface IAnimationBrain
    {
        void PlayAnimation(EMovementAnimationType type, float fadeTime = 0.2f);
        void SetDefaultAnimation(EMovementAnimationType type, float fadeTime = 0.2f);
        void PlayForce(AnimationRequest request);
    }
}