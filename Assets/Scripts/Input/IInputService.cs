using System;
using UniRx;
using UnityEngine;

namespace Input
{
    public interface IInputService
    {
        Subject<float> PitchingDelta { get; }
        Subject<Vector3> MouseDown { get; }
        Subject<Vector3> Mouse { get; }
        Subject<Vector3> MouseUp { get; }

        event Action OnMouseDown;
        event Action OnMouseUp;
    }
}