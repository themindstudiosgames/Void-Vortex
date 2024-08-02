using System;
using UnityEngine;

namespace Screens.LeaderboardPopup
{
    [Serializable]
    public struct TopPositionConfig
    {
        [SerializeField] public Color backgroundColor;
        [SerializeField] public Color scoreBackgroundColor;
        [SerializeField] public Sprite cupSprite;
        [Header("Optional(only for local user panel)")]
        [SerializeField] public Color outlineColor;
    }
}