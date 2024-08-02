using System;
using Data.Enums;
using JetBrains.Annotations;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace Screens.MainSceneScreen
{
    public class MainSceneScreenView : MonoBehaviour
    {
        [SerializeField] private Button singlePlayerButton;
        [SerializeField] private Button leaderboardButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button fullscreenButton;
        [SerializeField] private Button changeNicknameButton;
        [SerializeField, CanBeNull] private Button exitButton;

        [SerializeField] private GameObject nicknamePanel;
        [SerializeField] private TMP_Text nicknameText;
        [SerializeField] private GameObject hintForDesktop;
        
        public event Action SinglePlayerClicked;
        public event Action LeaderboardClicked;
        public event Action SettingsClicked;
        public event Action FullscreenClicked;
        public event Action ChangeNicknameClicked;
        public event Action ExitButtonClicked;

        private void Start()
        {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX||UNITY_EDITOR
            exitButton?.gameObject.SetActive(true);
            hintForDesktop.SetActive(true);
#else
            exitButton?.gameObject.SetActive(false);
            hintForDesktop.SetActive(false);
#endif
            singlePlayerButton.ActionWithThrottle(() => SinglePlayerClicked?.Invoke());
            leaderboardButton.ActionWithThrottle(() => LeaderboardClicked?.Invoke());
            settingsButton.ActionWithThrottle(() => { SettingsClicked?.Invoke(); });
            changeNicknameButton.ActionWithThrottle(() => { ChangeNicknameClicked?.Invoke(); });
            exitButton?.ActionWithThrottle(() => { ExitButtonClicked?.Invoke(); });
            fullscreenButton.OnPointerDownAsObservable()
                .Subscribe(_ => { FullscreenClicked?.Invoke(); })
                .AddTo(this);
        }

        public void SetFullscreenIcon(Sprite icon, Sprite pressedIcon)
        {
            fullscreenButton.image.sprite = icon;
            fullscreenButton.spriteState = new SpriteState() { pressedSprite = pressedIcon };
        }

        public void ChangeViewState(LoginStateType screenType)
        {
            nicknamePanel.SetActive(screenType is LoginStateType.Logged);
            singlePlayerButton.gameObject.SetActive(screenType is LoginStateType.Logged);
            leaderboardButton.gameObject.SetActive(screenType is LoginStateType.Logged);
        }

        public void SetNickname(string nickname)
        {
            nicknameText.text = nickname;
        }
    }
}