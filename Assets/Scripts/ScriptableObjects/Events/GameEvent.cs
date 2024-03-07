using System;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Game Event")]
    public class GameEvent : ScriptableObject
    {
        public event Action OnRaise;

        public void Raise()
        {
            //Debug.Log($"Event Raised {name}");
            OnRaise?.Invoke();
        }
    }
}