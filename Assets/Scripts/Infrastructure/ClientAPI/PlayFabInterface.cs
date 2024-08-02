using System;
using System.Collections.Generic;
using System.Linq;
using Balances.Match;
using PlayFab;
using PlayFab.ClientModels;
using UniRx;
using UnityEngine;

namespace Infrastructure.ClientAPI
{
    public class PlayFabInterface : IApiInterface
    {
        private readonly ReactiveProperty<bool> _isRequestProcessing = new(false);
        private readonly List<Request> _requestsList = new();

        public IReadOnlyReactiveProperty<bool> IsRequestProcessing => _isRequestProcessing;
        
        public event Action RequestSendingFailed;
        public event Action RequestSentSuccessfully;

        public void LoginWithDeviceID(string deviceId, Action<LoginResult> onSuccess)
        {
            void NewRequest()
            {
                PlayFabClientAPI.LoginWithCustomID(
                    new LoginWithCustomIDRequest
                    {
                        CustomId = deviceId,
                        CreateAccount = true
                    },
                    result =>
                    {
                        DeleteExecutedRequestAndSendNext();
                        onSuccess?.Invoke(result);
                    },
                    FailureCallback);
            };

            AddRequestToList(new Request(nameof(LoginWithDeviceID), NewRequest));
        }

        public void GetPlayerProfile(Action<GetPlayerProfileResult> onSuccess)
        {
            void NewRequest() => PlayFabClientAPI.GetPlayerProfile(
                new GetPlayerProfileRequest(),
                result =>
                {
                    DeleteExecutedRequestAndSendNext();
                    onSuccess?.Invoke(result);
                },
                FailureCallback);

            AddRequestToList(new Request(nameof(GetPlayerProfile), NewRequest));
        }
        
        public void ChangeNickname(string nickname, Action<string> onSuccess, Action<string> onError)
        {
            void NewRequest() => PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = nickname,
                },
                result =>
                {
                    DeleteExecutedRequestAndSendNext();
                    onSuccess?.Invoke(result.DisplayName);
                },
                error =>
                {
                    DeleteExecutedRequestAndSendNext();
                    onError?.Invoke(error.ErrorMessage);
                });
            
            AddRequestToList(new Request(nameof(GetPlayerProfile), NewRequest));
        }
        
        public void GetLeaderboardForLevel(MatchLevel level, Action<GetLeaderboardResult> onSuccess, Action<string> onFailure)
        {
            void NewRequest() => PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
            {
                StatisticName = level.ToString(),
                StartPosition = 0,
                MaxResultsCount = 100
            }, result =>
            {
                DeleteExecutedRequestAndSendNext();
                onSuccess?.Invoke(result);
                
            }, error =>
            {
                DeleteExecutedRequestAndSendNext();
                onFailure?.Invoke(error.ErrorMessage);
            });
            
            AddRequestToList(new Request("GetLeaderboardFor" + level, NewRequest));
        }

        public void SubmitScore(MatchLevel level, int playerScore, Action onSuccess)
        {
            void NewRequest() => PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate
                        {
                            StatisticName = level.ToString(),
                            Value = playerScore
                        }
                    }
                },
                result =>
                {
                    DeleteExecutedRequestAndSendNext();
                    onSuccess?.Invoke();
                },
                FailureCallback);

            AddRequestToList(new Request(nameof(SubmitScore), NewRequest));
        }

        private void FailureCallback(PlayFabError error)
        {
            Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
            RequestSendingFailed?.Invoke();
        }

        #region request queue

        private void AddRequestToList(Request request, bool asFirst = false)
        {
            if (_requestsList.Any(someRequest => someRequest.Key == request.Key))
            {
                Debug.LogWarning($"{request.Key} already added in Requests List!" );
                return;
            }
            
            if (asFirst)
            {
                _requestsList.Insert(0, request);
            }
            else
            {
                _requestsList.Add(request);
            }

            if (_requestsList.Count == 1)
            {
                TryToSendNextRequest();
            }
        }

        public void TryToSendNextRequest()
        {
            if (_requestsList.Count == 0)
            {
                return;
            }
            
            _isRequestProcessing.Value = true;
            Debug.Log("Send request: " + _requestsList[0].Key);
            _requestsList[0]?.Action?.Invoke();
        }

        private void DeleteExecutedRequestAndSendNext()
        {
            if (_requestsList.Count == 0) return;
            RequestSentSuccessfully?.Invoke();
            _requestsList.RemoveAt(0);
            if (_requestsList.Count == 0) _isRequestProcessing.Value = false;
            TryToSendNextRequest();
        }

        #endregion // request queue
    }
}