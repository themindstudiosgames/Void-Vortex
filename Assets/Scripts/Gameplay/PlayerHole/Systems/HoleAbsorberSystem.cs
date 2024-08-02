using Data.DataProxy;
using Gameplay.CollectableItems;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Gameplay.PlayerHole.Systems
{
    public class HoleAbsorberSystem : IPlayerHoleSystem
    {
        private readonly PlayerHoleDataProxy _playerHoleDataProxy;
        private readonly CollectableItemAbsorber _collectableItemAbsorber;
        private readonly Collider _collectorTrigger;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable(); 
        private bool _active;

        public HoleAbsorberSystem(CollectableItemAbsorber collectableItemAbsorber, Collider collectorTrigger,
            PlayerHoleDataProxy playerHoleDataProxy)
        {
            _collectableItemAbsorber = collectableItemAbsorber;
            _playerHoleDataProxy = playerHoleDataProxy;
            _collectorTrigger = collectorTrigger;
        }

        public void Initialize()
        {
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                if(!_active) return;
                _collectableItemAbsorber.AbsorbItems(_playerHoleDataProxy.MagnetismHorizontalForce.Value,
                        _playerHoleDataProxy.MagnetismVerticalForce.Value);
            }).AddTo(_compositeDisposable);
            _collectorTrigger.OnTriggerEnterAsObservable().Subscribe(obj =>
            {
                if (!_active || !obj.TryGetComponent(out CollectableItemPresenter item)) return;
                if(item.IsCollected) return;
                item.Collect();
            }).AddTo(_compositeDisposable);
            _collectorTrigger.OnTriggerExitAsObservable().Subscribe(obj =>
            {
                if (!_active || !obj.TryGetComponent(out CollectableItemPresenter item)) return;
                item.gameObject.SetActive(false);
            }).AddTo(_compositeDisposable);
        }

        public void SetActive(bool active) => _active = active;

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}