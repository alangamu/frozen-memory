using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(menuName = "Variables/String Variable")]
    public class StringVariable : ScriptableObject
    {
        public event Action<string> OnValueChanged;
        
        public string Value; 

        public void SetValue(string value)
        {
            Value = value;
            OnValueChanged?.Invoke(Value);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}