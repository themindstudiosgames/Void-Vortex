using UniRx;
using UnityEngine;

namespace Input.Game
{
    public class InputHandler : MonoBehaviour, IMovementInputHandler
    {
        [SerializeField] private DynamicOnScreenStick dynamicOnScreenStick;
        private Vector2 _direction;
        public Vector2 MovementDirection => _direction;

        private const string HorizontalKey = "Horizontal";
        private const string VerticalKey = "Vertical";
        private void Awake()
        {
            Observable.EveryUpdate().Subscribe(_ =>
            {
                Vector2 keyboardInput = Vector2.ClampMagnitude(new Vector2(UnityEngine.Input.GetAxis(HorizontalKey),
                    UnityEngine.Input.GetAxis(VerticalKey)),1);
                Vector2 touchscreenInput = dynamicOnScreenStick.MovementDirection;
                _direction = keyboardInput == Vector2.zero ? touchscreenInput : keyboardInput;
            }).AddTo(this);
        }
    }
}