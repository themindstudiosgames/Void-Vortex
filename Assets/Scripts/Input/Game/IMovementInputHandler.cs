using UnityEngine;

namespace Input.Game
{
    public interface IMovementInputHandler
    {
        Vector2 MovementDirection { get; }
    }
}