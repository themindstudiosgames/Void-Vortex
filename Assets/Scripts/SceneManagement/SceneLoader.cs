using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace SceneManagement
{
    public class SceneLoader
    {
        private readonly ReactiveProperty<float> _loadingProgress = new ReactiveProperty<float>();
        public IReadOnlyReactiveProperty<float> LoadingProgress => _loadingProgress;
        public void Load(SceneNames name, Action onLoaded = null)
        {
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            LoadScene(name, onLoaded);
        }

        private void LoadScene(SceneNames nextScene, Action onLoaded = null)
        {
            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene.ToString());
            IDisposable loadingObserver = Observable.EveryUpdate().Subscribe(_ =>
            {
                if (_loadingProgress.Value < waitNextScene.progress)
                {
                    _loadingProgress.Value = waitNextScene.progress;
                }
            });
            waitNextScene.completed += delegate
            {
                _loadingProgress.Value = 1;
                loadingObserver?.Dispose();
                onLoaded?.Invoke();
            };
        }

        public void SetProgress(float progress)
        {
            _loadingProgress.Value = progress;
        }
    }
}