using Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Screens
{
    [RequireComponent(typeof(CanvasScaler))]
    public class ScreenOrientationCanvasScaler : MonoBehaviour
    {
        [SerializeField] private Vector2 portraitReferenceResolution;
        [SerializeField] private Vector2 desktopReferenceResolution;

        private void Awake()
        {
            CanvasScaler mainCanvasScaler = GetComponent<CanvasScaler>();
            mainCanvasScaler.referenceResolution =
                ScreenHelper.IsPortrait() ? portraitReferenceResolution : desktopReferenceResolution;
        }
    }
}