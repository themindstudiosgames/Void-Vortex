using System;
using UniRx;
using UnityEngine;
using Zenject;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace Input
{
    public class EditorInputService : IInputService, IInitializable, IDisposable
    {
        private CompositeDisposable _compositeDisposable = new();

        public Subject<float> PitchingDelta { get; } = new();
        public Subject<Vector3> MouseDown { get; } = new();
        public Subject<Vector3> Mouse { get; } = new();

        public Subject<Vector3> MouseUp { get; } = new();
        public event Action OnMouseDown;
        public event Action OnMouseUp;


        public void Initialize()
        {
            Observable.EveryUpdate().Subscribe(delegate(long l)
            {
                PitchingDelta.OnNext(GetPinchingDelta());

                if (UnityEngine.Input.GetMouseButtonDown(0))
                {
                    MouseDown.OnNext(GetTouchScreenPoint());
                    OnMouseDown?.Invoke();
                }

                if (UnityEngine.Input.GetMouseButton(0))
                {
                    Mouse.OnNext(GetTouchScreenPoint());
                }

                if (UnityEngine.Input.GetMouseButtonUp(0))
                {
                    MouseUp.OnNext(GetTouchScreenPoint());
                    OnMouseUp?.Invoke();
                }
            }).AddTo(_compositeDisposable);
        }

        private static Vector3 GetTouchScreenPoint()
        {
            return UnityEngine.Input.mousePosition;
        }

        private static float GetPinchingDelta()
        {
            return UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = new CompositeDisposable();
        }
    }
}