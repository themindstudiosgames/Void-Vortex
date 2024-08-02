using Sounds;
using UnityEngine;
using Zenject;

namespace Assets
{
    [CreateAssetMenu(fileName = "GameAssetsSO", menuName = "SO/GameAssets/GameAssetsSO")]
    public class GameAssetsSO : ScriptableObjectInstaller<GameAssetsSO>
    {
        [SerializeField] private ScreenAssets screenAssets;
        [SerializeField] private SpriteAssetsSO spriteAssetsSo;
        [SerializeField] private AudioAssetsSO audioAssetsSo;

        public override void InstallBindings()
        {
            Container.BindInstance(spriteAssetsSo);
            Container.BindInstance(audioAssetsSo);
            Container.BindInstance(screenAssets);
        }
    }
}