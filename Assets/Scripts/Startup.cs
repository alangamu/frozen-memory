using Assets.Scripts.ScriptableObjects.Events;
using Assets.Scripts.ScriptableObjects.Variables;
using UnityEngine;

namespace Assets.Scripts
{
    public class Startup : MonoBehaviour
    {
        [SerializeField]
        private LobbyManager _lobbyManager;
        [SerializeField]
        private BoolVariable _isStartupLoaded;
        [SerializeField]
        private GameEvent _quitGameEvent;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _quitGameEvent.OnRaise += QuitGame;
        }

        private void OnDisable()
        {
            _quitGameEvent.OnRaise -= QuitGame;
        }

        private void QuitGame()
        {
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call<bool>("moveTaskToBack", true);
            //Application.Quit();
        }

        private async void Start()
        {
            await _lobbyManager.SignIn();
            _isStartupLoaded.SetValue(true);
        }

        private void OnApplicationQuit()
        {
            _isStartupLoaded.SetValue(false);
        }
    }
}