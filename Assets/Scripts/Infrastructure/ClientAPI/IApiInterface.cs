using System;
using Balances.Match;
using PlayFab.ClientModels;
using UniRx;

namespace Infrastructure.ClientAPI
{
    public interface IApiInterface
    {
        public IReadOnlyReactiveProperty<bool> IsRequestProcessing { get; }

        public event Action RequestSendingFailed;
        public event Action RequestSentSuccessfully;

        public void LoginWithDeviceID(string deviceId, Action<LoginResult> onSuccess);
        public void GetPlayerProfile(Action<GetPlayerProfileResult> onSuccess);
        public void ChangeNickname(string nickname, Action<string> onSuccess, Action<string> onError);
        public void GetLeaderboardForLevel(MatchLevel level, Action<GetLeaderboardResult> onSuccess,
            Action<string> onFailure);
        public void SubmitScore(MatchLevel level, int playerScore, Action onSuccess);
        public void TryToSendNextRequest();
    }
}