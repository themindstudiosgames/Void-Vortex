using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const float TimeToHold = 0.5f;
        private float _holdTime;
        private bool _interactable = true;
        private Coroutine _holdTimeCoroutine;
        public event Action Clicked;

        public void OnPointerDown(PointerEventData eventData)
        {
            _holdTime = 0f;
            _holdTimeCoroutine = StartCoroutine(HoldCoroutine());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_holdTimeCoroutine != null)
            {
                StopCoroutine(_holdTimeCoroutine);
            }

            if (_interactable && _holdTime < TimeToHold)
            {
                Clicked?.Invoke();
            }
        }

        public void SetInteractable(bool interactable) => _interactable = interactable;

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public Vector3 GetTopPosition()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector3 position = rectTransform.position;
            position.y += rectTransform.sizeDelta.y / 2;
            return position;
        }

        private IEnumerator HoldCoroutine()
        {
            while (_holdTime < TimeToHold)
            {
                _holdTime += Time.deltaTime;
                yield return null;
            }

            while (_interactable)
            {
                Clicked?.Invoke();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}