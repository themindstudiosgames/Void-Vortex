using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.SettingsPopup
{
    public class SettingsPopupView : ScreenView
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button musicButton;
        [SerializeField] private CanvasGroup popupCanvasGroup;
        [SerializeField] private Color colorForButtonDisabling;

        public event Action CloseButtonClicked;
        public event Action SoundButtonClicked;
        public event Action MusicButtonClicked;

        private void Awake()
        {
            CanvasGroup = popupCanvasGroup;
            closeButton.ActionWithThrottle(() => { CloseButtonClicked?.Invoke(); });
            soundButton.ActionWithThrottle(() => { SoundButtonClicked?.Invoke(); });
            musicButton.ActionWithThrottle(() => { MusicButtonClicked?.Invoke(); });
        }

        public void SetSoundButtonState(bool active) => SetColorButtonByState(soundButton, active);
        public void SetMusicButtonState(bool active) => SetColorButtonByState(musicButton, active);

        private void SetColorButtonByState(Button button, bool active) =>
            button.image.color = active ? Color.white : colorForButtonDisabling;
        
    }
}