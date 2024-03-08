using Assets.Scripts.ScriptableObjects.Variables;
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
        private Text _lobbyNumMaxPlayersText;
        [SerializeField]
        private LobbyManager _lobbyManager;
        [SerializeField]
        private StringVariable _errorMessageVariable;

        private string _lobbyId;

        public void Initialize(string lobbyName, string lobbyId, int numPlayers, int maxPlayers)
        {
            _lobbyNameText.text = lobbyName;
            _lobbyId = lobbyId;
            _lobbyNumMaxPlayersText.text = $"{numPlayers} / {maxPlayers}";
        }

        public async void JoinLobby()
        {
            JoinLobbyByIdOptions options = new()
            {
                Player = _lobbyManager.CreatePlayer()
            };
            Lobby lobby = await _lobbyManager.JoinLobby(_lobbyId, options);
            if (lobby == null)
            {
                _errorMessageVariable.SetValue("Can't connect to the lobby");
                return;
            }

            if (TryGetComponent(out SceneTransition sceneTransition))
            {
                sceneTransition.PerformTransition();
            }
        }
    }
}