using System;
using UniRx;
using UnityEngine;
using Zenject;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace Input
{
    using Input = UnityEngine.Input;
    public class MobileInputService : IInputService, IInitializable, IDisposable
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

                if (Input.touchCount != 1) return;
                
                if (Input.GetMouseButtonDown(0))
                {
                    MouseDown.OnNext(GetTouchScreenPoint());
                    OnMouseDown?.Invoke();
                }

                if (Input.GetMouseButton(0))
                {
                    Mouse.OnNext(GetTouchScreenPoint());
                }
                
                if (Input.GetMouseButtonUp(0))
                {
                    MouseUp.OnNext(GetTouchScreenPoint());
                    OnMouseUp?.Invoke();
                }
            }).AddTo(_compositeDisposable);
        }

        private static Vector3 GetTouchScreenPoint()
        {
            return Input.mousePosition;
        }

        private static float GetPinchingDelta()
        {
            if (Input.touchCount != 2) return Input.GetAxis("Mouse ScrollWheel");
            Touch touchA = Input.GetTouch(0);
            Touch touchB = Input.GetTouch(1);
            Vector2 touchAPrevPosition = touchA.position - touchA.deltaPosition;
            Vector2 touchBPrevPosition = touchB.position - touchB.deltaPosition;
            float delta = (touchA.position - touchB.position).magnitude -
                          (touchAPrevPosition - touchBPrevPosition).magnitude;
            return delta / 750f;
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = new CompositeDisposable();
        }
    }
}