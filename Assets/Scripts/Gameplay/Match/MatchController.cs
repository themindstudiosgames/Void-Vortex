using System;
using Camera;
using Data.DataProxy;
using Infrastructure.ClientAPI;
using Infrastructure.GameStateMachine;
using Screens;
using Screens.MatchFinishPopup;
using UniRx;
using Zenject;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace Gameplay.Match
{
    public class MatchController : IInitializable
    {
        private readonly CollectableItemsDataProxy _collectableItemsDataProxy;
        private readonly MatchDataProxy _matchDataProxy;
        private readonly ScreenNavigationSystem _screenNavigationSystem;
        private readonly GameStateMachine _gameStateMachine;
        private readonly GameCamera _gameCamera;
        private readonly IApiInterface _apiInterface;
        
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private IDisposable _matchTimer;

        public MatchController(CollectableItemsDataProxy collectableItemsDataProxy, MatchDataProxy matchDataProxy,
            ScreenNavigationSystem screenNavigationSystem, GameStateMachine gameStateMachine,
            GameCamera gameCamera, IApiInterface apiInterface)
        {
            _collectableItemsDataProxy = collectableItemsDataProxy;
            _matchDataProxy = matchDataProxy;
            _screenNavigationSystem = screenNavigationSystem;
            _gameStateMachine = gameStateMachine;
            _gameCamera = gameCamera;
            _apiInterface = apiInterface;
        }
        
        public void Initialize()
        {
            _matchDataProxy.SetMatchState(MatchStateType.Waiting);
            _collectableItemsDataProxy.ItemCollected.Subscribe(type =>
            {
                if(_matchDataProxy.MatchState.Value != MatchStateType.InProgress) return;
                int score = _collectableItemsDataProxy.GetItemPoints(type);
                _matchDataProxy.AddScore(score);
            }).AddTo(_compositeDisposable);
            
            _matchDataProxy.InitializeTimersForCurrentMatch();
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                _gameCamera.ShowScenePreview(3, StartCountdown);
            });
        }

        private void StartCountdown()
        {
            _matchDataProxy.SetMatchState(MatchStateType.CountDown);
            _matchTimer = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                _matchDataProxy.ElapseMatchCountDownTimer(1);
                if (_matchDataProxy.MatchCountdownTime.Value == 0)
                {
                    StartMatch();
                }
            });
        }

        private void StartMatch()
        {
            _matchTimer?.Dispose();
            _matchDataProxy.SetMatchState(MatchStateType.InProgress);
            _matchTimer = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                _matchDataProxy.ElapseMatchLeftTimer(1);
                if (_matchDataProxy.MatchLeftTime.Value == 0)
                {
                    FinishMatch();
                }
            });
        }

        private void FinishMatch()
        {
            _matchDataProxy.FinishMatch();

            _matchTimer?.Dispose();
            _apiInterface.SubmitScore(_matchDataProxy.MatchLevel, _matchDataProxy.Score.Value, () =>
            {
                _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand()
                    .ShowNextScreen(ScreenName.MatchFinishPopup)
                    .WithExtraData(new MatchFinishPopupDependencies(_gameStateMachine, _matchDataProxy)));
            });
        }
    }
}