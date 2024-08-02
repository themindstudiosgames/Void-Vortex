using DG.Tweening;
using UnityEngine;

namespace DubbzSDK.Screens.Shared
{
    public class Loader : MonoBehaviour
    {
        [SerializeField] private RectTransform rotatableTransform;

        private const float RotationDuration = 1.3f;

        private void OnEnable()
        {
           StartAnimation();
        }

        private void OnDisable()
        {
            StopAnimation();
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        private void StartAnimation()
        {
            StopAnimation();
            rotatableTransform.DOLocalRotate(new Vector3(0, 0, -360), RotationDuration, RotateMode.FastBeyond360)
                .SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);
        }

        private void StopAnimation()
        {
            rotatableTransform.DOKill();
        }
    }
}