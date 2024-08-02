using System;
using UnityEngine;

namespace Balances.CollectableItems
{
    [Serializable]
    public class CollectableItemBalance
    {
        [field: SerializeField] public CollectableItemType ItemType { private set; get; }
        [field: SerializeField] public int Points { private set; get; }
        [field: SerializeField] public float SizeGrowth { private set; get; }
    }
}