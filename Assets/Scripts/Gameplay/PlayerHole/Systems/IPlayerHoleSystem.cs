using System;

namespace Gameplay.PlayerHole.Systems
{
    public interface IPlayerHoleSystem : IDisposable
    {
        void Initialize();
        void SetActive(bool active);
    }
}