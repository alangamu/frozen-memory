using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MainMenuUIManager : MonoBehaviour
    {
        [SerializeField]
        private LobbyManager _lobbyManager;

        private async void Start()
        {
            await _lobbyManager.SignIn();
        }

    }
}