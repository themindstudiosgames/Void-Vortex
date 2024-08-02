using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public struct SerializedPair<TKey, TValue>
    {
        [field: SerializeField] public TKey Key { get; private set; }
        [field: SerializeField] public TValue Value { get; private set; }
    }

    [Serializable]
    public sealed class SerializedDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] private SerializedPair<TKey, TValue>[] data;

        private IReadOnlyDictionary<TKey, TValue> _dictionary;

        public IReadOnlyDictionary<TKey, TValue> Dictionary
        {
            get
            {
                if (_dictionary == null)
                {
                    OnAfterDeserialize();
                }

                return _dictionary;
            }
        }

        public TValue this[TKey key] => _dictionary[key];

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            try
            {
                _dictionary = data.ToDictionary(p => p.Key, p => p.Value);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}