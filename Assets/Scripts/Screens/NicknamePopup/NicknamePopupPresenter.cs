using Data.DataProxy;
using Data.Enums;
using Infrastructure.ClientAPI;
using Screens.TransitionAnimations;
using Sounds;
using Zenject;

namespace Screens.NicknamePopup
{
    public class NicknamePopupPresenter : ScreenPresenter
    {
        private NicknamePopupView _view;

        private UserDataProxy _userDataProxy;
        private IApiInterface _apiInterface;

        [Inject]
        public void Construct(UserDataProxy userDataProxy, IApiInterface apiInterface)
        {
            _view = GetComponent<NicknamePopupView>();
            _userDataProxy = userDataProxy;
            _apiInterface = apiInterface;
        }

        private void Awake()
        {
            _view = GetComponent<NicknamePopupView>();
            OnShowTransitionAnimation = new ScaleTransition(_view.CanvasGroup, 0, 1, 0.2f, true);
            OnHideTransitionAnimation = new ScaleTransition(_view.CanvasGroup, 1, 0, 0.2f, true);

            OnShowCallback += o =>
            {
                SoundsManager.PlaySound(AudioKey.OpenPopup);
                if (!string.IsNullOrEmpty(_userDataProxy.Nickname.Value))
                {
                    _view.NicknameInputField.text = _userDataProxy.Nickname.Value;
                }
            };
            
            OnHideCallback += () => { SoundsManager.PlaySound(AudioKey.ClosePopup); };

            _view.SetIsValid(true);
            _view.NicknameInputField.onValueChanged.AddListener(nickname =>
            {
                bool isValid = nickname.Length is < 13 and > 2;
                _view.SetIsValid(isValid);
                if (!isValid)
                {
                    _view.SetNicknameErrorMessage("Nickname must be between 3 and 12 characters.");
                }
            });
            _view.SendClicked += () =>
            {
                SoundsManager.PlaySound(AudioKey.Button);
                _apiInterface.ChangeNickname(_view.NicknameInputField.text,
                    nickname =>
                    {
                        _userDataProxy.ChangeNickname(nickname);
                        _userDataProxy.ChangeLoginState(LoginStateType.Logged);
                        CloseScreen();
                    },
                    error =>
                    {
                        _view.SetIsValid(false);
                        _view.SetNicknameErrorMessage(error == "Name not available" ? "Nickname already exists" : error);
                    });
            };
            _view.CloseClicked += () =>
            {
                if (_userDataProxy.LoginState.Value == LoginStateType.NicknameNeeded) return;
                CloseScreen();
            };
        }
    }
}