using GOAP.Animation.Interface;

namespace GOAP.Animation
{
    public class AnimationBrain
    {
        private readonly ICharacterAnimanсerParameters _characterAnimancerParameters;

        public AnimationBrain(
            ICharacterAnimanсerParameters characterAnimancerParameters
        )
        {
            _characterAnimancerParameters = characterAnimancerParameters;
        }
    }
}