using System;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu]
    public class IntVariable : ScriptableObject
    {
        public event Action<int> OnValueChanged;
        public int Value => _value;

        private int _value;

        public void SetValue(int value)
        {
            _value = value;
            OnValueChanged?.Invoke(value);
        }
    }
}