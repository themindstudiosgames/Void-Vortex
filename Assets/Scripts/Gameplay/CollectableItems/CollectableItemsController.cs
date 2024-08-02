using System;
using System.Collections.Generic;
using Balances.CollectableItems;
using Data.DataProxy;
using Gameplay.Match;
using UniRx;
using Zenject;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace Gameplay.CollectableItems
{
    public class CollectableItemsController : IInitializable, IDisposable
    {
        private readonly CollectableItemsDataProxy _collectableItemsDataProxy;
        private readonly MatchDataProxy _matchDataProxy;
        private readonly List<CollectableItemPresenter> _collectableItems;

        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public CollectableItemsController(CollectableItemsDataProxy collectableItemsDataProxy, 
            List<CollectableItemPresenter> collectableItems, MatchDataProxy matchDataProxy)
        {
            _collectableItemsDataProxy = collectableItemsDataProxy;
            _collectableItems = collectableItems;
            _matchDataProxy = matchDataProxy;
        }

        public void Initialize()
        {
            foreach (CollectableItemPresenter item in _collectableItems)
            {
                item.Collected += OnItemCollected;
            }

            ObservableExtensions.Subscribe(_matchDataProxy.MatchState.Where(m => m == MatchStateType.Finish).Take(1), _ =>
            {
                CountNotCollectedItems();
            }).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            foreach (CollectableItemPresenter item in _collectableItems)
            {
                item.Collected -= OnItemCollected;
            }
            _compositeDisposable.Dispose();
        }

        private void OnItemCollected(CollectableItemPresenter item)
        {
            _collectableItemsDataProxy.SendCollectedItem(item.Type);
        }

        private void CountNotCollectedItems()
        {
            Dictionary<CollectableItemType, int> notCollectedItemsCount = new Dictionary<CollectableItemType, int>();
            
            foreach (var item in _collectableItems)
            {
                if(item.IsCollected) continue;
                
                if (notCollectedItemsCount.ContainsKey(item.Type))
                {
                    notCollectedItemsCount[item.Type]++;
                }
                else
                {
                    notCollectedItemsCount.Add(item.Type, 1);
                }
            }

            
        }
    }
}