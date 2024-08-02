using UnityEngine;
using Utils;

namespace Sounds
{
    [CreateAssetMenu(fileName = "AudioAssetsSO", menuName = "SO/GameAssets/Audio")]
    public class AudioAssetsSO : ScriptableObject
    {
        [field: SerializeField] public SerializedDictionary<AudioKey, AudioClip> AudioByKey { private set; get; }
    }
}