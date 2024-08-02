using UnityEngine;

namespace Gameplay.Level
{
    public class MovementBorder : MonoBehaviour
    {
        [SerializeField] private Vector2 size;
        private Transform _transform;
        private float _minX;
        private float _maxX;
        private float _minZ;
        private float _maxZ;
        private void Awake()
        {
            _transform = transform;
            Vector3 center = _transform.position;
            Vector2 halfSize = size / 2;
            _minX = center.x - halfSize.x;
            _maxX = center.x + halfSize.x;
            _minZ = center.z - halfSize.y;
            _maxZ = center.z + halfSize.y;
        }

        public Vector3 ClampPosition(Vector3 position)
        {
            return new Vector3()
            {
                x = position.x < _minX
                    ? _minX
                    : position.x > _maxX
                        ? _maxX
                        : position.x,
                y = position.y,
                z = position.z < _minZ
                    ? _minZ
                    : position.z > _maxZ
                        ? _maxZ
                        : position.z
            };
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 10, size.y));
        }
    }
}