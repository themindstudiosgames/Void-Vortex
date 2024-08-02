using Assets.Types;
using UnityEngine;
using Utils;

namespace Assets
{
    [CreateAssetMenu(fileName = "SpriteAssetsSO", menuName = "SO/GameAssets/Sprite")]
    public class SpriteAssetsSO : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<ButtonType, Sprite> buttonSprites;
        
        public Sprite GetButtonSprite(ButtonType buttonType) => buttonSprites[buttonType];
    }
}