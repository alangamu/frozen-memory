using System.Collections.Generic;
using System.Linq.Expressions;
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
using System;

namespace Assets.Scripts
{
    [CreateAssetMenu]
    public class LobbyManager : ScriptableObject
    {
        public event Action<string> OnGameStart;
        public string JoinedLobbyId => _joinedLobbyId;
        public string HostId => _hostId;

        [SerializeField]
        private StringVariable _playerNameVariable;
        [SerializeField]
        private StringVariable _keyStartGameVariable;
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

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
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
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        }

        public async void CreateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                Debug.Log($"Relay join code {joinCode}");
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void JoinRelay(string joinCode)
        {
            try
            {
                Debug.Log($"Join relay {joinCode}");
                await RelayService.Instance.JoinAllocationAsync(joinCode);
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
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
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async Task StartGame()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log("Start Game Host");

                string relayCode = await StartHostWithRelay();

                //await Lobbies.Instance.UpdateLobbyAsync(_joinedLobbyId, new UpdateLobbyOptions { 
                //    Data = new Dictionary<string, DataObject>
                //    {
                //        { _keyStartGameVariable.Value, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                //    }
                //});

                OnGameStart?.Invoke(relayCode);
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