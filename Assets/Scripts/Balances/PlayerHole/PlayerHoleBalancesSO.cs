using UnityEngine;

namespace Balances.PlayerHole
{
    [CreateAssetMenu(fileName = "PlayerHoleBalancesSO", menuName = "SO/GameBalances/PlayerHole")]
    public class PlayerHoleBalancesSO : ScriptableObject
    {
        [field: SerializeField] public float Speed { private set; get; }
        [field: SerializeField] public float MagnetismHorizontalForce { private set; get; }
        [field: SerializeField] public float MagnetismVerticalForce { private set; get; }
        [field: SerializeField] public float BaseSize { private set; get; }
        [field: SerializeField] public float SpeedBySize { private set; get; }
        [field: SerializeField] public float MagnetismBySizeCoefficient { private set; get; }
    }
}