using Balances.Boosters;
using Balances.CollectableItems;
using Balances.Match;
using Balances.PlayerHole;
using UnityEngine;
using Zenject;

namespace Balances
{
    [CreateAssetMenu(fileName = "GameBalancesSO", menuName = "SO/GameBalances/GameBalancesSO")]
    public class GameBalancesSO : ScriptableObjectInstaller<GameBalancesSO>
    {
        [SerializeField] private CollectableItemBalancesSO collectableItemBalancesSo;
        [SerializeField] private PlayerHoleBalancesSO playerHoleBalancesSo;
        [SerializeField] private MatchBalancesSO matchBalancesSo;
        [SerializeField] private BoosterByProgressionBalanceSO boosterByProgressionSo;
        
        public override void InstallBindings()
        {
            Container.BindInstance(collectableItemBalancesSo);
            Container.BindInstance(playerHoleBalancesSo);
            Container.BindInstance(matchBalancesSo);
            Container.BindInstance(boosterByProgressionSo);
        }
    }
}