using System.Collections.Generic;
using Balances.Match;
using PlayFab.ClientModels;
using UniRx;

namespace Data.DataProxy
{
    public class LeaderboardsDataProxy
    {
        private readonly IReadOnlyDictionary<MatchLevel, ReactiveProperty<GetLeaderboardResult>> _leaderboardsLevelMap =
            new Dictionary<MatchLevel, ReactiveProperty<GetLeaderboardResult>>
            {
                {MatchLevel.Level1Scene, new ReactiveProperty<GetLeaderboardResult>(new GetLeaderboardResult { Leaderboard = new List<PlayerLeaderboardEntry>() })},
                {MatchLevel.Level2Scene, new ReactiveProperty<GetLeaderboardResult>(new GetLeaderboardResult { Leaderboard = new List<PlayerLeaderboardEntry>() })},
                {MatchLevel.Level3Scene, new ReactiveProperty<GetLeaderboardResult>(new GetLeaderboardResult { Leaderboard = new List<PlayerLeaderboardEntry>() })},
                {MatchLevel.Level4Scene, new ReactiveProperty<GetLeaderboardResult>(new GetLeaderboardResult { Leaderboard = new List<PlayerLeaderboardEntry>() })},
                {MatchLevel.Level5Scene, new ReactiveProperty<GetLeaderboardResult>(new GetLeaderboardResult { Leaderboard = new List<PlayerLeaderboardEntry>() })},
                {MatchLevel.Level6Scene, new ReactiveProperty<GetLeaderboardResult>(new GetLeaderboardResult { Leaderboard = new List<PlayerLeaderboardEntry>() })},
                {MatchLevel.Level7Scene, new ReactiveProperty<GetLeaderboardResult>(new GetLeaderboardResult { Leaderboard = new List<PlayerLeaderboardEntry>() })},
                {MatchLevel.Level8Scene, new ReactiveProperty<GetLeaderboardResult>(new GetLeaderboardResult { Leaderboard = new List<PlayerLeaderboardEntry>() })},
                {MatchLevel.Level9Scene, new ReactiveProperty<GetLeaderboardResult>(new GetLeaderboardResult { Leaderboard = new List<PlayerLeaderboardEntry>() })},
            };
        
        public IReadOnlyReactiveProperty<GetLeaderboardResult> GetLeaderboardForLevel(MatchLevel level) => _leaderboardsLevelMap[level];
        
        public void SaveLeaderboardForLevel(MatchLevel level, GetLeaderboardResult leaderboardResult)
        {
            _leaderboardsLevelMap[level].Value = leaderboardResult;
        }
    }
}