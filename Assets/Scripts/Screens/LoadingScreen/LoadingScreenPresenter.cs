using System;
using SceneManagement;
using Zenject;
using UniRx;

namespace Screens.LoadingScreen
{
    public class LoadingScreenPresenter : ScreenPresenter
    {
        private LoadingScreenView _view;
        private SceneLoader _sceneLoader;
        private IDisposable _progressDisposable;
        [Inject]
        private void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private void Awake()
        {
            _view = GetComponent<LoadingScreenView>();
            OnShowCallback += sceneLoad =>
            {
                bool isLoadOfScene = (bool)sceneLoad;
                _view.SetLoadingProgress(0, false);
                if (isLoadOfScene)
                {
                    _progressDisposable = _sceneLoader.LoadingProgress.Subscribe(progress =>
                    {
                        _view.SetLoadingProgress(progress, true);
                    });
                }
            };
            OnHideCallback += () => { _progressDisposable?.Dispose(); };
        }
    }
}