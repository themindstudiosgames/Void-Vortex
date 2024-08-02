using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class ScreenAssets
    {
        [field: SerializeField] public List<ScreenAsset> ScreenPrefabs { private set; get; }
    }
}