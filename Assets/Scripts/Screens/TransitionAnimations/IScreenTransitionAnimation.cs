using System;

namespace Screens.TransitionAnimations
{
    public interface IScreenTransitionAnimation
    {
        void PerformAnimation(Action onEndCallback = null);
        bool IsPlaying { get; }
        void KillAnim();
    }
}
