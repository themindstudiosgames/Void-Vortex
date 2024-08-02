using System;
using Data.DataProxy;
using UniRx;
using Zenject;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace Gameplay.Boosters
{
    public class BoosterByProgressionController : IInitializable, IDisposable
    {
        private readonly BoosterByProgressionDataProxy _boosterByProgressionDataProxy;
        private readonly MatchDataProxy _matchDataProxy;
        private readonly CollectableItemsDataProxy _collectableItemsDataProxy;
        private readonly PlayerHoleDataProxy _playerHoleDataProxy;

        private IDisposable _leftTimeInterval; 
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public BoosterByProgressionController(BoosterByProgressionDataProxy boosterByProgressionDataProxy,
            MatchDataProxy matchDataProxy, CollectableItemsDataProxy collectableItemsDataProxy,
            PlayerHoleDataProxy playerHoleDataProxy)
        {
            _boosterByProgressionDataProxy = boosterByProgressionDataProxy;
            _matchDataProxy = matchDataProxy;
            _collectableItemsDataProxy = collectableItemsDataProxy;
            _playerHoleDataProxy = playerHoleDataProxy;
        }

        public void Initialize()
        {
            _boosterByProgressionDataProxy.ConfigureCurrentMilestone(_matchDataProxy.Score.Value);
            _collectableItemsDataProxy.ItemCollected.Subscribe(item =>
            {
                _boosterByProgressionDataProxy.AddCollectedItem();
            }).AddTo(_compositeDisposable);
            
            _boosterByProgressionDataProxy.BoosterActivated.Where(b => b).Subscribe(_ =>
                {
                    _boosterByProgressionDataProxy.ConfigureCurrentMilestone(_matchDataProxy.Score.Value);
                }).AddTo(_compositeDisposable);
            
            _boosterByProgressionDataProxy.BoosterActive.Subscribe(active =>
            {
                if (active)
                {
                    _leftTimeInterval = ObservableExtensions.Subscribe(Observable.Interval(TimeSpan.FromSeconds(1)), _ =>
                    {
                        _boosterByProgressionDataProxy.ElapseTimer(1);
                    });
                }
                else
                {
                    _leftTimeInterval?.Dispose();
                    _leftTimeInterval = null;
                }
                
                _matchDataProxy.ScoreMultiplier = active ? _boosterByProgressionDataProxy.ScoreBoostCoefficient : 1;
                _playerHoleDataProxy.SetSpeedMultiplier(active ? _boosterByProgressionDataProxy.SpeedBoostCoefficient : 1);
                _playerHoleDataProxy.SetMagnetismForceMultiplier(active ? _boosterByProgressionDataProxy.MagnetismBoostCoefficient : 1);
            });
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}