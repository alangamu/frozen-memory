using System;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/String Bool GameEvent")]
    public class StringBoolEvent : ScriptableObject
    {
        public event Action<string, bool> OnRaise;

        public void Raise(string stringVariable, bool boolVariable)
        {
            OnRaise?.Invoke(stringVariable, boolVariable);
        }
    }
}