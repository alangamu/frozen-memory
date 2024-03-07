using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.ScriptableObjects.Events;
using Assets.Scripts.ScriptableObjects.Variables;
using Ricimi;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private StringBoolEvent _setPlayerReadyLobbyWaitingRoomEvent;

        [SerializeField]
        private StringGameEvent _removePlayerLobbyWaitingRoomEvent;
        [SerializeField]
        private StringGameEvent _hostRemovePlayerFromLobby;
        

        [SerializeField]
        private BoolEvent _setReadyLobbyWaitingRoomEvent;
        [SerializeField]
        private BoolEvent _setEnableReadyButtonLobbyWaitingRoomEvent;

        [SerializeField]
        private GameEvent _readyButtonLobbyWaitingRoomPressed;
        [SerializeField]
        private GameEvent _backButtonLobbyWaitingRoomPressed;

        [SerializeField]
        private PlayersModel _playersController;
        [SerializeField]
        private LobbyManager _lobbyManager;
        [SerializeField]
        private StringVariable _errorMessageVariable;

        private string _playerId;
        private bool _isPlayerReady = false;
        private SceneTransition _sceneTransition;

        private async void BackToLobby()
        {
            if (_playerId.Equals(_lobbyManager.HostId))
            {
                await _lobbyManager.DeleteLobby(_lobbyManager.JoinedLobbyId);
                HostDisconnectRpc();
            }
            else
            {
                await RemovePlayer(_playerId);
            }

            NetworkManager.Singleton.Shutdown();
            _playersController.CLearPlayers();
            _sceneTransition.PerformTransition();
        }

        private void OnEnable()
        {
            _hostRemovePlayerFromLobby.OnRaise += RemovePlayerFromLobby;
            _readyButtonLobbyWaitingRoomPressed.OnRaise += PlayerReady;
            _backButtonLobbyWaitingRoomPressed.OnRaise += BackToLobby;
        }

        private void OnDisable()
        {
            _hostRemovePlayerFromLobby.OnRaise -= RemovePlayerFromLobby;
            _readyButtonLobbyWaitingRoomPressed.OnRaise -= PlayerReady;
            _backButtonLobbyWaitingRoomPressed.OnRaise -= BackToLobby;
        }

        private async void RemovePlayerFromLobby(string removedPlayerId)
        {
            await RemovePlayer(removedPlayerId);
        }

        private async Task RemovePlayer(string removedPlayerId)
        {
            await _lobbyManager.LeaveLobby(_lobbyManager.JoinedLobbyId, removedPlayerId);
            RemovePlayerRpc(removedPlayerId);
        }

        private void Awake()
        {
            TryGetComponent(out _sceneTransition);
            _playerId = AuthenticationService.Instance.PlayerId;
        }

        public async void PlayerReady()
        {
            _isPlayerReady = !_isPlayerReady;
            _setReadyLobbyWaitingRoomEvent.Raise(_isPlayerReady);
            SetPlayerReadyRpc(_playerId, _isPlayerReady);

            _setEnableReadyButtonLobbyWaitingRoomEvent.Raise(false);

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
                _setEnableReadyButtonLobbyWaitingRoomEvent.Raise(true);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        [Rpc(SendTo.Everyone)]
        private void SetPlayerReadyRpc(string playerId, bool isReady)
        {
            _playersController.SetPlayerReady(playerId, isReady);

            _setPlayerReadyLobbyWaitingRoomEvent.Raise(playerId, isReady);

            if (isReady && NetworkManager.Singleton.IsHost)
            {
                CheckStartGame();
            }
        }

        [Rpc(SendTo.NotMe)]
        private void RemovePlayerRpc(string playerId)
        {
            if (playerId.Equals(_playerId))
            {
                _errorMessageVariable.SetValue("Host removed you from the lobby");
            }
            else
            {
                _playersController.RemovePlayer(playerId);
                _removePlayerLobbyWaitingRoomEvent.Raise(playerId);
            }
        }

        [Rpc(SendTo.NotMe)]
        private void HostDisconnectRpc()
        {
            _errorMessageVariable.SetValue("Host Disconnected");
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

        private void LoadGameScene()
        {
            string m_SceneName = "GameScene";
            if (NetworkManager.Singleton.IsServer && !string.IsNullOrEmpty(m_SceneName))
            {
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