using System;
using System.Collections.Generic;
using Balances.Match;
using Data.DataProxy;
using Infrastructure.ClientAPI;
using PlayFab.ClientModels;
using Screens.LeaderboardPopup.Factories;
using Screens.TransitionAnimations;
using Sounds;
using UniRx;
using UnityEngine;
using Zenject;

namespace Screens.LeaderboardPopup
{
    public class LeaderboardPopupPresenter : ScreenPresenter
    {
        private LeaderboardPopupView _view;
        private LeaderboardsDataProxy _leaderboardsDataProxy;
        private UserDataProxy _userDataProxy;
        private readonly ReactiveProperty<MatchLevel> _displayedLeaderboardLevel = new();
        
        private ScreenNavigationSystem _screenNavigationSystem;
        private IApiInterface _apiInterface;

        private LeaderboardItemViewFactory _leaderboardItemViewFactory;
        private readonly List<LeaderboardItemView> _activeItemViews = new();

        [Inject]
        private void Construct(UserDataProxy userDataProxy, LeaderboardsDataProxy leaderboardsDataProxy, ScreenNavigationSystem screenNavigationSystem,
            LeaderboardItemViewFactory leaderboardItemViewFactory, IApiInterface apiInterface)
        {
            _userDataProxy = userDataProxy;
            _leaderboardsDataProxy = leaderboardsDataProxy;
            _screenNavigationSystem = screenNavigationSystem;
            _leaderboardItemViewFactory = leaderboardItemViewFactory;
            _apiInterface = apiInterface;
        }

        private void Awake()
        {
            _view = GetComponent<LeaderboardPopupView>();
            OnShowTransitionAnimation = new ScaleTransition(_view.CanvasGroup, 0, 1, 0.2f, true);
            OnHideTransitionAnimation = new ScaleTransition(_view.CanvasGroup, 1, 0, 0.2f, true);

            _view.CloseButtonClicked += CloseScreen;
            
            OnShowCallback += _ =>
            {
                _displayedLeaderboardLevel.SetValueAndForceNotify(MatchLevel.Level1Scene);
                SoundsManager.PlaySound(AudioKey.OpenPopup);
            };
            OnHideCallback += () => { SoundsManager.PlaySound(AudioKey.ClosePopup); };
        }

        private void Start()
        {
            Array levels = Enum.GetValues(typeof(MatchLevel));
            if (levels.Length > _view.LevelItems.Count)
            {
                int initialCount = _view.LevelItems.Count;
                for (int i = 0; i < levels.Length - initialCount; i++)
                {
                    _view.LevelItems.Add(Instantiate(_view.LevelItems[0], _view.LevelItems[0].transform.parent));
                }
            }
            
            UpdateLevelItemViews();
            
            for (int i = 0; i < _view.LevelItems.Count; i++)
            {
                MatchLevel level = (MatchLevel) i + 1;
                _view.LevelItems[i].OnClick.Subscribe(_ =>
                {
                    _displayedLeaderboardLevel.Value = level;
                }).AddTo(_view);
            }
            
            _displayedLeaderboardLevel
                .Subscribe(level =>
                {
                    _apiInterface.GetLeaderboardForLevel(level,
                        leaderboardResult => { _leaderboardsDataProxy.SaveLeaderboardForLevel(level, leaderboardResult); },
                        error => { Debug.Log($"Error with get Leaderboard {level}: {error}"); });
                    
                    List<PlayerLeaderboardEntry> leaderboard = _leaderboardsDataProxy.GetLeaderboardForLevel(level).Value.Leaderboard;

                    DisplayLeaderboard(leaderboard);
                    UpdateLevelItemViews();
                }).AddTo(this);
            
            foreach (MatchLevel level in levels)
            {
                _leaderboardsDataProxy.GetLeaderboardForLevel(level).Subscribe(result =>
                {
                    if (level == _displayedLeaderboardLevel.Value)
                    {
                        DisplayLeaderboard(result.Leaderboard);
                    }
                }).AddTo(this);
            }
        }

        private void DisplayLeaderboard(List<PlayerLeaderboardEntry> leaderboard)
        {
            _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand().CloseScreen(ScreenName.OverlayScreen));
            
            foreach (LeaderboardItemView activeItemView in _activeItemViews)
            {
                activeItemView.Dispose();
            }
            _activeItemViews.Clear();
            foreach (PlayerLeaderboardEntry entry in leaderboard)
            {
                LeaderboardItemView itemView = _leaderboardItemViewFactory.Create(new LeaderboardItemViewInfo
                    {Entry = entry, Parent = _view.LeaderboardItemViewsParent});
                _activeItemViews.Add(itemView);
            }
            
            PlayerLeaderboardEntry userEntry = leaderboard.Find((entry => entry.PlayFabId == _userDataProxy.UserPlayFabID));
            _view.UserItem.gameObject.SetActive(userEntry != null);
            if (userEntry != null)
            {
                _view.UserItem.SetupViewEntryInfo(userEntry);
            }
        }

        private void UpdateLevelItemViews()
        {
            for (int i = 0; i < _view.LevelItems.Count; i++)
            {
                int levelIndex = i + 1;
                _view.LevelItems[i].SetupView(levelIndex, true, levelIndex == (int)_displayedLeaderboardLevel.Value);
            }
        }
    }
}