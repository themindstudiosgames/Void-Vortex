using System;
using Assets;
using Assets.Types;
using Data.DataProxy;
using Gameplay.Match;
using Gameplay.PlayerHole;
using Helpers;
using Sounds;
using UniRx;
using UnityEngine;
using Zenject;

namespace Screens.GameSceneHud
{
    public class GameSceneHudPresenter : IInitializable, IDisposable
    {
        private readonly GameSceneHudView _view;
        private readonly MatchDataProxy _matchDataProxy;
        private readonly SpriteAssetsSO _spriteAssetsSo;
        private readonly FromToAnimationHelper _fromToAnimationHelper;
        private readonly PlayerHolePresenter _playerHolePresenter;
        private readonly BoosterByProgressionDataProxy _boosterByProgressionDataProxy;

        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public GameSceneHudPresenter(GameSceneHudView view, MatchDataProxy matchDataProxy, 
            SpriteAssetsSO spriteAssetsSo, FromToAnimationHelper fromToAnimationHelper,
            PlayerHolePresenter playerHolePresenter, BoosterByProgressionDataProxy boosterByProgressionDataProxy)
        {
            _view = view;
            _matchDataProxy = matchDataProxy;
            _spriteAssetsSo = spriteAssetsSo;
            _fromToAnimationHelper = fromToAnimationHelper;
            _playerHolePresenter = playerHolePresenter;
            _boosterByProgressionDataProxy = boosterByProgressionDataProxy;
        }

        public void Initialize()
        {
            SoundsManager.PlayMusic(AudioKey.GameSceneMusic, 0.1f);
            _view.SetCountDownTimerActive(false);
            _view.TimerAndScorePanelView.Close();
            
            _matchDataProxy.MatchLeftTime.Subscribe(leftTime =>
            {
                _view.TimerAndScorePanelView.SetMatchLeftTime(leftTime);
            }).AddTo(_compositeDisposable);
            _matchDataProxy.Score.Subscribe(leftTime =>
            {
                _view.TimerAndScorePanelView.SetPointsText(leftTime);
            }).AddTo(_compositeDisposable);
            _matchDataProxy.MatchCountdownTime.Subscribe(leftTime =>
            {
                _view.SetCountDownTimer(leftTime);
            }).AddTo(_compositeDisposable);
            _matchDataProxy.MatchState.Subscribe(type =>
            {
                switch (type)
                {
                    case MatchStateType.CountDown:
                        _view.SetCountDownTimerActive(true);
                        break;
                    case MatchStateType.InProgress:
                        _view.HideCountdownTimer(1f);
                        _view.TimerAndScorePanelView.Open(1);
                        break;
                    case MatchStateType.Finish:
                        _view.TimerAndScorePanelView.Hide(0.5f);
                        break;
                }
            }).AddTo(_compositeDisposable);
            
            _view.FullscreenClicked += () =>
            {
                SoundsManager.PlaySound(AudioKey.Button);
                Screen.fullScreen = !Screen.fullScreen;
            };
            
            _matchDataProxy.ScoresAdded.Subscribe(score =>
            {
                _fromToAnimationHelper
                    .SpawnScoreItem(_playerHolePresenter.ScoreFromToAnimationPoint.position, score);
            }).AddTo(_compositeDisposable);
            
            Observable.EveryUpdate().Subscribe(_ =>
            {
                SetFullscreenIcon(Screen.fullScreen);
            }).AddTo(_compositeDisposable);

            ConfigureBoosterPanel();
            ConfigureMultiplayerPanel();
            
            void SetFullscreenIcon(bool fullscreen)
            {
                _view.SetFullscreenIcon(fullscreen
                        ? _spriteAssetsSo.GetButtonSprite(ButtonType.CollapseButton)
                        : _spriteAssetsSo.GetButtonSprite(ButtonType.FullscreenButton), 
                    fullscreen
                        ? _spriteAssetsSo.GetButtonSprite(ButtonType.CollapseButtonPressed)
                        : _spriteAssetsSo.GetButtonSprite(ButtonType.FullscreenButtonPressed));
            }
        }
        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        private void ConfigureMultiplayerPanel()
        {
            _view.SetMusicButtonState(SoundsManager.IsMusicOn);
            _view.SetSoundButtonState(SoundsManager.IsSoundOn);
            _view.SoundButtonClicked += () =>
            {
                bool active = !SoundsManager.IsSoundOn;
                SoundsManager.ToggleSound(active);
                _view.SetSoundButtonState(active);
                if(active)
                {
                    SoundsManager.PlaySound(AudioKey.Button);
                }
            };
            _view.MusicButtonClicked += () =>
            {
                bool active = !SoundsManager.IsMusicOn;
                SoundsManager.ToggleMusic(active);
                _view.SetMusicButtonState(active);
                SoundsManager.PlaySound(AudioKey.Button);
            };
        }

        private void ConfigureBoosterPanel()
        {
            _boosterByProgressionDataProxy.CollectedItemsForBoost.CombineLatest(
                _boosterByProgressionDataProxy.NeedItemsForBoost,
                (collected, need) => (collected, need)).Subscribe(info =>
            {
                _view.BoosterProgressPanelView.SetProgress((float)info.collected / info.need);
            }).AddTo(_compositeDisposable);
            _boosterByProgressionDataProxy.BoosterActive.Subscribe(active =>
                {
                    _view.BoosterProgressPanelView.SetBoosterActiveStatus(active);
                    if(active) SoundsManager.PlaySound(AudioKey.BoosterActivated);
                }).AddTo(_compositeDisposable);
            _boosterByProgressionDataProxy.LeftTime.Subscribe(leftTime =>
                {
                    _view.BoosterProgressPanelView.SetLeftTimeText(Mathf.RoundToInt(leftTime));
                }).AddTo(_compositeDisposable);
        }
    }
}