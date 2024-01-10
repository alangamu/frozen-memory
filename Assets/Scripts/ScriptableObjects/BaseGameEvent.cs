﻿using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class BaseGameEvent<T> : ScriptableObject
    {
        public event Action<T> OnRaise;

        public virtual void Raise(T type)
        {
            OnRaise?.Invoke(type);
        }
    }

}