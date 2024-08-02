using System.Collections.Generic;
using Data.DataProxy;
using Gameplay.Level;
using Gameplay.Match;
using Gameplay.PlayerHole.Systems;
using Input.Game;
using UniRx;
using UnityEngine;
using Zenject;

namespace Gameplay.PlayerHole
{
    public class PlayerHolePresenter : MonoBehaviour
    {
        private PlayerHoleView _view;
        private CollectableItemsDataProxy _collectableItemsDataProxy;
        private IMovementInputHandler _movementInputHandler;
        private PlayerHoleDataProxy _playerHoleDataProxy;
        private MatchDataProxy _matchDataProxy;
        private BoosterByProgressionDataProxy _boosterByProgressionDataProxy;
        
        private readonly List<IPlayerHoleSystem> _systems = new List<IPlayerHoleSystem>(2);
        private MovementBorder _movementBorder;

        private Transform _transform;
        public float CurrentSize => _view.HoleRoot.localScale.x;
        public Transform ScoreFromToAnimationPoint => _view.ScoreFromToAnimationPoint;
        
        [Inject]
        private void Construct(IMovementInputHandler movementInputHandler,
            MatchDataProxy matchDataProxy,
            CollectableItemsDataProxy collectableItemsDataProxy, MovementBorder movementBorder,
            PlayerHoleDataProxy playerHoleDataProxy, BoosterByProgressionDataProxy boosterByProgressionDataProxy)
        {
            _movementBorder = movementBorder;
            _playerHoleDataProxy = playerHoleDataProxy;
            _collectableItemsDataProxy = collectableItemsDataProxy;
            _movementInputHandler = movementInputHandler;
            _matchDataProxy = matchDataProxy;
            _boosterByProgressionDataProxy = boosterByProgressionDataProxy;
        }

        private void Awake()
        {
            _view = GetComponent<PlayerHoleView>();
            _transform = transform;
            InitializeSystems();
        }

        private void InitializeSystems()
        {
            _systems.Add(new HoleMovementSystem(this, _movementInputHandler));
            _systems.Add(new HoleProgressionSystem(this, _collectableItemsDataProxy, _playerHoleDataProxy));
            _systems.Add(new HoleAbsorberSystem(_view.CollectableItemAbsorber, _view.ItemCollectorTrigger, _playerHoleDataProxy));

            _matchDataProxy.MatchState.Subscribe(state =>
            {
                foreach (IPlayerHoleSystem system in _systems)
                {
                    system.SetActive(state == MatchStateType.InProgress);
                }
            }).AddTo(this);

            foreach (IPlayerHoleSystem system in _systems)
            {
                system.Initialize();
            }

            _boosterByProgressionDataProxy.BoosterActive.Skip(1).Subscribe(active =>
            {
                _view.SetBoosterActive(active);
            }).AddTo(this);
        }

        public void Move(Vector2 movementDirection)
        {
            _transform.position = _movementBorder.ClampPosition(_transform.position + new Vector3(movementDirection.x, 0,
                movementDirection.y) * (_playerHoleDataProxy.TotalSpeed * Time.deltaTime));
        }

        public void SetSize(float size)
        {
            Vector3 scale = Vector3.one * size;
            scale.y = _view.HoleRoot.localScale.y;
            _view.HoleRoot.localScale = scale;
        }

        private void OnDestroy()
        {
            foreach (IPlayerHoleSystem system in _systems)
            {
                system.Dispose();
            }
        }
    }
}