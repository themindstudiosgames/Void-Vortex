using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Screens.TransitionAnimations
{
    public class ScaleTransition : IScreenTransitionAnimation
    {
        private readonly CanvasGroup _canvasGroup;
        private readonly Transform _transform;
        private readonly float _duration;
        private readonly Vector3 _fromScale;
        private readonly Vector3 _toScale;
        private readonly bool _ignoreTimeScale;

        private TweenerCore<Vector3, Vector3, VectorOptions> _anim;

        public ScaleTransition(CanvasGroup canvasGroup, float fromScale, float toScale, float showDuration = 0.2f, bool ignoreTimeScale = false)
        {
            _canvasGroup = canvasGroup;
            _transform = canvasGroup.transform;
            _duration = showDuration;
            _fromScale = Vector3.one * fromScale;
            _toScale = Vector3.one * toScale;
            _ignoreTimeScale = ignoreTimeScale;
        }

        public void PerformAnimation(Action onEndCallback = null)
        {
            _canvasGroup.alpha = 1;
            _transform.localScale = _fromScale;
            _anim = _transform.DOScale(_toScale, _duration).SetUpdate(_ignoreTimeScale).OnComplete(delegate { onEndCallback?.Invoke(); });
        }

        public bool IsPlaying => _anim.IsPlaying();

        public void KillAnim()
        {
            if (_anim == null) return;
            _anim.Kill();
            _anim = null;
            _transform.localScale = _toScale;
        }
    }
}