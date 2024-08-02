using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utils;

namespace Screens.GameSceneHud
{
    public class TimerAndScorePanelView : MonoBehaviour, ICounterView
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform root;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private float closeWidth;
        [SerializeField] private float openWidth;
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material lowTimeMaterial;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color lowTimeColor;
        [SerializeField] private Transform scoreTarget;
        public Transform Target => scoreTarget;
        
        private int _hiddenScoreAmount;
        private int _score;

        public void SetMatchLeftTime(int leftTime)
        {
            timeText.text = leftTime.ToTimerText();
            bool lowTime = leftTime <= 5;
            timeText.fontMaterial = lowTime ? lowTimeMaterial : normalMaterial;
            timeText.color = lowTime ? lowTimeColor : normalColor;
        }

        public void Close()
        {
            Vector2 endSizeDelta = new Vector2(closeWidth, root.sizeDelta.y);
            root.sizeDelta = endSizeDelta;
        }

        public void SetPointsText(int score)
        {
            _score = score;
            UpdateScoreText();
        }

        public void Open(float duration, Action callback = null)
        {
            Vector2 endSizeDelta = new Vector2(openWidth, root.sizeDelta.y);
            root.DOSizeDelta(endSizeDelta, duration).OnComplete(() => { callback?.Invoke();});
        }

        public void Hide(float duration, Action callback = null) =>
            canvasGroup.DOFade(0,duration).OnComplete(() => { callback?.Invoke();});

        public void HideAmount(int hiddenAmount)
        {
            _hiddenScoreAmount += hiddenAmount;
            UpdateScoreText();
        }

        public void RevealHiddenAmount(int hiddenAmount)
        {
            _hiddenScoreAmount -= hiddenAmount;
            UpdateScoreText();
        }
        
        private void UpdateScoreText() => scoreText.text = (_score - _hiddenScoreAmount).ToString();
    }
}