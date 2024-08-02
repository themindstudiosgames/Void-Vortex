using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Utils
{
    public static class Utils
    {
        public static string GenerateTimerText(int timeInSeconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(timeInSeconds);
            if (timeInSeconds is < 60 and >= 0)
            {
                return $"0:{ToTime(timeInSeconds)}";
            }

            if (t.Hours > 0 && t.TotalHours < 24)
            {
                return $"{ToTime(t.Hours)}:{ToTime(t.Minutes)}";
            }

            if (t.TotalHours >= 24)
            {
                return $"{t.Days}:{ToTime(t.Hours)}:{ToTime(t.Minutes)}";
            }

            return $"{t.Minutes}:{ToTime(t.Seconds)}";

            string ToTime(int time)
            {
                return time > 9 ? time.ToString() : $"0{time}";
            }
        }
    }
}