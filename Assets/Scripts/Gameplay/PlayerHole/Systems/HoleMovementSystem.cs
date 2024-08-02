using Input.Game;
using UniRx;

namespace Gameplay.PlayerHole.Systems
{
    public class HoleMovementSystem : IPlayerHoleSystem
    {
        private readonly PlayerHolePresenter _playerHolePresenter;
        private readonly IMovementInputHandler _movementInputHandler;
        private bool _active;

        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public HoleMovementSystem(PlayerHolePresenter playerHolePresenter,
            IMovementInputHandler movementInputHandler)
        {
            _playerHolePresenter = playerHolePresenter;
            _movementInputHandler = movementInputHandler;
        }

        public void Initialize()
        {
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                if(!_active) return;
                _playerHolePresenter.Move(_movementInputHandler.MovementDirection);
            }).AddTo(_compositeDisposable);
        }

        public void SetActive(bool active) => _active = active;

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}