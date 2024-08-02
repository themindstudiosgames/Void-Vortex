using System;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using Zenject;

namespace Screens
{
    public class ScreensInstaller : MonoBehaviour
    {
        [SerializeField] private Canvas defaultCanvas;

        private readonly Dictionary<ScreenName, ScreenAsset> _localScreens = new();

        private Transform _dCanvasTransform;
        private bool _isPortrait;

        [Inject]
        public void Construct(ScreenAssets screenAssets)
        {
            _dCanvasTransform = defaultCanvas.transform;
            foreach (ScreenAsset screenAsset in screenAssets.ScreenPrefabs)
            {
                _localScreens.Add(screenAsset.ScreenName, screenAsset);
            }
            
            _isPortrait = Screen.height > Screen.width;
        }

        public void InstantiateScreen(ScreenName screenName, Action<ScreenPresenter> onSuccess)
        {
            if (_localScreens.TryGetValue(screenName, out var screenAsset))
            {
                GameObject newScreen = Instantiate(_isPortrait ? screenAsset.PortraitScreen : screenAsset.DesktopScreen,
                    _dCanvasTransform);
                Debug.Log($"Setup {screenName}");
                newScreen.name = screenName.ToString();
                if (newScreen.TryGetComponent(out ScreenPresenter presenter))
                {
                    onSuccess.Invoke(presenter);
                }
                else
                {
                    Debug.LogError("ScreenPresenter script not found on instantiated screen.");
                }
            }
            else
            {
                Debug.LogError($"Screen {screenName} not found in ScreenInstaller");
            }
        }
    }
}