using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu]
    public class LobbyManager : ScriptableObject
    {
        public string JoinedLobbyId => _joinedLobbyId;
        public string HostId => _hostId;

        [SerializeField]
        private StringVariable _playerNameVariable;
        [SerializeField]
        private BoolVariable _isStartupLoaded;

        private string _joinedLobbyId;
        private string _hostId;

        public async Task SignIn()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () => {
                Debug.Log($"Signed in {AuthenticationService.Instance.PlayerId}");
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async Task CreateLobby(string lobbyName, CreateLobbyOptions createLobbyOptions)
        {
            int maxPlayers = 4;
            try
            {
                Lobby _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
                _joinedLobbyId = _joinedLobby.Id;
                _hostId = _joinedLobby.HostId;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async Task<List<Lobby>> GetLobbyList()
        {
            try
            {
                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

                return queryResponse.Results;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                return null;
            }
        }

        public async Task LeaveLobby(string lobbyId, string playerId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async Task DeleteLobby(string lobbyId)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async Task JoinLobby(string lobbyId, JoinLobbyByIdOptions options)
        {
            try
            {
                Lobby _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
                _joinedLobbyId = _joinedLobby.Id;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async Task<Lobby> GetLobby(string lobbyId)
        {
            try
            {
                return await LobbyService.Instance.GetLobbyAsync(lobbyId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                return null;
            }
        }

        public Player CreatePlayer()
        {
            Player player =  new Player(
                                id: AuthenticationService.Instance.PlayerId,
                                data: new Dictionary<string, PlayerDataObject>()
                                {
                                    {
                                        "PlayerName", new PlayerDataObject(
                                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                                            value: _playerNameVariable.Value)
                                    },
                                    {
                                        "ready", new PlayerDataObject(
                                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                                            value: "false")
                                    }
                                });
            return player;
        }
    }
}