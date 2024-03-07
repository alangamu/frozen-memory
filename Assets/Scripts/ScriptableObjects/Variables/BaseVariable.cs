using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects.Variables
{
    public class BaseVariable<T> : ScriptableObject
    {
        public event Action<T> OnValueChanged;
        public T Value => _value;

        [SerializeField]
        private T _value;

        public void SetValue(T value)
        {
            _value = value;
            OnValueChanged?.Invoke(value);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}