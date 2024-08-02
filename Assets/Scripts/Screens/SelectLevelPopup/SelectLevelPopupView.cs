using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.SelectLevelPopup
{
    public class SelectLevelPopupView : ScreenView
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button nextPageButton;
        [SerializeField] private Button prevPageButton;
        [SerializeField] private CanvasGroup popupCanvasGroup;

        [field: SerializeField] public PageListView PageListView { get; private set; }
        [field: SerializeField] public List<LevelItemView> LevelItems { get; private set; }

        public event Action CloseButtonClicked;
        public event Action NextPageButtonClicked;
        public event Action PrevPageButtonClicked;

        public void SetButtonsInteractable(int pageIndex, int pagesCount)
        {
            nextPageButton.interactable = pageIndex < pagesCount - 1;
            prevPageButton.interactable = pageIndex > 0;
        }

        private void Awake()
        {
            CanvasGroup = popupCanvasGroup;
            closeButton.ActionWithThrottle(() => { CloseButtonClicked?.Invoke(); });
            nextPageButton.ActionWithThrottle(() => { NextPageButtonClicked?.Invoke(); });
            prevPageButton.ActionWithThrottle(() => { PrevPageButtonClicked?.Invoke(); });
        }
    }
}