using Data.DataProxy;
using Infrastructure.GameStateMachine;
using Infrastructure.GameStateMachine.GameStates;
using SceneManagement;
using Screens.TransitionAnimations;
using Sounds;

namespace Screens.MatchFinishPopup
{
    public class MatchFinishPopupPresenter : ScreenPresenter
    {
        private MatchFinishPopupView _view;
        private GameStateMachine _gameStateMachine;
        private MatchDataProxy _matchDataProxy;
        
        private void Awake()
        {
            _view = GetComponent<MatchFinishPopupView>();
            OnShowTransitionAnimation = new ScaleTransition(_view.CanvasGroup, 0, 1);
            OnHideTransitionAnimation = new ScaleTransition(_view.CanvasGroup, 1, 0);
            _view.HomeButtonClicked += () =>
            {
                SoundsManager.PlaySound(AudioKey.Button);
                _gameStateMachine.Enter<LoadLevelState, SceneNames>(SceneNames.MainScene);
            };
            _view.NewGameButton += () =>
            {
                SoundsManager.PlaySound(AudioKey.Button);
                _matchDataProxy.StartNewMatch();
                _gameStateMachine.Enter<LoadLevelState, SceneNames>(_matchDataProxy.CurrentSceneName());
            };
            OnShowCallback += data =>
            {
                SoundsManager.PlaySound(AudioKey.GameOverPopup);
                ResolveDependencies((MatchFinishPopupDependencies)data);
                _view.SetScoreText(_matchDataProxy.Score.Value);
            };
        }

        private void ResolveDependencies(MatchFinishPopupDependencies dependencies)
        {
            _gameStateMachine = dependencies.GameStateMachine;
            _matchDataProxy = dependencies.MatchDataProxy;
        }
    }
}