using System;
using Balances.CollectableItems;
using UniRx;

namespace Data.DataProxy
{
    public class CollectableItemsDataProxy 
    {
        private readonly CollectableItemBalancesSO _collectableItemBalancesSo;
        private readonly Subject<CollectableItemType> _itemCollected = new();
        public IObservable<CollectableItemType> ItemCollected => _itemCollected;
        public CollectableItemsDataProxy(CollectableItemBalancesSO collectableItemBalancesSo)
        {
            _collectableItemBalancesSo = collectableItemBalancesSo;
        }

        public void SendCollectedItem(CollectableItemType type) => _itemCollected.OnNext(type);
        public int GetItemPoints(CollectableItemType type) => _collectableItemBalancesSo.GetPoints(type);
    }
}