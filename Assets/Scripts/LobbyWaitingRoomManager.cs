using Assets.Scripts.ScriptableObjects;
using Ricimi;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class LobbyWaitingRoomManager : NetworkBehaviour
    {
        [SerializeField]
        private LobbyWaitingRoomView _lobbyWaitingRoomView;
        [SerializeField]
        private PlayersModel _playersController;
        [SerializeField]
        private LobbyManager _lobbyManager;
        [SerializeField]
        private StringVariable _keyStartGameVariable;
        [SerializeField]
        private StringVariable _errorMessageVariable;

        private string _playerId;
        private bool _isPlayerReady = false;
        private SceneTransition _sceneTransition;

        private async void BackToLobby()
        {
            NetworkManager.Singleton.Shutdown();

            if (_playerId.Equals(_lobbyManager.HostId))
            {
                await _lobbyManager.DeleteLobby(_lobbyManager.JoinedLobbyId);
                HostDisconnectRpc();
            }
            else
            {
                await _lobbyManager.LeaveLobby(_lobbyManager.JoinedLobbyId, _playerId);
                RemovePlayerRpc(_playerId);
            }
            _playersController.CLearPlayers();
            _sceneTransition.PerformTransition();
        }

        private void OnEnable()
        {
            _lobbyWaitingRoomView.OnBackButtonPressed += BackToLobby;
            _lobbyWaitingRoomView.OnReadyButtonPressed += PlayerReady;
        }

        private void OnDisable()
        {
            _lobbyWaitingRoomView.OnBackButtonPressed -= BackToLobby;
            _lobbyWaitingRoomView.OnReadyButtonPressed -= PlayerReady;
        }

        private void Awake()
        {
            TryGetComponent(out _sceneTransition);
        }

        public async void PlayerReady()
        {
            _isPlayerReady = !_isPlayerReady;
            _lobbyWaitingRoomView.SetReady(_isPlayerReady);
            SetPlayerReadyRpc(_playerId, _isPlayerReady);

            _lobbyWaitingRoomView.SetEnebleReadyButton(false);

            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "ready", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: _isPlayerReady ? "true" : "false")
                    }
                };

                await LobbyService.Instance.UpdatePlayerAsync(_lobbyManager.JoinedLobbyId, _playerId, options);
                _lobbyWaitingRoomView.SetEnebleReadyButton(true);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
    }

        [Rpc(SendTo.Everyone)]
        private void SetPlayerReadyRpc(string playerId, bool isReady)
        {
            Debug.Log($"SetPlayerReadyRpc playerId {playerId} isReady {isReady}");
            _playersController.SetPlayerReady(playerId, isReady);
            _lobbyWaitingRoomView.SetPlayerReady(playerId, isReady);

            if (isReady && NetworkManager.Singleton.IsHost)
            {
                CheckStartGame();
            }
        }

        [Rpc(SendTo.Everyone)]
        private void AddPlayerRpc(string playerId, string playerName, int avatarIndex)
        {
            PlayerInfo playerInfo = new PlayerInfo(avatarIndex, playerName);
            _playersController.AddPlayer(playerId, playerInfo);
            _lobbyWaitingRoomView.AddPlayer(playerName, playerId, avatarIndex);
        }

        [Rpc(SendTo.NotMe)]
        private void RemovePlayerRpc(string playerId)
        {
            _playersController.RemovePlayer(playerId);
            _lobbyWaitingRoomView.RemovePlayer(playerId);
        }

        [Rpc(SendTo.NotMe)]
        private void HostDisconnectRpc()
        {
            Debug.Log("Host Disconnected");
            _errorMessageVariable.SetValue("Host Disconnected");
        }

        private async void Start()
        {
            _lobbyWaitingRoomView.ShowLoading(true);
            _playerId = AuthenticationService.Instance.PlayerId;

            if (_playerId.Equals(_lobbyManager.HostId))
            {
                _lobbyWaitingRoomView.SetEnebleReadyButton(false);
            }

            _lobbyWaitingRoomView.SetReady(false);
            Lobby joinedLobby = await _lobbyManager.GetLobby(_lobbyManager.JoinedLobbyId);

            _lobbyWaitingRoomView.SetLobbyName(joinedLobby.Name);

            var callbacks = new LobbyEventCallbacks();
            callbacks.PlayerJoined += OnPlayerJoined;
            callbacks.PlayerLeft += OnPlayerLeft;

            try
            {
                var m_LobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(joinedLobby.Id, callbacks);
            }
            catch (LobbyServiceException ex)
            {
                switch (ex.Reason)
                {
                    case LobbyExceptionReason.AlreadySubscribedToLobby: Debug.LogWarning($"Already subscribed to lobby[{joinedLobby.Id}]. We did not need to try and subscribe again. Exception Message: {ex.Message}"); break;
                    case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); throw;
                    case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); throw;
                    default: throw;
                }
            }

            if (_playerId.Equals(_lobbyManager.HostId))
            {
                Debug.Log($"start game is host");
                await _lobbyManager.StartGame();
            }
            else
            {
                Debug.Log($"join with relay {joinedLobby.Data[_keyStartGameVariable.Value].Value}");
                bool isJoined = await _lobbyManager.StartClientWithRelay(joinedLobby.Data[_keyStartGameVariable.Value].Value);

                Debug.Log($"isJoined {isJoined}");
            }

            _playersController.CLearPlayers();
            _lobbyWaitingRoomView.ClearPlayers();

            _lobbyWaitingRoomView.ShowLoading(false);

            foreach (var player in joinedLobby.Players)
            {
                if (player.Data.TryGetValue("PlayerName", out PlayerDataObject playerName))
                {
                    if (player.Data.TryGetValue("avatarIndex", out PlayerDataObject playerAvatarIndex))
                    {
                        PlayerInfo playerInfo = new PlayerInfo(int.Parse(playerAvatarIndex.Value), playerName.Value);
                        _playersController.AddPlayer(player.Id, playerInfo);
                        _lobbyWaitingRoomView.AddPlayer(playerName.Value, player.Id, int.Parse(playerAvatarIndex.Value));
                    }
                }
                if (player.Data.TryGetValue("ready", out PlayerDataObject playerReady))
                {
                    _lobbyWaitingRoomView.SetPlayerReady(player.Id, playerReady.Value.Equals("true"));
                }
            }
        }

        private void CheckStartGame()
        {
            Debug.Log($"CanStartGame -------");
            foreach (var player in _playersController.Players)
            {
                if (!player.Value.IsReady)
                {
                    Debug.Log($"player {player.Value.PlayerName} is not ready");
                    return;
                }
            }

            LoadGameScene();
        }

        private void OnPlayerLeft(List<int> list)
        {
            Debug.Log($"Player left {list[0]}");
            //_joinedLobby = await RefreshPlayersList();
            //_lobbyWaitingRoomView.SetEnebleReadyButton(_joinedLobby.Players.Count > 1);
        }

        private void OnPlayerJoined(List<LobbyPlayerJoined> list)
        {
            _lobbyWaitingRoomView.SetEnebleReadyButton(true);
            AddPlayerRpc(list[0].Player.Id, list[0].Player.Data["PlayerName"].Value, int.Parse(list[0].Player.Data["avatarIndex"].Value));
        }

        private void LoadGameScene()
        {
            string m_SceneName = "GameScene";
            if (NetworkManager.Singleton.IsServer && !string.IsNullOrEmpty(m_SceneName))
            {
                Debug.Log("LoadGameScene");
                var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
                if (status != SceneEventProgressStatus.Started)
                {
                    Debug.LogWarning($"Failed to load {m_SceneName} " +
                          $"with a {nameof(SceneEventProgressStatus)}: {status}");
                }
            }
        }
    }
}