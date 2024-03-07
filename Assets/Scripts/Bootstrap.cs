#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Assets.Scripts.ScriptableObjects.Variables;
#endif

namespace Assets.Scripts
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class Bootstrap
    {
#if UNITY_EDITOR
        static Bootstrap()
        {
            EditorApplication.playModeStateChanged += EstadoCambioModoJuego;
        }

        private static void EstadoCambioModoJuego(PlayModeStateChange estado)
        {
            if (estado == PlayModeStateChange.EnteredEditMode)
            {
                Resources.Load<BoolVariable>("Variables/IsStartupLoaded").SetValue(false);
            }
        }
#endif
    }
}



