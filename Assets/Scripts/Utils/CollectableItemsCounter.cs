using System.Collections.Generic;
using Balances.CollectableItems;
using Data.DataProxy;
using Gameplay.CollectableItems;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Utils
{
    public class CollectableItemsCounter : MonoBehaviour
    {
        [Inject]
        private void Construct(List<CollectableItemPresenter> items, CollectableItemsDataProxy collectableItemsDataProxy)
        {
            Dictionary<CollectableItemType, int> itemsCount = new Dictionary<CollectableItemType, int>();
            
            foreach (var item in items)
            {
                if (itemsCount.ContainsKey(item.Type))
                {
                    itemsCount[item.Type]++;
                }
                else
                {
                    itemsCount.Add(item.Type, 1);
                }
            }
            Dictionary<int, int> scoreItemsCount = new Dictionary<int, int>();

            
            foreach (KeyValuePair<CollectableItemType,int> keyValuePair in itemsCount)
            {
                int score = collectableItemsDataProxy.GetItemPoints(keyValuePair.Key);
                if (scoreItemsCount.ContainsKey(score))
                {
                    scoreItemsCount[score] += keyValuePair.Value;
                }
                else
                {
                    scoreItemsCount.TryAdd(score, keyValuePair.Value);
                }
            }
            
            int totalScore = 0;

            foreach (var scoreItem in scoreItemsCount)
            {
                totalScore += scoreItem.Key * scoreItem.Value;
            }

            string log = $"Scene: {SceneManager.GetActiveScene().name}\n\n";

            foreach (var keyValue in itemsCount)
            {
                log += $"\tItems: {keyValue.Key}, Count: {keyValue.Value}\n";
            }

            log += "\n";

            foreach (var keyValue in scoreItemsCount)
            {
                log += $"\tItems by score: {keyValue.Key}, Count: {keyValue.Value}\n";
            }
            
            log += $"\nTotal items count: {items.Count}, Total score without boosters: {totalScore}";
            
            Debug.Log(log);
        }
    }
}