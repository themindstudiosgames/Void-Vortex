using Infrastructure.ClientAPI;
using Screens;
using UniRx;
using UnityEngine;
using Zenject;

namespace Helpers
{
    public class InternetRequestsScreenHelper : MonoBehaviour
    {
        [SerializeField] private GameObject overlayScreen;

        private IApiInterface _apiInterface;
        private ScreenNavigationSystem _screenNavigationSystem;
        private bool _isTryAgainPopupOpened;

        [Inject]
        public void Construct(IApiInterface apiInterface, ScreenNavigationSystem screenNavigationSystem)
        {
            _apiInterface = apiInterface;
            _screenNavigationSystem = screenNavigationSystem;
        }

        public void Awake()
        {
            _apiInterface.RequestSendingFailed += OpenTryAgainPopup;
            _apiInterface.RequestSentSuccessfully += CloseTryAgainPopup;
            _apiInterface.IsRequestProcessing.Subscribe(processing =>
            {
                Debug.Log("___Request processing: " + processing);
                overlayScreen.SetActive(processing);
            }).AddTo(this);
        }

        public void OnDestroy()
        {
            _apiInterface.RequestSendingFailed -= OpenTryAgainPopup;
            _apiInterface.RequestSentSuccessfully -= CloseTryAgainPopup;
        }

        private void OpenTryAgainPopup()
        {
            overlayScreen.SetActive(false);
            if (!_isTryAgainPopupOpened)
            {
                _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand().ShowNextScreen(ScreenName.TryAgainPopup));
                _isTryAgainPopupOpened = true;
            }
        }

        private void CloseTryAgainPopup()
        {
            _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand().CloseScreen(ScreenName.TryAgainPopup));
            _isTryAgainPopupOpened = false;
        }
    }
}