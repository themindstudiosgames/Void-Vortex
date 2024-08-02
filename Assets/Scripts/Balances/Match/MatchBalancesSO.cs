using System.Linq;
using UnityEngine;

namespace Balances.Match
{
    [CreateAssetMenu(fileName = "MatchBalancesSO", menuName = "SO/GameBalances/MatchBalances")]
    public class MatchBalancesSO : ScriptableObject
    {
        [SerializeField] private MatchBalance[] matchBalances;

        public MatchBalance GetMatchBalanceByLevel(MatchLevel matchLevel) => 
            matchBalances.First(m => m.AvailableLevels.Contains(matchLevel));
    }
}