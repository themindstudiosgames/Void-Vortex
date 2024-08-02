using UnityEngine;

namespace Screens.GameSceneHud
{
    public interface ICounterView
    {
        public Transform Target { get; }
        void HideAmount(int hiddenAmount);
        void RevealHiddenAmount(int hiddenAmount);
    }
}