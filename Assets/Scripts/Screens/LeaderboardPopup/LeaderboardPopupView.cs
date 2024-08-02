using System;
using System.Collections.Generic;
using Screens.SelectLevelPopup;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.LeaderboardPopup
{
    public class LeaderboardPopupView : ScreenView
    {
        [field: SerializeField] public List<LevelItemView> LevelItems { get; private set; }
        [field: SerializeField] public LeaderboardItemView UserItem { get; private set; }
        [field: SerializeField] public RectTransform LeaderboardItemViewsParent { get; private set; }
        
        [SerializeField] private Button closeButton;
        [SerializeField] private CanvasGroup popupCanvasGroup;
        
        public event Action CloseButtonClicked;
        
        private void Awake()
        {
            CanvasGroup = popupCanvasGroup;
            closeButton.ActionWithThrottle(() => { CloseButtonClicked?.Invoke(); });
        }
    }
}