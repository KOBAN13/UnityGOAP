using System;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers.SerializedDictionary
{
    [Serializable]
    public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [HideInInspector]
        [SerializeField] private List<TKey> _keyData = new();

        [HideInInspector]
        [SerializeField] private List<TValue> _valueData = new();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();

            for (var i = 0; i < _keyData.Count && i < _valueData.Count; i++)
            {
                this[_keyData[i]] = _valueData[i];
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _keyData.Clear();
            _valueData.Clear();

            foreach (var item in this)
            {
                _keyData.Add(item.Key);
                _valueData.Add(item.Value);
            }
        }
    }
}