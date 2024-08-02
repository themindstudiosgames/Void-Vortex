using System;
using UnityEngine;

namespace Screens.FromToAnimationItems.ViewInfos
{
    public struct ScoreFromToAnimationItemViewInfo
    {
        public Vector3 From;
        public Transform Target;
        public float Duration;
        public Action OnItemLanded;
        public int Score;
    }
}