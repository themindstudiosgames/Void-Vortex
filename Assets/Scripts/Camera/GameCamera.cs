using System;
using System.Collections;
using Cinemachine;
using Data.DataProxy;
using UniRx;
using UnityEngine;
using Zenject;
using Observable = UniRx.Observable;

namespace Camera
{
    public class GameCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera transposerCamera;
        [SerializeField] private CinemachineVirtualCamera previewCamera;

        private PlayerHoleDataProxy _playerHoleDataProxy;
        private CinemachineFramingTransposer _cinemachineFramingTransposer;
        private Coroutine _changeSizeCoroutine;
        private float _transposerDefaultSize;

        [Inject]
        private void Construct(PlayerHoleDataProxy playerHoleDataProxy)
        {
            _playerHoleDataProxy = playerHoleDataProxy;
        }
        
        private void Awake()
        {
            _cinemachineFramingTransposer = transposerCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _transposerDefaultSize = _cinemachineFramingTransposer.m_CameraDistance;
        }

        private void Start()
        {
            SetTransposerCameraSize(GetCameraSize(), 0);
            _playerHoleDataProxy.Size.Subscribe(_ =>
            {
                SetTransposerCameraSize(GetCameraSize());
            });
            return;

            float GetCameraSize()
            {
                return _transposerDefaultSize - 1 + _playerHoleDataProxy.Size.Value * 2;
            }
        }
        
        public void ShowScenePreview(float duration, Action callback)
        {
            SetActivePreviewCamera(true);
            Observable.Timer(TimeSpan.FromSeconds(duration)).Subscribe(_ =>
            {
                SetActivePreviewCamera(false);
                callback?.Invoke();
            });
            void SetActivePreviewCamera(bool active)
            {
                transposerCamera.gameObject.SetActive(!active);
                previewCamera.gameObject.SetActive(active);
            }
        }
        
        private void SetTransposerCameraSize(float size, float duration = 1)
        {
            float currentSize = _cinemachineFramingTransposer.m_CameraDistance;
            float needSize = size;
            float progress = 0f;

            if (duration == 0f)
            {
                _cinemachineFramingTransposer.m_CameraDistance = needSize;
                return;
            }

            if (_changeSizeCoroutine != null) return;
            _changeSizeCoroutine = StartCoroutine(ChangeSizeCoroutine());
    
            IEnumerator ChangeSizeCoroutine()
            {
                float progressScale = 1f / duration;
                while (progress <= 1f)
                {
                    progress += Time.deltaTime * progressScale;
                    _cinemachineFramingTransposer.m_CameraDistance = Mathf.Lerp(currentSize, needSize, progress);
                    yield return null;
                }

                _changeSizeCoroutine = null;
            }
        }
    }
}