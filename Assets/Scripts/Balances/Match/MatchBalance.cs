using System;
using UnityEngine;

namespace Balances.Match
{
    [Serializable]
    public class MatchBalance
    {
        [field: SerializeField] public int MatchDuration { get; private set; }
        [field: SerializeField] public MatchLevel[] AvailableLevels { get; private set; }
    }
}