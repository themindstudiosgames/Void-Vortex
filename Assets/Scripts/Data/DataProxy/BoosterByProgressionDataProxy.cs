using System;
using Balances.Boosters;
using UniRx;

namespace Data.DataProxy
{
    public class BoosterByProgressionDataProxy
    {
        private readonly BoosterByProgressionBalanceSO _boosterByProgressionBalanceSo;
        
        private readonly ReactiveProperty<int> _collectedItemsForBoost = new ReactiveProperty<int>();
        private readonly ReactiveProperty<int> _needItemsForBoost = new ReactiveProperty<int>();
        private readonly ReactiveProperty<bool> _boosterActive = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<float> _leftTime = new ReactiveProperty<float>();
        private readonly Subject<bool> _boosterActivated = new Subject<bool>();

        public IReactiveProperty<int> CollectedItemsForBoost => _collectedItemsForBoost;
        public IReactiveProperty<int> NeedItemsForBoost => _needItemsForBoost;
        public IReactiveProperty<bool> BoosterActive => _boosterActive;
        public IReactiveProperty<float> LeftTime => _leftTime;
        public IObservable<bool> BoosterActivated => _boosterActivated;
        public int ScoreBoostCoefficient => _boosterByProgressionBalanceSo.ScoreBoostCoefficient;
        public float SpeedBoostCoefficient => _boosterByProgressionBalanceSo.SpeedBoostCoefficient;
        public float MagnetismBoostCoefficient => _boosterByProgressionBalanceSo.MagnetismBoostCoefficient;

        public BoosterByProgressionDataProxy(BoosterByProgressionBalanceSO boosterByProgressionBalanceSo)
        {
            _boosterByProgressionBalanceSo = boosterByProgressionBalanceSo;
        }

        public void AddCollectedItem()
        {
            _collectedItemsForBoost.Value++;
            if (_collectedItemsForBoost.Value < _needItemsForBoost.Value) return;
            SetBoosterActive(true);
            _leftTime.Value += _boosterByProgressionBalanceSo.Duration;
            _collectedItemsForBoost.Value = 0;
        }

        public void ConfigureCurrentMilestone(int matchScore)
        {
            int pointsMilestoneLength = _boosterByProgressionBalanceSo.PointsMilestone.Length;
            int milestoneIndex = pointsMilestoneLength - 1;
            for (int i = 0; i < pointsMilestoneLength; i++)
            {
                if (matchScore >= _boosterByProgressionBalanceSo.PointsMilestone[i])
                {
                    milestoneIndex = i;
                }
                else
                {
                    break;
                }
            }
            _needItemsForBoost.Value = 
                (milestoneIndex + 1) * _boosterByProgressionBalanceSo.ObjectCountForEveryMilestone;
        }

        public void ElapseTimer(float deltaTime)
        {
            _leftTime.Value -= deltaTime;
            if (_leftTime.Value > 0) return;
            SetBoosterActive(false);
            _leftTime.Value = 0;
        }

        private void SetBoosterActive(bool active)
        {
            _boosterActive.Value = active;
            _boosterActivated.OnNext(active);
        }
    }
}