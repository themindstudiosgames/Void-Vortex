using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.MatchFinishPopup
{
    public class MatchFinishPopupView : ScreenView
    {
        [SerializeField] private Button homeButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private CanvasGroup popupCanvasGroup;

        public event Action HomeButtonClicked;
        public event Action NewGameButton;

        private void Awake()
        {
            CanvasGroup = popupCanvasGroup;
            homeButton.ActionWithThrottle(() => { HomeButtonClicked?.Invoke(); });
            newGameButton.ActionWithThrottle(() => { NewGameButton?.Invoke(); });
        }

        public void SetScoreText(int score) => scoreText.text = score.ToString();
    }
}