using Infrastructure.ClientAPI;
using Screens.TransitionAnimations;
using Sounds;
using Zenject;

namespace Screens.TryAgainPopup
{
    public class TryAgainPopupPresenter : ScreenPresenter
    {
        private TryAgainPopupView _view;
        private IApiInterface _apiInterface;

        [Inject]
        private void Construct(IApiInterface apiInterface)
        {
            _apiInterface = apiInterface;
        }

        private void Awake()
        {
            _view = GetComponent<TryAgainPopupView>();
            OnShowTransitionAnimation = new ScaleTransition(_view.CanvasGroup, 0, 1, 0.2f, true);
            OnHideTransitionAnimation = new ScaleTransition(_view.CanvasGroup, 1, 0, 0.2f, true);
            _view.TryAgainButtonClicked += () =>
            {
                _apiInterface.TryToSendNextRequest();
                SoundsManager.PlaySound(AudioKey.Button);
            };
            OnShowCallback += o => { SoundsManager.PlaySound(AudioKey.OpenPopup); };
            OnHideCallback += () => { SoundsManager.PlaySound(AudioKey.OpenPopup); };
        }
    }
}