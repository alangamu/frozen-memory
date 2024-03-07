using System;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/String String Int GameEvent")]
    public class StringStringIntEvent : ScriptableObject
    {
        public event Action<string, string, int> OnRaise;

        public void Raise(string stringVariable1, string stringVariable2, int intVariable)
        {
            OnRaise?.Invoke(stringVariable1, stringVariable2, intVariable);
        }
    }
}