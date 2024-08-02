using UnityEngine;

namespace Balances.Boosters
{
    [CreateAssetMenu(fileName = "BoosterByProgressionBalanceSO", menuName = "SO/GameBalances/BoosterByProgression")]
    public class BoosterByProgressionBalanceSO : ScriptableObject
    {
        [field: SerializeField] public float Duration { private set; get; }
        [field: SerializeField] public float SpeedBoostCoefficient { private set; get; }
        [field: SerializeField] public float MagnetismBoostCoefficient { private set; get; }
        [field: SerializeField] public int ScoreBoostCoefficient { private set; get; }
        [field: SerializeField] public int[] PointsMilestone { private set; get; }
        [field: SerializeField] public int ObjectCountForEveryMilestone { private set; get; }
    }
}