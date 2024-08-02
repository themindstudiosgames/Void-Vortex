using Helpers;
using Screens.MainSceneScreen;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private Transform screensRoot;
        [SerializeField] private MainSceneScreenView portraitMainSceneScreenView;
        [SerializeField] private MainSceneScreenView desktopMainSceneScreenView;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MainSceneScreenPresenter>().AsSingle().NonLazy();
            Container.Bind<MainSceneScreenView>().FromComponentInNewPrefab(
                    ScreenHelper.IsPortrait() ? portraitMainSceneScreenView : desktopMainSceneScreenView)
                .UnderTransform(screensRoot).AsSingle();
        }
    }
}