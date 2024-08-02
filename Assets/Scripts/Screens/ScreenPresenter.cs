using System;
using Screens.TransitionAnimations;
using UnityEngine;
using Zenject;

namespace Screens
{
    [RequireComponent(typeof(UILayerConfigurator), typeof(ZenAutoInjecter))]
    public abstract class ScreenPresenter : MonoBehaviour
    {
        public event Action<object> OnShowCallback;
        public event Action OnGotFocusCallback;
        public event Action OnLostFocusCallback;
        public event Action OnScreenActivated;
        public event Action OnHideCallback;
        private Action _onCloseAction;

        public IScreenTransitionAnimation OnShowTransitionAnimation { get; protected set; }
        public IScreenTransitionAnimation OnHideTransitionAnimation { get; protected set; }

        private bool _isOnPosition;
        private float _inactiveTimer;
        
        private UILayerConfigurator _uiLayerConfigurator;
        private ScreenView _screenView;
        
        private CanvasGroup CanvasGroup => ScreenView.CanvasGroup;
        private UILayerConfigurator UILayerConfigurator => _uiLayerConfigurator ??= GetComponent<UILayerConfigurator>();
        private ScreenView ScreenView => _screenView ??= GetComponent<ScreenView>();
        public bool IsFocused { get; private set; }
        public bool IsRootScreen => UILayerConfigurator.OrderLayer == UIOrderLayer.Root;

        private void SetScreenVisible(bool visible)
        {
            CanvasGroup.blocksRaycasts = visible;
            CanvasGroup.interactable = visible;
            CanvasGroup.alpha = Convert.ToInt32(visible);
        }
        
        public void PrepareScreen()
        {
            SetScreenVisible(false);
        }

        public void ShowOnPosition(object extraData)
        {
            ActivateScreen();
            _isOnPosition = true;
            OnShowTransitionAnimation?.KillAnim();
            InvokeShowWith(extraData);
            GotFocus();
        }

        private void ActivateScreen()
        {
            Debug.Log("ScreenActivate: " + name);
            gameObject.SetActive(true);
            OnScreenActivated?.Invoke();
        }

        private void DeactivateScreen()
        {
            Debug.Log("ScreenDeactivate: " + name);
            SetScreenVisible(false);
            gameObject.SetActive(false);
            LostFocus();
        }

        public void MoveToInitialPosition()
        {
            _isOnPosition = false;
            OnHideCallback?.Invoke();
            DeactivateScreen();
        }

        public void GotFocus()
        {
            if (IsFocused) return;
            IsFocused = true;
            SetScreenVisible(true);
            UILayerConfigurator.BackToDefaultOrder();
            Debug.Log("ScreenGotFocus: " + name);
            OnGotFocusCallback?.Invoke();
        }

        public void LostFocus()
        {
            if (!IsFocused) return;
            IsFocused = false;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            Debug.Log("ScreenLostFocus: " + name);
            OnLostFocusCallback?.Invoke();
        }

        public void PerformHideAnimation(Action callback)
        {
            OnShowTransitionAnimation?.KillAnim();
            UILayerConfigurator.SetHideAnimatingOrder();
            OnHideTransitionAnimation?.PerformAnimation(delegate { callback?.Invoke(); });
        }

        public void PerformShowAnimationWhenReady(Action callback)
        {
            ActivateScreen();
            UILayerConfigurator.SetShowAnimatingOrder();
            OnHideTransitionAnimation?.KillAnim();
            OnShowTransitionAnimation?.PerformAnimation(delegate
            {
                GotFocus();
                callback();
            });
        }

        public void InvokeShowWith(object extraData)
        {
            OnShowCallback?.Invoke(extraData);
        }

        public void CloseScreen()
        {
            _onCloseAction();
        }

        public void LayUnderScreen(int shift = 1)
        {
            UILayerConfigurator.SetDefaultLayer(shift);
        }

        public void SetCloseAction(Action closeAction)
        {
            _onCloseAction = closeAction;
        }
    }
}