using Data.DataProxy;
using Infrastructure.GameStateMachine;

namespace Screens.MatchFinishPopup
{
    public struct MatchFinishPopupDependencies
    {
        public MatchFinishPopupDependencies(GameStateMachine gameStateMachine, MatchDataProxy matchDataProxy)
        {
            GameStateMachine = gameStateMachine;
            MatchDataProxy = matchDataProxy;
        }

        public GameStateMachine GameStateMachine { get; }
        public MatchDataProxy MatchDataProxy { get; }
        

    }
}