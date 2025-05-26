using System;

namespace GOAP.Animation.Enums
{
    [Serializable]
    public enum EMovementAnimationType : int
    {
        None = 0,
        ForwardRun = 1,
        TurnRight = 2,
        Attack = 3,
        Idle = 4,
        TurnLeft = 5,
        SitDown = 6,
        Sit = 7,
        StandUp = 8,
    }
}