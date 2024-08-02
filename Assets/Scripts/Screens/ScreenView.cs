using UnityEngine;

namespace Screens
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ScreenView : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        public CanvasGroup CanvasGroup
        {
            get => _canvasGroup ??= GetComponent<CanvasGroup>();
            protected set => _canvasGroup = value;
        }
    }
}