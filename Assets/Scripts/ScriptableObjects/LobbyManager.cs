using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
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
        private IntVariable _playerAvatarIndexVariable;
        [SerializeField]
        private StringVariable _keyStartGameVariable;
        [SerializeField]
        private BoolVariable _isStartupLoaded;
        [SerializeField]
        private IntVariable _lobbyHeartbeatSeconds;

        private string _joinedLobbyId;
        private string _hostId;

        private bool _keepLobbyAlive = false;

        public async Task SignIn()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () => {
                Debug.Log($"Signed in {AuthenticationService.Instance.PlayerId}");
            };

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        public void StopHeartbeat()
        {
            _keepLobbyAlive = false;
        }

        public async Task<string> StartHostWithRelay(int maxConnections = 3)
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            return NetworkManager.Singleton.StartHost() ? joinCode : null;
        }

        public async Task<bool> StartClientWithRelay(string joinCode)
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        }

        public async Task CreateLobby(string lobbyName)
        {
            CreateLobbyOptions options = new CreateLobbyOptions();

            options.Player = CreatePlayer();
            options.Data = new Dictionary<string, DataObject>
            {
                { _keyStartGameVariable.Value, new DataObject(DataObject.VisibilityOptions.Member, "0") }
            };

            int maxPlayers = 4;
            try
            {
                Lobby _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
                _joinedLobbyId = _joinedLobby.Id;
                _hostId = _joinedLobby.HostId;
                _keepLobbyAlive = true;
                Heartbeat(_joinedLobbyId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async Task StartGame()
        {
            string relayCode = await StartHostWithRelay();

            await Lobbies.Instance.UpdateLobbyAsync(_joinedLobbyId, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { _keyStartGameVariable.Value, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                }
            });
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

        public async Task<Lobby> JoinLobby(string lobbyId, JoinLobbyByIdOptions options)
        {
            try
            {
                _joinedLobbyId = lobbyId;
                Lobby _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
                //_joinedLobbyId = _joinedLobby.Id;
                _hostId = string.Empty;
                return _joinedLobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
                return null;
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
                                        //TODO: change both for StringVariables
                                        "PlayerName", new PlayerDataObject(
                                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                                            value: _playerNameVariable.Value)
                                    },
                                    {
                                        "avatarIndex", new PlayerDataObject(
                                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                                            value: _playerAvatarIndexVariable.Value.ToString())
                                    },
                                    {
                                        "ready", new PlayerDataObject(
                                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                                            value: "false")
                                    }
                                });
            return player;
        }

        private async void Heartbeat(string lobbyId)
        {
            while (_keepLobbyAlive)
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                await Task.Delay(1000 * _lobbyHeartbeatSeconds.Value);
            }
        }
    }
}