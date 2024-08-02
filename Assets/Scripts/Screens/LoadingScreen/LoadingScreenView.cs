using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.LoadingScreen
{
    public class LoadingScreenView : ScreenView
    {
        [SerializeField] private Slider loadingBar;
        public void SetLoadingProgress(float progress, bool withAnimation)
        {
            if (withAnimation)
            {
                loadingBar.DOValue(progress, 0.1f);
            }
            else
            {
                loadingBar.value = progress;
            }
        }
    }
}