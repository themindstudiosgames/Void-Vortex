using System;
using Assets;
using Assets.Types;
using Data.DataProxy;
using Data.Enums;
using Sounds;
using UniRx;
using UnityEngine;
using Zenject;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace Screens.MainSceneScreen
{
    public class MainSceneScreenPresenter : IInitializable, IDisposable
    {
        private readonly MainSceneScreenView _view;
        private readonly ScreenNavigationSystem _screenNavigationSystem;
        private readonly SpriteAssetsSO _spriteAssetsSo;
        private readonly UserDataProxy _userDataProxy;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public MainSceneScreenPresenter(MainSceneScreenView view,
            ScreenNavigationSystem screenNavigationSystem, SpriteAssetsSO spriteAssetsSo, UserDataProxy userDataProxy)
        {
            _view = view;
            _screenNavigationSystem = screenNavigationSystem;
            _spriteAssetsSo = spriteAssetsSo;
            _userDataProxy = userDataProxy;
        }

        public void Initialize()
        {
            SoundsManager.PlayMusic(AudioKey.MainMenuMusic, 0.1f);

            _view.SinglePlayerClicked += () =>
            {
                SoundsManager.PlaySound(AudioKey.Button);
                _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand().ShowNextScreen(ScreenName.SelectLevelPopup));
            };
            
            _view.LeaderboardClicked += () =>
            {
                SoundsManager.PlaySound(AudioKey.Button);
                _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand().ShowNextScreen(ScreenName.LeaderboardPopup));
            };

            _view.SettingsClicked += () =>
            {
                _screenNavigationSystem.ExecuteNavigationCommand(
                    new NavigationCommand().ShowNextScreen(ScreenName.SettingsPopup));
            };
            _view.FullscreenClicked += () =>
            {
                SoundsManager.PlaySound(AudioKey.Button);
                Screen.fullScreen = !Screen.fullScreen;
            };
            _view.ChangeNicknameClicked += () =>
            {
                SoundsManager.PlaySound(AudioKey.Button);
                _screenNavigationSystem.ExecuteNavigationCommand(
                    new NavigationCommand().ShowNextScreen(ScreenName.NicknamePopup));
            };
            _view.ExitButtonClicked += () =>
            {
                SoundsManager.PlaySound(AudioKey.Button);
                Application.Quit();
            };
            ObservableExtensions.Subscribe(Observable.EveryUpdate(), _ => { SetFullscreenIcon(Screen.fullScreen); }).AddTo(_compositeDisposable);

            void SetFullscreenIcon(bool fullscreen)
            {
                _view.SetFullscreenIcon(fullscreen
                        ? _spriteAssetsSo.GetButtonSprite(ButtonType.CollapseButton)
                        : _spriteAssetsSo.GetButtonSprite(ButtonType.FullscreenButton),
                    fullscreen
                        ? _spriteAssetsSo.GetButtonSprite(ButtonType.CollapseButtonPressed)
                        : _spriteAssetsSo.GetButtonSprite(ButtonType.FullscreenButtonPressed));
            }

            _view.ChangeViewState(_userDataProxy.LoginState.Value);
            
            _userDataProxy.LoginState.Subscribe(loginState =>
            {
                if (loginState is LoginStateType.NicknameNeeded)
                {
                    _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand().ShowNextScreen(ScreenName.NicknamePopup));
                }
                _view.ChangeViewState(loginState);
            }).AddTo(_compositeDisposable);
            
            _userDataProxy.Nickname.Subscribe(nickname =>
            {
                    _view.SetNickname(nickname);
            }).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}