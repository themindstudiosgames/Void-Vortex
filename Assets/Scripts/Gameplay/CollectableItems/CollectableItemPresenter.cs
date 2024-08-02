using System;
using Balances.CollectableItems;
using Gameplay.PlayerHole;
using Sounds;
using UnityEngine;

namespace Gameplay.CollectableItems
{
    public class CollectableItemPresenter : MonoBehaviour
    {
        [field: SerializeField] public CollectableItemType Type { private set; get; }

        private Transform _transform;
        private Rigidbody _rigidbody;
        public event Action<CollectableItemPresenter> Collected;
        public Vector3 Position => _transform.position;
        public bool IsCollected { private set; get; }
        private static int _enterLayer;
        private static int _exitLayer;
        private const string NoGroundCollisionLayerName = "NoGroundCollision";
        private const string DefaultLayerName = "Default";

        private int _switcherTriggerCount = 0;

        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _enterLayer = LayerMask.NameToLayer(NoGroundCollisionLayerName);
            _exitLayer = LayerMask.NameToLayer(DefaultLayerName);
        }

        private void SetLayer(int enterLayer)
        {
            TryRemoveKinematic();
            gameObject.layer = enterLayer;
        }

        public void Collect()
        {
            IsCollected = true;
            SoundsManager.PlaySound(AudioKey.Collect);
            Collected?.Invoke(this);
        }
        
        public void AddForce(Vector3 force)
        {
            TryRemoveKinematic();
            _rigidbody.AddForce(force, ForceMode.Acceleration);
        }

        private void OnCollisionEnter(Collision other)
        {
            TryRemoveKinematic();
        }

        private void TryRemoveKinematic() => _rigidbody.isKinematic = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out CollectableItemLayerSwitcher switcher)) return;
            if (_switcherTriggerCount == 0)
            {
                SetLayer(_enterLayer);
            }
            _switcherTriggerCount++;
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out CollectableItemLayerSwitcher switcher)) return;
            _switcherTriggerCount--;
            if (_switcherTriggerCount == 0)
            {
                SetLayer(_exitLayer);
            }
        }
    }
}