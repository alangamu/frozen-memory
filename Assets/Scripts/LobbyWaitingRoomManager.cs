using Ricimi;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LobbyWaitingRoomManager : MonoBehaviour
    {
        [SerializeField]
        private LobbyManager _lobbyManager;
        [SerializeField]
        private Text _lobbyName;
        [SerializeField]
        private GameObject _waitingRoomPlayerUIPrefab;
        [SerializeField]
        private Transform _playersParentTranform;
        [SerializeField]
        private GameObject _playerReadyTickGameObject;
        [SerializeField]
        private Text _readyButtonText;

        private WaitingRoomPlayerUI _waitingRoomPlayer;
        private string _playerId;
        private bool _isPlayerReady = false;
        private Lobby _joinedLobby;
        private SceneTransition _sceneTransition;

        public async void BackToLobby()
        {
            await _lobbyManager.LeaveLobby(_joinedLobby.Id, _playerId);

            _sceneTransition.PerformTransition();
        }

        public async void PlayerReady()
        {
            _isPlayerReady = !_isPlayerReady;

            _playerReadyTickGameObject.SetActive(_isPlayerReady);
            _readyButtonText.text = _isPlayerReady ? "Ready" : "Cancel";

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

                _joinedLobby = await LobbyService.Instance.UpdatePlayerAsync(_lobbyManager.JoinedLobbyId, _playerId, options);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

            _waitingRoomPlayer.SetReady(_isPlayerReady);

            if (IsAllPlayersReady() && _isPlayerReady)
            {
                _sceneTransition.scene = "GameScene";
                _sceneTransition.PerformTransition();
            }
        }

        private void Awake()
        {
            TryGetComponent(out _sceneTransition);
        }

        private async void Start()
        {
            _playerId = AuthenticationService.Instance.PlayerId;
            _playerReadyTickGameObject.SetActive(false);
            _joinedLobby = await RefreshPlayersList();

            _lobbyName.text = _joinedLobby.Name;

            var callbacks = new LobbyEventCallbacks();
            callbacks.PlayerJoined += OnPlayerJoined;
            callbacks.PlayerLeft += OnPlayerLeft;
            callbacks.PlayerDataChanged += OnOtherPlayerDataChanged;

            try
            {
                var m_LobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(_joinedLobby.Id, callbacks);
            }
            catch (LobbyServiceException ex)
            {
                switch (ex.Reason)
                {
                    case LobbyExceptionReason.AlreadySubscribedToLobby: Debug.LogWarning($"Already subscribed to lobby[{_joinedLobby.Id}]. We did not need to try and subscribe again. Exception Message: {ex.Message}"); break;
                    case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); throw;
                    case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); throw;
                    default: throw;
                }
            }
        }

        private async void OnOtherPlayerDataChanged(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> obj)
        {
            _joinedLobby = await RefreshPlayersList();

            if (IsAllPlayersReady() && _isPlayerReady)
            {
                if (_playerId == _lobbyManager.HostId)
                {
                    NetworkManager.Singleton.StartHost();
                }
                else
                {
                    NetworkManager.Singleton.StartClient();
                }

                _sceneTransition.scene = "GameScene";
                _sceneTransition.PerformTransition();
            }
        }

        private bool IsAllPlayersReady()
        {
            foreach (var player in _joinedLobby.Players)
            {
                if (player.Data.TryGetValue("ready", out PlayerDataObject playerReady))
                {
                    if (playerReady.Value.Equals("false"))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private async void OnPlayerLeft(List<int> list)
        {
            _joinedLobby = await RefreshPlayersList();
        }

        private async void OnPlayerJoined(List<LobbyPlayerJoined> list)
        {
            _joinedLobby = await RefreshPlayersList();
        }

        private async Task<Lobby> RefreshPlayersList()
        {
            Lobby joinedLobby = await _lobbyManager.GetLobby(_lobbyManager.JoinedLobbyId);

            foreach (Transform item in _playersParentTranform)
            {
                Destroy(item.gameObject);
            }

            foreach (var item in joinedLobby.Players)
            {
                GameObject waitingRoomPlayerUIGameObject = Instantiate(_waitingRoomPlayerUIPrefab, _playersParentTranform);

                if (waitingRoomPlayerUIGameObject.TryGetComponent(out WaitingRoomPlayerUI waitingRoomPlayerUI))
                {
                    if (item.Data.TryGetValue("PlayerName", out PlayerDataObject playerName))
                    {
                        waitingRoomPlayerUI.Initialize(playerName.Value);
                    }
                    if (item.Data.TryGetValue("ready", out PlayerDataObject playerReady))
                    {
                        waitingRoomPlayerUI.SetReady(playerReady.Value.Equals("true"));
                    }

                    if (item.Id.Equals(_playerId))
                    {
                        _waitingRoomPlayer = waitingRoomPlayerUI;
                    }
                }
            }

            return joinedLobby;
        }
    }
}