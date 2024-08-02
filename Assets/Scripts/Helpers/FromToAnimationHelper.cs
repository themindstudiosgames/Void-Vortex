using Screens.FromToAnimationItems.Factories;
using Screens.FromToAnimationItems.ViewInfos;
using Screens.GameSceneHud;
using UnityEngine;

namespace Helpers
{
    public class FromToAnimationHelper
    {
        private readonly ScoreFromToAnimationItemFactory _scoreFromToAnimationItemFactory;
        private readonly ICounterView _scoreCounterView;
        private readonly UnityEngine.Camera _camera;

        public FromToAnimationHelper(ScoreFromToAnimationItemFactory scoreFromToAnimationItemFactory, ICounterView scoreCounterView)
        {
            _scoreFromToAnimationItemFactory = scoreFromToAnimationItemFactory;
            _scoreCounterView = scoreCounterView;
            _camera = UnityEngine.Camera.main;
        }

        public void SpawnScoreItem(Vector3 from, int score)
        {
            from = _camera.WorldToScreenPoint(from);
            _scoreCounterView.HideAmount(score);
            _scoreFromToAnimationItemFactory.Create(new ScoreFromToAnimationItemViewInfo()
            {
                From = from,
                Score = score,
                Duration = 1,
                Target = _scoreCounterView.Target,
                OnItemLanded = () => { _scoreCounterView.RevealHiddenAmount(score); }
            });
        }
    }
}