using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.TryAgainPopup
{
    public class TryAgainPopupView : ScreenView
    {
        [SerializeField] private Button tryAgainButton;
        [SerializeField] private CanvasGroup popupCanvasGroup;
        public event Action TryAgainButtonClicked;

        private void Awake()
        {
            CanvasGroup = popupCanvasGroup;
            tryAgainButton.ActionWithThrottle(() => TryAgainButtonClicked?.Invoke());
        }
    }
}