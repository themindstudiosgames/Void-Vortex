using Data.DataProxy;
using Helpers;
using Infrastructure.ClientAPI;
using Infrastructure.GameStateMachine;
using SceneManagement;
using Screens;
using Screens.LeaderboardPopup;
using Screens.LeaderboardPopup.Factories;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ProjectContextInstaller : MonoInstaller
    {
        [SerializeField] private LeaderboardItemView leaderboardItemViewPrefab;
        [SerializeField] private LeaderboardItemView leaderboardItemViewDesktopPrefab;
        [SerializeField] private Transform uiPoolsParent;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle().NonLazy();
            Container.Bind<ScreensInstaller>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<ScreenNavigationSystem>().AsSingle().NonLazy();
            Container.BindInterfacesTo<PlayFabInterface>().AsSingle().NonLazy();
            Container.Bind<SceneLoader>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<InternetRequestsScreenHelper>().FromComponentInHierarchy().AsSingle().NonLazy();
            BindDataProxies();
            
            Container.BindFactory<LeaderboardItemViewInfo, LeaderboardItemView, LeaderboardItemViewFactory>()
                .FromPoolableMemoryPool<LeaderboardItemViewInfo, LeaderboardItemView, LeaderboardItemViewPool>(x =>
                    x.WithInitialSize(100).FromComponentInNewPrefab(
                        ScreenHelper.IsPortrait() ? leaderboardItemViewPrefab : leaderboardItemViewDesktopPrefab).UnderTransform(uiPoolsParent));
        }

        private void BindDataProxies()
        {
            Container.Bind<MatchDataProxy>().AsSingle().NonLazy();
            Container.Bind<UserDataProxy>().AsSingle().NonLazy();
            Container.Bind<LeaderboardsDataProxy>().AsSingle().NonLazy();
        }
        
        private class LeaderboardItemViewPool : MonoPoolableMemoryPool<LeaderboardItemViewInfo, IMemoryPool, LeaderboardItemView>
        {
        }
    }
}