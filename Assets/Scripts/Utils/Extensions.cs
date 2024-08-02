using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public static class Extensions
    {
        public static void ActionWithThrottle(this Button button, Action action, int throttleMillis = 200)
        {
            button.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(throttleMillis), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ =>
            {
                action?.Invoke();
            }).AddTo(button);
        }
        
        public static IObservable<Unit> OnClickWithThrottle(this Button button, int throttleMillis = 200)
        {
            return button.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(throttleMillis), Scheduler.MainThreadIgnoreTimeScale);
        }

        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            Color graphicColor = graphic.color;
            graphic.color = new Color(graphicColor.r, graphicColor.g, graphicColor.b, alpha);
        }
        
        public static string ToTimerText(this int i)
        {
            return Utils.GenerateTimerText(i);
        }
    }
}