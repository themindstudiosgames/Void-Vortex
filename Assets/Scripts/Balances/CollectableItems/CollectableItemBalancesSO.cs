using System.Collections.Generic;
using UnityEngine;

namespace Balances.CollectableItems
{
    [CreateAssetMenu(fileName = "CollectableItemBalancesSO", menuName = "SO/GameBalances/CollectableItemBalances")]
    public class CollectableItemBalancesSO : ScriptableObject
    {
        [field: SerializeField] public CollectableItemBalance[] CollectableItemBalance { private set; get; }

        private Dictionary<CollectableItemType, CollectableItemBalance> _pointsDictionary;
        private Dictionary<CollectableItemType, CollectableItemBalance> PointsDictionary =>
            _pointsDictionary ??= CreateBalancesDictionary();
        
        private Dictionary<CollectableItemType, CollectableItemBalance> CreateBalancesDictionary()
        {
            Dictionary<CollectableItemType, CollectableItemBalance> dictionary = 
                new Dictionary<CollectableItemType, CollectableItemBalance>(CollectableItemBalance.Length);
            foreach (var item  in CollectableItemBalance)
            {
                dictionary.Add(item.ItemType, item);
            }

            return dictionary;
        }
        
        public int GetPoints(CollectableItemType type) => PointsDictionary[type].Points;
        public float GetSizeGrowth(CollectableItemType type) => PointsDictionary[type].SizeGrowth;
    }
}