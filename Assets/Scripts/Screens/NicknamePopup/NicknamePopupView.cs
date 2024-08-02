using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.NicknamePopup
{
    public class NicknamePopupView : ScreenView
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button sendButton;
        [SerializeField] private CanvasGroup popupCanvasGroup;
        [SerializeField] private TMP_Text invalidNicknameMessage;

        [SerializeField] private Sprite invalidNicknameBackground;
        [SerializeField] private Sprite defaultNicknameBackground;

        [SerializeField] private Color invalidTextColor;
        [SerializeField] private Color defaultTextColor;
        [field: SerializeField] public TMP_InputField NicknameInputField { get; private set; }
        
        public event Action SendClicked;
        public event Action CloseClicked;
        
        private void Awake()
        {
            CanvasGroup = popupCanvasGroup;
            sendButton.ActionWithThrottle(() => SendClicked?.Invoke());
            closeButton.ActionWithThrottle(() => CloseClicked?.Invoke());
        }

        public void SetIsValid(bool isValid)
        {
            NicknameInputField.image.sprite = isValid ? defaultNicknameBackground : invalidNicknameBackground;
            NicknameInputField.textComponent.color = isValid ? defaultTextColor : invalidTextColor;
            invalidNicknameMessage.gameObject.SetActive(!isValid);
            sendButton.interactable = isValid;
        }

        public void SetNicknameErrorMessage(string error)
        {
            invalidNicknameMessage.text = error;
        }
    }
}