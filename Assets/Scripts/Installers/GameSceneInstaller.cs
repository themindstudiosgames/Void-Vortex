using Camera;
using Data.DataProxy;
using Gameplay.Boosters;
using Gameplay.CollectableItems;
using Gameplay.Level;
using Gameplay.Match;
using Gameplay.PlayerHole;
using Helpers;
using Input.Game;
using Screens.FromToAnimationItems;
using Screens.FromToAnimationItems.Factories;
using Screens.FromToAnimationItems.ViewInfos;
using Screens.GameSceneHud;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private Transform fromToAnimationRoot;
        [SerializeField] private ScoreFromToAnimationItem scoreFromToAnimationItemPrefab;
        [SerializeField] private Transform screensRoot;
        [SerializeField] private GameSceneHudView portraitGameSceneHudView;
        [SerializeField] private GameSceneHudView desktopGameSceneHudView;

        public override void InstallBindings()
        {
            BindControllers();
            BindDataProxies();
            BindInput();
            BindObjects();
            BindFactories();
        }

        private void BindControllers()
        {
            Container.BindInterfacesAndSelfTo<CollectableItemsController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MatchController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BoosterByProgressionController>().AsSingle().NonLazy();
        }

        private void BindObjects()
        {
            Container.Bind<PlayerHolePresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CollectableItemPresenter>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<GameSceneHudView>().FromComponentInNewPrefab(
                    ScreenHelper.IsPortrait() ? portraitGameSceneHudView : desktopGameSceneHudView)
                .UnderTransform(screensRoot).AsSingle();
            Container.BindInterfacesTo<GameSceneHudPresenter>().AsSingle().NonLazy();
            Container.Bind<GameCamera>().FromComponentInHierarchy().AsSingle();
            Container.Bind<MovementBorder>().FromComponentInHierarchy().AsSingle();
            Container.Bind<FromToAnimationHelper>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ICounterView>().FromComponentInHierarchy().AsSingle();
        }

        private void BindDataProxies()
        {
            Container.BindInterfacesAndSelfTo<PlayerHoleDataProxy>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<CollectableItemsDataProxy>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BoosterByProgressionDataProxy>().AsSingle().NonLazy();
        }
        
        private void BindInput()
        {
            Container.BindInterfacesAndSelfTo<InputHandler>().FromComponentInHierarchy().AsSingle();
        }
        
        private void BindFactories()
        {
            Container.BindFactory<ScoreFromToAnimationItemViewInfo, ScoreFromToAnimationItem, ScoreFromToAnimationItemFactory>()
                .FromPoolableMemoryPool<ScoreFromToAnimationItemViewInfo, ScoreFromToAnimationItem, FromToAnimationItemPool>(poolBinder =>
                    poolBinder
                        .WithInitialSize(10).FromComponentInNewPrefab(scoreFromToAnimationItemPrefab)
                        .UnderTransform(fromToAnimationRoot));
        }
        
        private class FromToAnimationItemPool : MonoPoolableMemoryPool<ScoreFromToAnimationItemViewInfo, IMemoryPool, ScoreFromToAnimationItem>
        {
        }
    }
}