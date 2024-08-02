using UnityEngine;
using UnityEngine.EventSystems;

namespace Input.Game
{
    public class PlayerMovementPanel : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private Vector2 _pointerDownPosition;

        public Vector2 MovementDirection { get; private set; } = Vector2.zero;
        private const float MinMovementShift = 15f;

        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDownPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var direction = eventData.position - _pointerDownPosition;
            if (Mathf.Abs(direction.x) > MinMovementShift || Mathf.Abs(direction.y) > MinMovementShift)
            {
                MovementDirection = direction.normalized;
                _pointerDownPosition = eventData.position;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            MovementDirection = Vector2.zero;
        }
    }
}