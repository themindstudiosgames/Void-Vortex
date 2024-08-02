using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input.Game
{
    public class DynamicOnScreenStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private RectTransform stickParent;
        [SerializeField] private RectTransform stickCenter;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private bool hideOnPointerUp;

        [Header("Input Info")] [SerializeField]
        private float movementRange = 100;

        private Vector3 _startPos;
        private Vector2 _pointerDownPos;
        private CanvasGroup _stickCanvas;

        private bool _isReseted = true;

        public Vector2 MovementDirection { get; private set; } = Vector2.zero;

        public void OnPointerDown(PointerEventData eventData)
        {
            canvasGroup.alpha = 1;
            _isReseted = false;
            _stickCanvas.DOKill();
            _stickCanvas.DOFade(1f, 0.5f);
            stickParent.position = eventData.position;

            if (eventData == null) throw new ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(stickParent, eventData.position,
                eventData.pressEventCamera, out _pointerDownPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(_isReseted) return;
            if (eventData == null) throw new ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(stickParent, eventData.position,
                eventData.pressEventCamera, out Vector2 position);
            Vector2 delta = position - _pointerDownPos;

            float magnitude = delta.magnitude;
            delta = Vector2.ClampMagnitude(delta, movementRange);
            stickCenter.anchoredPosition = _startPos + (Vector3) delta;

            Vector2 newPos = delta / movementRange;
            MovementDirection = newPos;

            if (magnitude > movementRange)
            {
                stickParent.anchoredPosition += newPos * (magnitude - movementRange);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ResetStick();
        }

        private void ResetStick()
        {
            if (hideOnPointerUp)
            {
                canvasGroup.alpha = 0;
            }

            _isReseted = true;
            _stickCanvas.DOKill();
            _stickCanvas.DOFade(0f, 0.5f);
            stickCenter.anchoredPosition = _startPos;
            MovementDirection = Vector2.zero;
        }

        private void Start()
        {
            canvasGroup.alpha = hideOnPointerUp ? 0 : 1;
            _stickCanvas = stickParent.GetComponent<CanvasGroup>();
            _stickCanvas.alpha = 0f;
            _startPos = stickCenter.anchoredPosition;
        }
    }
}