using System;
using Balances.Match;
using Data.DataProxy;
using Infrastructure.GameStateMachine;
using Infrastructure.GameStateMachine.GameStates;
using SceneManagement;
using Screens.TransitionAnimations;
using Sounds;
using UniRx;
using Utils;
using Zenject;

namespace Screens.SelectLevelPopup
{
    public class SelectLevelPopupPresenter : ScreenPresenter
    {
        private const int LevelsPerPage = 12;
        private int _levelsCount = 15;
        
        private SelectLevelPopupView _view;
        private readonly ReactiveProperty<int> _currentPage = new();
        private readonly CompositeDisposable _currentPageListeners = new();

        private GameStateMachine _gameStateMachine;
        private MatchDataProxy _matchDataProxy;

        [Inject]
        private void Construct(GameStateMachine gameStateMachine, MatchDataProxy matchDataProxy)
        {
            _gameStateMachine = gameStateMachine;
            _matchDataProxy = matchDataProxy;
        }

        private void Awake()
        {
            _view = GetComponent<SelectLevelPopupView>();
            OnShowTransitionAnimation = new ScaleTransition(_view.CanvasGroup, 0, 1, 0.2f, true);
            OnHideTransitionAnimation = new ScaleTransition(_view.CanvasGroup, 1, 0, 0.2f, true);
            
            _view.NextPageButtonClicked += () => { _currentPage.Value++; };
            _view.PrevPageButtonClicked += () => { _currentPage.Value--; };

            _view.CloseButtonClicked += CloseScreen;
            OnShowCallback += _ =>
            {
                DisplayLevelsPage(_currentPage.Value);
                SoundsManager.PlaySound(AudioKey.OpenPopup);
            };
            OnHideCallback += () =>
            {
                _currentPageListeners?.Clear();
                SoundsManager.PlaySound(AudioKey.ClosePopup);
            };
            
            _currentPageListeners.AddTo(this);
        }

        private void Start()
        {
            _levelsCount = Enum.GetValues(typeof(MatchLevel)).Length;
            int pagesCount = _levelsCount / LevelsPerPage + 1;
            
            _view.PageListView.InitializePageList(pagesCount);
            for (int i = 0; i < pagesCount; i++)
            {
                int pageIndex = i;
                _view.PageListView.PageButtonsArray[i].OnClickWithThrottle()
                    .Subscribe(_ => _currentPage.Value = pageIndex)
                    .AddTo(this).AddTo(_view);
            }
            
            _currentPage.Value = 0;
            _currentPage.Subscribe(pageIndex =>
            {
                DisplayLevelsPage(pageIndex);
                _view.SetButtonsInteractable(pageIndex, pagesCount);
            }).AddTo(this);
        }

        private void DisplayLevelsPage(int pageIndex)
        {
            _currentPageListeners?.Clear();
            _view.PageListView.SetPageActiveByIndex(pageIndex);

            int firstLevelOnPage = pageIndex * LevelsPerPage + 1;

            for (int i = 0; i < LevelsPerPage; i++)
            {
                int levelVisualIndex = firstLevelOnPage + i;
                bool levelUnlocked = _levelsCount > levelVisualIndex - 1;
                LevelItemView currentLevelItem = _view.LevelItems[i];
                currentLevelItem.SetupView(levelVisualIndex, levelUnlocked);

                if (levelUnlocked)
                {
                    currentLevelItem.OnClick.Subscribe(_ =>
                        {
                            _matchDataProxy.SelectMatchLevel((MatchLevel) levelVisualIndex);
                            _matchDataProxy.StartNewMatch();
                            _gameStateMachine.Enter<LoadLevelState, SceneNames>(_matchDataProxy.CurrentSceneName());
                        })
                        .AddTo(_currentPageListeners).AddTo(_view);
                }
            }
        }
    }
}