using UniRx;
using UnityEngine;

namespace Camera
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private bool updateConstantly;

        private Transform _transform;
        private Transform _mainCameraTransform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _mainCameraTransform = UnityEngine.Camera.main!.transform;
        }

        private void Start()
        {
            if (updateConstantly)
            {
                Observable.EveryLateUpdate().Subscribe(delegate
                {
                    _transform.eulerAngles = _mainCameraTransform.eulerAngles;
                }).AddTo(this);
            }
        }

        private void OnEnable()
        {
            _transform.eulerAngles = _mainCameraTransform.eulerAngles;
        }
    }
}