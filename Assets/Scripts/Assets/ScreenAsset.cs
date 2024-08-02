using System;
using Screens;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class ScreenAsset
    {
        [field: SerializeField] public ScreenName ScreenName { get; private set; }
        [field: SerializeField] public GameObject PortraitScreen { get; private set; }
        [field: SerializeField] public GameObject DesktopScreen { get; private set; }
    }
}