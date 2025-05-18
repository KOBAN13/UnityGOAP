using System;
using Animancer;
using GOAP.Animation.Enums;
using UnityEngine;

namespace GOAP.Animation
{
    [Serializable]
    [EventNames(typeof(EAnimancerEvents))]
    public class MotionClipTransition : ClipTransition
    {
        [SerializeField] private bool applyRootMotion;
        
        public EAnimationLayer AnimationLayer;
        
        public override void Apply(AnimancerState state)
        {
            base.Apply(state);
            
            state.Root.Component.Animator.applyRootMotion = applyRootMotion;
            state.LayerIndex = (int)AnimationLayer;
        }
    }
}