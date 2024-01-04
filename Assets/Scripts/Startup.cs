using UnityEngine;

namespace Assets.Scripts
{
    public class Startup : MonoBehaviour
    {
        [SerializeField]
        private LobbyManager _lobbyManager;
        [SerializeField]
        private BoolVariable _isStartupLoaded;

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