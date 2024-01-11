using Ricimi;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LobbyItemUI : MonoBehaviour
    {
        [SerializeField]
        private Text _lobbyNameText;
        [SerializeField]
        private LobbyManager _lobbyManager;

        private string _lobbyId;

        public void Initialize(string lobbyName, string lobbyId)
        {
            _lobbyNameText.text = lobbyName;
            _lobbyId = lobbyId;
        }

        public async void JoinLobby()
        {
            JoinLobbyByIdOptions options = new()
            {
                Player = _lobbyManager.CreatePlayer()
            };
            await _lobbyManager.JoinLobby(_lobbyId, options);
            if (TryGetComponent(out SceneTransition sceneTransition))
            {
                sceneTransition.PerformTransition();
            }
        }
    }
}