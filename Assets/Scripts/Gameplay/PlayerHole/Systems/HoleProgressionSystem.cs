using System.Collections;
using Data.DataProxy;
using UniRx;
using UnityEngine;

namespace Gameplay.PlayerHole.Systems
{
    public class HoleProgressionSystem : IPlayerHoleSystem
    {
        private readonly PlayerHoleDataProxy _playerHoleDataProxy;
        private readonly PlayerHolePresenter _playerHolePresenter;
        private readonly CollectableItemsDataProxy _collectableItemsDataProxy;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private bool _active;

        private Coroutine _changeSizeCoroutine;
        public HoleProgressionSystem(PlayerHolePresenter playerHolePresenter, CollectableItemsDataProxy collectableItemsDataProxy,
            PlayerHoleDataProxy playerHoleDataProxy)
        {
            _playerHolePresenter = playerHolePresenter;
            _collectableItemsDataProxy = collectableItemsDataProxy;
            _playerHoleDataProxy = playerHoleDataProxy;
        }

        public void Initialize()
        {
            _collectableItemsDataProxy.ItemCollected.Subscribe(type =>
            {
                if(!_active) return;
                _playerHoleDataProxy.IncreaseSizeAndSpeedByItem(type);
            }).AddTo(_compositeDisposable);
            
            _playerHolePresenter.SetSize(_playerHoleDataProxy.Size.Value);
            _playerHoleDataProxy.Size.Subscribe(ChangeSize).AddTo(_compositeDisposable);
        }

        public void SetActive(bool active) => _active = active;
        public void Dispose() => _compositeDisposable?.Dispose();

        private void ChangeSize(float duration = 1)
        {
            float currentSize = _playerHolePresenter.CurrentSize;
            float progress = 0;
            
            if (duration == 0)
            {
                _playerHolePresenter.SetSize(_playerHoleDataProxy.Size.Value);
                return;
            }

            if (_changeSizeCoroutine != null) return;
            _changeSizeCoroutine = _playerHolePresenter.StartCoroutine(ChangeSizeCoroutine());
            
            IEnumerator ChangeSizeCoroutine()
            {
                float progressScale = 1 / duration;
                while (progress <= 1)
                {
                    progress += Time.deltaTime * progressScale;
                    _playerHolePresenter.SetSize(Mathf.Lerp(currentSize, _playerHoleDataProxy.Size.Value, progress));
                    yield return null;
                }

                _changeSizeCoroutine = null;
            }
        }
    }
}