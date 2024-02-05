using Assets.Scripts.ScriptableObjects;
using Ricimi;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LobbyWaitingRoomManager : NetworkBehaviour
    {
        [SerializeField]
        private PlayersModel _playersController;
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
        [SerializeField]
        private StringVariable _keyStartGameVariable;
        [SerializeField]
        private StringVariable _playerNameVariable;
        [SerializeField]
        private AnimatedButton _playerReadyButton;

        [SerializeField]
        private Canvas _lobbyCanvas;

        private WaitingRoomPlayerUI _waitingRoomPlayer;
        private string _playerId;
        private bool _isPlayerReady = false;
        private Lobby _joinedLobby;
        private SceneTransition _sceneTransition;

        public async void BackToLobby()
        {
            if (_playerId.Equals(_lobbyManager.HostId))
            {
                await _lobbyManager.DeleteLobby(_lobbyManager.JoinedLobbyId);
            }
            else
            {
                await _lobbyManager.LeaveLobby(_joinedLobby.Id, _playerId);

            }
            _sceneTransition.PerformTransition();
        }

        private void OnEnable()
        {
            _playerReadyButton.onClick.AddListener(PlayerReady);
        }

        private void OnDisable()
        {
            _playerReadyButton.onClick.RemoveAllListeners();
        }

        private void Awake()
        {
            TryGetComponent(out _sceneTransition);
        }

        public async void PlayerReady()
        {
            _isPlayerReady = !_isPlayerReady;

            _playerReadyTickGameObject.SetActive(_isPlayerReady);
            _readyButtonText.text = _isPlayerReady ? "Cancel" : "Ready";

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

            await CheckStartGame();
        }

        private async void Start()
        {
            _playerId = AuthenticationService.Instance.PlayerId;

            if (_playerId.Equals(_lobbyManager.HostId))
            {
                _playerReadyButton.enabled = false;
            }

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

            if (_playerId.Equals(_lobbyManager.HostId))
            {
                Debug.Log($"start game is host");
                await _lobbyManager.StartGame();
            }
            else
            {
                Debug.Log($"join with relay {_joinedLobby.Data[_keyStartGameVariable.Value].Value}");
                bool isJoined = await _lobbyManager.StartClientWithRelay(_joinedLobby.Data[_keyStartGameVariable.Value].Value);

                Debug.Log($"isJoined {isJoined}");
            }
        }

        private async void OnOtherPlayerDataChanged(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> obj)
        {
            Debug.Log($"OnOtherPlayerDataChanged -------");

            await CheckStartGame();
        }

        private async Task CheckStartGame()
        {
            _joinedLobby = await RefreshPlayersList();

            Debug.Log($"CanStartGame -------");

            foreach (var player in _joinedLobby.Players)
            {
                if (player.Data.TryGetValue("ready", out PlayerDataObject playerReady))
                {
                    Debug.Log($"player {player.Id} ready {playerReady.Value}");
                    if (playerReady.Value.Equals("false"))
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            LoadGameScene();
        }

        private async void OnPlayerLeft(List<int> list)
        {
            _joinedLobby = await RefreshPlayersList();
            _playerReadyButton.enabled = _joinedLobby.Players.Count > 1;
        }

        private async void OnPlayerJoined(List<LobbyPlayerJoined> list)
        {
            _joinedLobby = await RefreshPlayersList();
            _playerReadyButton.enabled = true;
        }

        private async Task<Lobby> RefreshPlayersList()
        {
            Lobby joinedLobby = await _lobbyManager.GetLobby(_lobbyManager.JoinedLobbyId);

            foreach (Transform item in _playersParentTranform)
            {
                Destroy(item.gameObject);
            }

            _playersController.CLearPlayers();

            foreach (var item in joinedLobby.Players)
            {
                GameObject waitingRoomPlayerUIGameObject = Instantiate(_waitingRoomPlayerUIPrefab, _playersParentTranform);

                if (waitingRoomPlayerUIGameObject.TryGetComponent(out WaitingRoomPlayerUI waitingRoomPlayerUI))
                {
                    if (item.Data.TryGetValue("PlayerName", out PlayerDataObject playerName))
                    {
                        if (item.Data.TryGetValue("avatarIndex", out PlayerDataObject playerAvatarIndex))
                        {
                            PlayerInfo playerInfo = new PlayerInfo(int.Parse(playerAvatarIndex.Value), playerName.Value);
                            _playersController.AddPlayer(item.Id, playerInfo);
                            waitingRoomPlayerUI.Initialize(playerName.Value, item.Id, int.Parse(playerAvatarIndex.Value));
                        }
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