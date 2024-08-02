using System.Collections.Generic;
using Gameplay.CollectableItems;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Gameplay.PlayerHole
{
    public class CollectableItemAbsorber : MonoBehaviour
    {
        [SerializeField] private Transform horizontalTarget;
        [SerializeField] private Transform verticalTarget;
        [SerializeField] private Collider horizontalAbsorbTrigger;
        [SerializeField] private Collider verticalAbsorbTrigger;
        [SerializeField] private GameObject absorptionLocker;

        private readonly List<CollectableItemPresenter> _horizontalCollectableItems = new();
        private readonly List<CollectableItemPresenter> _verticalCollectableItems = new();
        private readonly List<CollectableItemPresenter> _lockedItems = new();
        
        private void Awake()
        {
            horizontalAbsorbTrigger.OnTriggerEnterAsObservable().Subscribe(obj =>
            {
                if(obj.isTrigger || !obj.TryGetComponent(out CollectableItemPresenter item)) return;
                _horizontalCollectableItems.Add(item);
            }).AddTo(this);
            
            horizontalAbsorbTrigger.OnTriggerExitAsObservable().Subscribe(obj =>
            {
                if(obj.isTrigger || !obj.TryGetComponent(out CollectableItemPresenter item)) return;
                _horizontalCollectableItems.Remove(item);
            }).AddTo(this);
            
            verticalAbsorbTrigger.OnTriggerEnterAsObservable().Subscribe(obj =>
            {
                if(obj.isTrigger || !obj.TryGetComponent(out CollectableItemPresenter item)) return;
                _verticalCollectableItems.Add(item);
            }).AddTo(this);
            
            verticalAbsorbTrigger.OnTriggerExitAsObservable().Subscribe(obj =>
            {
                if(obj.isTrigger || !obj.TryGetComponent(out CollectableItemPresenter item)) return;
                _verticalCollectableItems.Remove(item);
            }).AddTo(this);
            
            absorptionLocker.OnTriggerEnterAsObservable().Subscribe(obj =>
            {
                if(obj.isTrigger || !obj.TryGetComponent(out CollectableItemPresenter item)) return;
                _lockedItems.Add(item);
            }).AddTo(this);
            
            absorptionLocker.OnTriggerExitAsObservable().Subscribe(obj =>
            {
                if(obj.isTrigger || !obj.TryGetComponent(out CollectableItemPresenter item)) return;
                _lockedItems.Remove(item);
            }).AddTo(this);
        }

        public void AbsorbItems(float horizontalForce, float verticalForce)
        {
            foreach (CollectableItemPresenter item in _horizontalCollectableItems)
            {
                if(_lockedItems.Contains(item)) continue;
                Vector3 targetPosition = horizontalTarget.position;
                float distance = Vector3.Distance(targetPosition, item.Position);
                item.AddForce((targetPosition - item.Position).normalized * (1 / distance * horizontalForce));
            }
                
            foreach (CollectableItemPresenter item in _verticalCollectableItems)
            {
                if(_lockedItems.Contains(item)) continue;
                Vector3 targetPosition = verticalTarget.position;
                targetPosition.y = item.Position.y;
                Vector3 direction = (targetPosition - item.Position).normalized;
                Vector3 right = Vector3.Cross(direction, Vector3.up);
                direction += Vector3.down + right;
                item.AddForce(direction * verticalForce);
            }
        }
    }
}