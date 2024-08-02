using Screens.TransitionAnimations;
using Sounds;

namespace Screens.SettingsPopup
{
    public class SettingsPopupPresenter : ScreenPresenter
    {
        private SettingsPopupView _view;

        private void Awake()
        {
            _view = GetComponent<SettingsPopupView>();
            OnShowTransitionAnimation = new ScaleTransition(_view.CanvasGroup, 0, 1, 0.2f,true);
            OnHideTransitionAnimation = new ScaleTransition(_view.CanvasGroup, 1, 0, 0.2f,true);
            
            _view.CloseButtonClicked += CloseScreen;
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
            OnShowCallback += o =>
            {
                _view.SetMusicButtonState(SoundsManager.IsMusicOn);
                _view.SetSoundButtonState(SoundsManager.IsSoundOn);
                SoundsManager.PlaySound(AudioKey.OpenPopup);
            };
            OnHideCallback += () =>
            {
                SoundsManager.PlaySound(AudioKey.ClosePopup);
            };
        }
    }
}