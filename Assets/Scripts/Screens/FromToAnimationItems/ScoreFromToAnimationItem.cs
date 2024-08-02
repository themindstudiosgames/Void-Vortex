using System;
using DG.Tweening;
using Screens.FromToAnimationItems.ViewInfos;
using TMPro;
using UnityEngine;
using Zenject;

namespace Screens.FromToAnimationItems
{
    public class ScoreFromToAnimationItem : MonoBehaviour, IPoolable<ScoreFromToAnimationItemViewInfo, IMemoryPool>, IDisposable
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private AnimationCurve fadeCurve;
        private Transform _transform;
        private IMemoryPool _pool;

        private void Awake() => _transform = transform;

        public void OnDespawned()
        {
            
        }

        public void OnSpawned(ScoreFromToAnimationItemViewInfo info, IMemoryPool pool)
        {
            _pool = pool;
            scoreText.text = $"+{info.Score}";
            scoreText.alpha = 0;
            _transform.localScale = Vector3.zero;
            _transform.position = info.From;
            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(_transform.DOMove(info.Target.position, info.Duration))
                .Join(_transform.DOScale(1, info.Duration).SetEase(fadeCurve))
                .Join(scoreText.DOFade(1, info.Duration).SetEase(fadeCurve))
                .OnComplete(() =>
                {
                    info.OnItemLanded?.Invoke();
                    Dispose();
                });
        }

        public void Dispose() => _pool.Despawn(this);
    }
}