namespace Screens
{
    public class NavigationCommand
    {
        public ScreenName NextScreenName => _nextScreenName.Value;
        public ScreenName? ScreenToClose => _screenToClose;
        public object ExtraData => _extraData;
        public bool IsCloseAllScreens => _isCloseAllScreens;

        private object _extraData;
        private bool _isCloseAllScreens;
        private ScreenName? _nextScreenName;
        private ScreenName? _screenToClose;
        private bool _canReturnToPreviousScreen;
        private bool _closeAfterNextScreenShown;

        public NavigationCommand CloseAllScreens()
        {
            _isCloseAllScreens = true;
            return this;
        }

        public NavigationCommand ShowNextScreen(ScreenName screenName)
        {
            _nextScreenName = screenName;
            return this;
        }

        public NavigationCommand WithExtraData(object data)
        {
            _extraData = data;
            return this;
        }

        public NavigationCommand CloseScreen(ScreenName screenName)
        {
            _screenToClose = screenName;
            return this;
        }

        public bool IsNextScreenInQueue => !string.IsNullOrEmpty(_nextScreenName.ToString());
    }
}