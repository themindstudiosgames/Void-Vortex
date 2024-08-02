using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Screens
{
    public class ScreenNavigationSystem
    {
        private readonly List<ScreenPresenter> _navigationStack = new();
        private readonly Dictionary<ScreenName, ScreenPresenter> _availableScreens = new();
        private readonly Queue<NavigationCommand> _waitingCommands = new();
        private readonly ScreensInstaller _screensInstaller;
        
        private ScreenPresenter _currentShowAnimatedScreen;
        private IEnumerator _showInformationProcess;
        private ScreenName? _screenUnderProcessing;
        
        public ScreenNavigationSystem(ScreensInstaller screensInstaller)
        {
            _screensInstaller = screensInstaller;
        }

        private void GetNewScreen(NavigationCommand navigationCommand)
        {
            _screensInstaller.InstantiateScreen(navigationCommand.NextScreenName, screen =>
            {
                _screenUnderProcessing = null;
                AddScreen(navigationCommand.NextScreenName, screen);
                ExecuteNavigationCommand(navigationCommand);
            });
        }

        private void AddScreen(ScreenName name, ScreenPresenter screenPresenter)
        {
            _availableScreens.Add(name, screenPresenter);
            screenPresenter.PrepareScreen();
            screenPresenter.SetCloseAction(() => Close(screenPresenter));
        }

        public void ExecuteNavigationCommand(NavigationCommand navigationCommand)
        {
            if (_navigationStack.Count != 0)
            {
                if (navigationCommand.IsNextScreenInQueue)
                {
                    PreparePreviousScreens(navigationCommand);
                }

                if (navigationCommand.ScreenToClose != null &&
                    _availableScreens.TryGetValue(navigationCommand.ScreenToClose.Value,
                        out ScreenPresenter screenPresenter))
                {
                    Close(screenPresenter,
                        navigationCommand.IsNextScreenInQueue);
                }

                if (navigationCommand.IsCloseAllScreens)
                {
                    ForceCloseAllScreens();
                }
            }

            if (!navigationCommand.IsNextScreenInQueue) return;
            Show(navigationCommand);
        }

        private void PreparePreviousScreens(NavigationCommand navigationCommand)
        {
            var peek = _navigationStack.Last();
            peek.LostFocus();

            if (!_availableScreens.ContainsKey(navigationCommand.NextScreenName))
            {
                return;
            }

            var nextScreen = _availableScreens[navigationCommand.NextScreenName];

            //lay stack under next screen layer to correct show animation
            LayStackUnderNextScreen(nextScreen);

            if (peek == nextScreen) return;

            void OnNewScreenActivatedAction()
            {
                nextScreen.OnScreenActivated -= OnNewScreenActivatedAction;
            }

            nextScreen.OnScreenActivated += OnNewScreenActivatedAction;
        }

        private void LayStackUnderNextScreen(ScreenPresenter nextScreen)
        {
            if (nextScreen == null) return;
            var array = _navigationStack.ToArray();
            for (var index = 0; index < array.Length; index++)
            {
                ScreenPresenter screenPresenter = array[index];
                screenPresenter.LayUnderScreen(index + 1);
            }

            nextScreen.LayUnderScreen(array.Length + 1);
        }

        private void Show(NavigationCommand navigationCommand)
        {
            ScreenName screenName = navigationCommand.NextScreenName;
            object extraData = navigationCommand.ExtraData;

            Debug.Log("Try to show screen " + screenName);

            if (screenName == _screenUnderProcessing) return;

            if (!_availableScreens.ContainsKey(screenName))
            {
                Debug.Log($"Screen {screenName} has not loaded yet. Try to load.");
                _screenUnderProcessing = screenName;
                GetNewScreen(navigationCommand);
                return;
            }

            if (_currentShowAnimatedScreen == _availableScreens[screenName])
            {
                Debug.LogError("You are trying to show the same screen again. " + screenName);
                return;
            }

            if (_currentShowAnimatedScreen != null)
            {
                Debug.Log("animation of another screen is still running (WAIT)");
                if (_waitingCommands.Any(nc => nc.NextScreenName == screenName)) return;
                _waitingCommands.Enqueue(navigationCommand);
                return;
            }

            var screenPresenter = _availableScreens[screenName];
            CheckRootScreenShown(screenPresenter);
            AddScreenToStack(screenPresenter);
            
            if (screenPresenter.OnShowTransitionAnimation != null)
            {
                _currentShowAnimatedScreen = screenPresenter;
                screenPresenter.PerformShowAnimationWhenReady(
                    delegate
                    {
                        _currentShowAnimatedScreen = null;
                        if (_waitingCommands.Count <= 0) return;
                        var waitingNavigationCommand = _waitingCommands.Dequeue();
                        ExecuteNavigationCommand(waitingNavigationCommand);
                    });
                screenPresenter.InvokeShowWith(extraData);
            }
            else
            {
                screenPresenter.ShowOnPosition(extraData);
            }
        }

        private void CheckRootScreenShown(ScreenPresenter screenPresenter)
        {
            if (!screenPresenter.IsRootScreen) return;
            Debug.Log("Root screen shown. Close all screens");

            for (int i = _navigationStack.Count - 1; i >= 0; i--)
            {
                if (screenPresenter != _navigationStack[i])
                {
                    Close(_navigationStack[i]);
                }
            }
        }

        private void AddScreenToStack(ScreenPresenter screenPresenter)
        {
            if (_navigationStack.Count != 0 && _navigationStack.Contains(screenPresenter))
            {
                Debug.LogWarning("Can not add screen " + screenPresenter.name + " to stack because they already in");
                return;
            }

            _navigationStack.Add(screenPresenter);
            Debug.Log("Screen ADDED to stack " + screenPresenter.name);
        }

        private void RemoveScreenFromStack(ScreenPresenter screenPresenter, bool nextScreenInQueue = false)
        {
            if (_navigationStack.Count == 0) return;

            if (!_navigationStack.Contains(screenPresenter)) return;
            
            _navigationStack.Remove(screenPresenter);
            screenPresenter.LostFocus();
                
            if (_navigationStack.Count > 0 && !nextScreenInQueue)
            {
                //move focus to previous screen
                var activeScreen = _navigationStack.Last();
                activeScreen.GotFocus();
            }

            Debug.Log("Screen REMOVED from stack " + screenPresenter.name);
        }

        private void Close(ScreenPresenter screenPresenter, bool nextScreenInQueue = false, bool withAnim = true)
        {
            if (!_navigationStack.Contains(screenPresenter)) return;
            RemoveScreenFromStack(screenPresenter, nextScreenInQueue);

            if (screenPresenter.OnHideTransitionAnimation == null || !withAnim)
            {
                screenPresenter.MoveToInitialPosition();
            }
            else
            {
                screenPresenter.PerformHideAnimation(delegate { screenPresenter.MoveToInitialPosition(); });
            }
        }

        private void ForceCloseCurrentScreen()
        {
            if (_navigationStack.Count == 0) return;
            var peek = _navigationStack.Last();
            peek.OnHideTransitionAnimation?.KillAnim();
            peek.OnShowTransitionAnimation?.KillAnim();
            peek.MoveToInitialPosition();
            RemoveScreenFromStack(peek);
        }

        private void ForceCloseAllScreens()
        {
            Debug.Log("Force close all screens");
            int stackCount = _navigationStack.Count;
            while (stackCount-- != 0)
            {
                ForceCloseCurrentScreen();
            }
        }
    }
}