using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utils;
using Zenject;

namespace Sounds
{
    public class SoundsManager : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private AudioMixerGroup soundsMixerGroup;

        private static SoundsManager _instance;
        private float? _musicVolume;
        private float? _soundsVolume;
        private bool? _isVibrationsOn;
        
        private const string MusicVolumeParam = "MusicVolume";
        private const string SoundsVolumeParam = "SoundsVolume";
        private const string Music = "Music";
        private const string Sounds = "Sounds";
        
        private const float MaxDB = 0f;
        private const float FactMinDB = -40f;
        private const float MinDB = -80f;
        private AudioMixer _audioMixer;
        private ComponentPool<AudioSource> _audioSourcePool;
        private AudioSource _musicSource;
        private IReadOnlyDictionary<AudioKey, AudioClip> _sounds;
        private AudioAssetsSO _audioAssetsSo;

        public static bool IsMusicOn => MusicVolume > FactMinDB;
        public static float MusicVolume
        {
            get
            {
                _instance._musicVolume ??= PlayerPrefs.GetFloat(Music, MaxDB);
                return _instance._musicVolume.Value;
            }
            set
            {
                if (_instance._musicVolume == value) return;
                if (value <= FactMinDB) value = MinDB;
                _instance._musicVolume = value;
                PlayerPrefs.SetFloat(Music, _instance._musicVolume.Value);
                PlayerPrefs.Save();
                SetMusicVolume(value);
            }
        }

        public static bool IsSoundOn => SoundsVolume > FactMinDB;
        public static float SoundsVolume
        {
            get
            {
                _instance._soundsVolume ??= PlayerPrefs.GetFloat(Sounds, MaxDB);
                return _instance._soundsVolume.Value;
            }
            set
            {
                if (_instance._soundsVolume == value) return;
                if (value <= FactMinDB) value = MinDB;
                _instance._soundsVolume = value;
                PlayerPrefs.SetFloat(Sounds, _instance._soundsVolume.Value);
                PlayerPrefs.Save();
                SetSoundsVolume(value);
            }
        }
        
        [Inject]
        public void Construct(AudioAssetsSO audioAssetsSo)
        {
            _audioAssetsSo = audioAssetsSo;
        }

        private void Awake()
        {
            _sounds = _audioAssetsSo.AudioByKey.Dictionary;
            if (_instance != null)
            {
                Debug.LogError("Trying to add second SOUNDS MANAGER");
                Destroy(gameObject);
            }

            _instance = this;
            _audioMixer = musicMixerGroup.audioMixer;
            _audioSourcePool = new ComponentPool<AudioSource>(gameObject);
        }

        private void Start()
        {
            SetMusicVolume(MusicVolume);
            SetSoundsVolume(SoundsVolume);
        }

        public static void ToggleMusic(bool active) => MusicVolume = active ? MaxDB : MinDB;
        
        public static void ToggleSound(bool active) => SoundsVolume = active ? MaxDB : MinDB;

        public static void PlaySound(AudioKey audioKey, float volume = 1f, bool loop = false)
        {
            PlaySoundAudioClip(_instance._sounds[audioKey], volume, loop);
        }

        public static void PlayMusic(AudioKey audioKey, float volume)
        {
            AudioSource musicSource = _instance._musicSource;
            if (musicSource == null) musicSource = _instance._audioSourcePool.Get();
            _instance._musicSource = musicSource;
            musicSource.outputAudioMixerGroup = _instance.musicMixerGroup;
            musicSource.volume = volume;
            musicSource.loop = true;
            musicSource.clip = _instance._sounds[audioKey];
            musicSource.Play();
        }

        private static void SetMusicVolume(float volume)
        {
            _instance._audioMixer.SetFloat(MusicVolumeParam, volume);
        }

        private static void SetSoundsVolume(float volume)
        {
            _instance._audioMixer.SetFloat(SoundsVolumeParam, volume);
        }

        private static void PlaySoundAudioClip(AudioClip audioClip, float volume, bool loop)
        {
            AudioSource audioSource = _instance._audioSourcePool.Get();
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = _instance.soundsMixerGroup;
            audioSource.volume = volume;
            audioSource.loop = loop;
            _instance.StartCoroutine(PlaySound(audioClip, audioSource, delegate { _instance._audioSourcePool.Release(audioSource); }));
        }

        private static IEnumerator PlaySound(AudioClip audioClip, AudioSource audioSource, Action callback = null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            if (audioSource.loop) yield break;
            yield return new WaitForSeconds(audioClip.length);
            callback?.Invoke();
        }
    }
}