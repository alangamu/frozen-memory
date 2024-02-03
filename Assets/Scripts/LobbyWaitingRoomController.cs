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

namespace Assets.Scripts
{
    public class LobbyWaitingRoomController : NetworkBehaviour
    {
        [SerializeField]
        private PlayersModel _playersController;
        [SerializeField]
        private LobbyManager _lobbyManager;
        [SerializeField]
        private StringVariable _playerNameVariable;
        [SerializeField]
        private GameObject _waitingRoomPlayerUIPrefab;
        [SerializeField]
        private Transform _playerReadyTickTransform;
        [SerializeField]
        private AnimatedButton _playerReadyButton;
        //[SerializeField]
        //private WaitingRoomPlayerUI[] _waitingRoomPlayersUI;
        [SerializeField]
        private Transform _playersParentTranform;

        private SceneTransition _sceneTransition;
        //private Lobby _joinedLobby;

        private NetworkVariable<int> _readyPlayersCountVariable = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private string _playerId;
        private WaitingRoomPlayerUI _waitingRoomPlayer;

        public override async void OnNetworkSpawn()
        {
            Debug.Log($"OnNetworkSpawn");
            base.OnNetworkSpawn();

            AddPlayerClientRpc(_playerId, _playerNameVariable.Value);

            var callbacks = new LobbyEventCallbacks();
            callbacks.PlayerJoined += OnPlayerJoined;
            callbacks.PlayerLeft += OnPlayerLeft;

            try
            {
                var m_LobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(_lobbyManager.JoinedLobbyId, callbacks);
            }
            catch (LobbyServiceException ex)
            {
                switch (ex.Reason)
                {
                    case LobbyExceptionReason.AlreadySubscribedToLobby: Debug.LogWarning($"Already subscribed to lobby[{_lobbyManager.JoinedLobbyId}]. We did not need to try and subscribe again. Exception Message: {ex.Message}"); break;
                    case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); throw;
                    case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); throw;
                    default: throw;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayerReadyServerRpc()
        {
            Debug.Log($"PlayerReady");
            _readyPlayersCountVariable.Value++;
            _playerReadyTickTransform.gameObject.SetActive(true);
        }

        private void OnPlayerLeft(List<int> list)
        {
            Debug.Log($"OnPlayerLeft");
            //_joinedLobby = await RefreshPlayersList();
        }

        private void OnPlayerJoined(List<LobbyPlayerJoined> list)
        {
            Debug.Log($"OnPlayerJoined");
            //_joinedLobby = await RefreshPlayersList();
            AddPlayerClientRpc(list[0].Player.Id, list[0].Player.Data["PlayerName"].Value);
        }

        private async void Start()
        {
            _playerReadyTickTransform.gameObject.SetActive(false);
            _playerReadyButton.onClick.AddListener(PlayerReadyServerRpc);
            _readyPlayersCountVariable.OnValueChanged += OnReadyPlayersCountChange; 
            
            TryGetComponent(out _sceneTransition);

            _playerId = AuthenticationService.Instance.PlayerId;

            if (_playerId.Equals(_lobbyManager.HostId))
            {
                Debug.Log($"start game is host");
                await _lobbyManager.StartGame();
            }
            else
            {
                await RefreshPlayersList();
            }
        }

        private void OnReadyPlayersCountChange(int previousValue, int newValue)
        {
            if (newValue == 0)
            {
                return;
            }
            if (NetworkManager.Singleton.IsHost)
            {
                int playersCount = NetworkManager.Singleton.ConnectedClients.Count;
                if (playersCount == newValue)
                {
                    _readyPlayersCountVariable.Value = 0;
                    //start
                    Debug.Log("Load Game");
                    LoadGameScene();
                }
            }
        }

        private async Task<Lobby> RefreshPlayersList()
        {
            //foreach (var waitingRoomPlayerUI in _waitingRoomPlayersUI)
            //{
            //    waitingRoomPlayerUI.gameObject.SetActive(false);
            //}

            //Lobby joinedLobby = await _lobbyManager.GetLobby(_lobbyManager.JoinedLobbyId);

            //for (int i = 0; i < joinedLobby.Players.Count; i++)
            //{
            //    WaitingRoomPlayerUI waitingRoomPlayerUI = _waitingRoomPlayersUI[i];
            //    Player player = joinedLobby.Players[i];
            //    waitingRoomPlayerUI.gameObject.SetActive(false);
            //    if (player.Data.TryGetValue("PlayerName", out PlayerDataObject playerName))
            //    {
            //        waitingRoomPlayerUI.Initialize(playerName.Value, player.Id);
            //    }
            //}

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
                        waitingRoomPlayerUI.Initialize(playerName.Value, item.Id);
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

        [ClientRpc]
        private void AddPlayerClientRpc(string playerId, string playerName)
        {
            Debug.Log($"AddPlayerClientRpc {playerId}, {playerName}");
            _playersController.AddPlayer(playerId, playerName);

            //GameObject waitingRoomPlayerUIGameObject = Instantiate(_waitingRoomPlayerUIPrefab, _playersParentTranform);

            //if (waitingRoomPlayerUIGameObject.TryGetComponent(out WaitingRoomPlayerUI waitingRoomPlayerUI))
            //{
            //        waitingRoomPlayerUI.Initialize(playerName);
            //        waitingRoomPlayerUI.SetReady(false);

            //    //if (playerId.Equals())
            //    //{
            //    //    _waitingRoomPlayer = waitingRoomPlayerUI;
            //    //}
            //}
            RefreshPlayersList();
        }

        [ClientRpc]
        private void PlayerReadyClientRpc(string playerId)
        {
            if (playerId.Equals(_playerId))
            {

            }
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