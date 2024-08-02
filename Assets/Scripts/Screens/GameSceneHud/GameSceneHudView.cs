using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace Screens.GameSceneHud
{
    public class GameSceneHudView : MonoBehaviour
    {
        [field: SerializeField] public TimerAndScorePanelView TimerAndScorePanelView { private set; get; }
        [field: SerializeField] public BoosterProgressPanelView BoosterProgressPanelView { private set; get; }
        [SerializeField] private Button fullscreenButton;
        [SerializeField] private TextMeshProUGUI countdownTimerText;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button musicButton;
        
        private const float CountdownTextSizeNormal = 220;
        private const float CountdownTextSizeBig = 250;
        private const string GoKey = "GO";
        public event Action FullscreenClicked;
        public event Action SoundButtonClicked;
        public event Action MusicButtonClicked;
        
        private void Start()
        {
            fullscreenButton.OnPointerDownAsObservable().Subscribe(_ => { FullscreenClicked?.Invoke();}).AddTo(this);
            soundButton.ActionWithThrottle(() => { SoundButtonClicked?.Invoke(); });
            musicButton.ActionWithThrottle(() => { MusicButtonClicked?.Invoke(); });
        }

        public void SetFullscreenIcon(Sprite icon, Sprite pressedIcon)
        {
            fullscreenButton.image.sprite = icon;
            fullscreenButton.spriteState = new SpriteState() { pressedSprite = pressedIcon };
        }

        public void SetCountDownTimerActive(bool active)
        {
            countdownTimerText.gameObject.SetActive(active);
            countdownTimerText.alpha = 1;
            countdownTimerText.transform.localScale = Vector3.one;
        }

        public void SetCountDownTimer(int leftTime)
        {
            bool normal = leftTime > 0;
            countdownTimerText.text = normal ? leftTime.ToString() : GoKey;
            countdownTimerText.fontSize = normal ? CountdownTextSizeNormal : CountdownTextSizeBig;
        }

        public void HideCountdownTimer(float duration)
        {
            countdownTimerText.DOFade(0, duration);
            countdownTimerText.transform.DOScale(0, duration).OnComplete(() =>
            {
                countdownTimerText.gameObject.SetActive(false);
            });
        }

        public void SetSoundButtonState(bool active) => soundButton.image.SetAlpha(active ? 1 : 0.75f);
        public void SetMusicButtonState(bool active) => musicButton.image.SetAlpha(active ? 1 : 0.75f);
    }
}