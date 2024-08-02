using Balances.CollectableItems;
using Balances.PlayerHole;
using UniRx;

namespace Data.DataProxy
{
    public class PlayerHoleDataProxy
    {
        private readonly CollectableItemBalancesSO _collectableItemBalancesSo;
        private readonly PlayerHoleBalancesSO _playerHoleBalancesSo;
        private readonly ReactiveProperty<float> _size = new ReactiveProperty<float>();
        private readonly ReactiveProperty<float> _magnetismHorizontalForce = new ReactiveProperty<float>();
        private readonly ReactiveProperty<float> _magnetismVerticalForce = new ReactiveProperty<float>();

        private float _speed;
        private float _speedBoost = 1;
        private float _horizontalMagnetism;
        private float _verticalMagnetism;
        private float _magnetismBoost = 1;
        public IReadOnlyReactiveProperty<float> Size => _size;
        public IReadOnlyReactiveProperty<float> MagnetismHorizontalForce => _magnetismHorizontalForce;
        public IReadOnlyReactiveProperty<float> MagnetismVerticalForce => _magnetismVerticalForce;
        public float TotalSpeed { private set; get; }

        private float _sizeIncreasedAfterUpgrade;
        
        public PlayerHoleDataProxy(PlayerHoleBalancesSO playerHoleBalances, CollectableItemBalancesSO collectableItemBalancesSo)
        {
            _collectableItemBalancesSo = collectableItemBalancesSo;
            _playerHoleBalancesSo = playerHoleBalances;
            InitializeStats();
            
            void InitializeStats()
            {
                _size.Value = _playerHoleBalancesSo.BaseSize;
                _speed = _playerHoleBalancesSo.Speed;
                _horizontalMagnetism = _playerHoleBalancesSo.MagnetismHorizontalForce;
                _verticalMagnetism = _playerHoleBalancesSo.MagnetismVerticalForce;
                UpdateSpeed();
                UpdateMagnetism();
            }
        }

        public void IncreaseSizeAndSpeedByItem(CollectableItemType type)
        {
            float sizeGrowth = _collectableItemBalancesSo.GetSizeGrowth(type);
            _sizeIncreasedAfterUpgrade += sizeGrowth;
            _size.Value += sizeGrowth;
            if (!(_sizeIncreasedAfterUpgrade >= 1)) return;
            
            _speed += _playerHoleBalancesSo.SpeedBySize;
            _horizontalMagnetism +=
                _playerHoleBalancesSo.MagnetismHorizontalForce * _playerHoleBalancesSo.MagnetismBySizeCoefficient;
            _verticalMagnetism +=
                _playerHoleBalancesSo.MagnetismVerticalForce * _playerHoleBalancesSo.MagnetismBySizeCoefficient;
            _sizeIncreasedAfterUpgrade -= 1;
            UpdateSpeed();
            UpdateMagnetism();
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _speedBoost = multiplier;
            UpdateSpeed();
        }
        
        public void SetMagnetismForceMultiplier(float multiplier)
        {
            _magnetismBoost = multiplier;
            UpdateMagnetism();
        }

        private void UpdateSpeed() => TotalSpeed = _speed * _speedBoost;

        private void UpdateMagnetism()
        {
            _magnetismHorizontalForce.Value = _horizontalMagnetism * _magnetismBoost;
            _magnetismVerticalForce.Value = _verticalMagnetism * _magnetismBoost;
        }
    }
}