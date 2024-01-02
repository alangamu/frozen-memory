using System;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(menuName = "Game Event")]
    public class GameEvent : ScriptableObject
    {
        public event Action OnRaise;

        public void Raise()
        {
            OnRaise?.Invoke();
        }
    }
}