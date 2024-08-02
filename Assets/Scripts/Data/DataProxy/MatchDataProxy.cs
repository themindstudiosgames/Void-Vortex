using System;
using System.Collections.Generic;
using Balances.Boosters;
using Balances.CollectableItems;
using Balances.Match;
using Gameplay.Match;
using Newtonsoft.Json;
using SceneManagement;
using UniRx;
using UnityEngine;

namespace Data.DataProxy
{
    public class MatchDataProxy
    {
        private readonly MatchBalancesSO _matchBalancesSo;
        private readonly ReactiveProperty<int> _score = new ReactiveProperty<int>();
        private readonly ReactiveProperty<int> _matchLeftTime = new ReactiveProperty<int>();
        private readonly ReactiveProperty<int> _matchCountdownTime = new ReactiveProperty<int>();
        private readonly ReactiveProperty<MatchStateType> _matchState = new ReactiveProperty<MatchStateType>();
        private readonly Subject<int> _scoresAdded = new Subject<int>();

        private MatchBalance _currentMatchBalance;
        private string _gameId;
        private string _tournamentId;
        
        public IReactiveProperty<int> Score => _score;
        public IReactiveProperty<int> MatchLeftTime => _matchLeftTime;
        public IReactiveProperty<int> MatchCountdownTime => _matchCountdownTime;
        public IReactiveProperty<MatchStateType> MatchState => _matchState;
        public IObservable<int> ScoresAdded => _scoresAdded;
        public int ScoreMultiplier { set; get; } = 1;
        public MatchLevel MatchLevel { private set; get; }

        public MatchDataProxy(MatchBalancesSO matchBalancesSo)
        {
            _matchBalancesSo = matchBalancesSo;
        }

        public SceneNames CurrentSceneName() => 
            Enum.TryParse(MatchLevel.ToString(), out SceneNames sceneName) ? sceneName : SceneNames.Level1Scene;
        

        public void SelectMatchLevel(MatchLevel level)
        {
            MatchLevel = level;
            _currentMatchBalance = _matchBalancesSo.GetMatchBalanceByLevel(level);
        }

        public void StartNewMatch()
        {
            ResetMatchData();
        }

        private void ResetMatchData()
        {
            _matchState.Value = MatchStateType.NotReady;
            _score.Value = 0;
        }

        public void SetMatchState(MatchStateType state) => _matchState.Value = state;
        
        public void AddScore(int score)
        {
            int totalScore = score * ScoreMultiplier;
            _score.Value += totalScore;
            _scoresAdded.OnNext(totalScore);
        }

        public void InitializeTimersForCurrentMatch()
        {
            _matchLeftTime.Value = _currentMatchBalance.MatchDuration;
            _matchCountdownTime.Value = 5;
        }

        public void ElapseMatchCountDownTimer(int deltaTime) => _matchCountdownTime.Value -= deltaTime;

        public void ElapseMatchLeftTimer(int deltaTime) => _matchLeftTime.Value -= deltaTime;

        public void FinishMatch()
        {
            SetMatchState(MatchStateType.Finish);
        }
        
    }
}