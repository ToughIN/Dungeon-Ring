using System;
using System.Collections.Generic;
using ToufFrame;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace ToufFrame
{
    [Serializable]
    public class TriggerVariable<T>
    {
        [SerializeField]
        private T _value;
        public event Action<T> OnValueChange;

        public TriggerVariable(T initialValue)
        {
            _value = initialValue;
        }

        public T Value
        {
            get { return _value; }
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    OnValueChange?.Invoke(_value);
                }
            }
        }
    }


}

