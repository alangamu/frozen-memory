using Assets.Scripts.ScriptableObjects.Events;
using Ricimi;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LobbyWaitingRoomView : MonoBehaviour
    {
        [SerializeField]
        private StringStringIntEvent _addPlayerReadyLobbyWaitingRoomEvent;

        [SerializeField]
        private StringBoolEvent _setPlayerReadyLobbyWaitingRoomEvent;
        [SerializeField]
        private StringBoolEvent _showRemoveButtonLobbyWaitingRoomEvent;

        [SerializeField]
        private StringGameEvent _removePlayerLobbyWaitingRoomEvent;
        [SerializeField]
        private StringGameEvent _setLobbyNameLobbyWaitingRoomEvent;
        [SerializeField]
        private StringGameEvent _hostRemovePlayerFromLobby;

        [SerializeField]
        private GameEvent _readyButtonLobbyWaitingRoomPressed;
        [SerializeField]
        private GameEvent _backButtonLobbyWaitingRoomPressed;
        [SerializeField]
        private GameEvent _clearPlayersLobbyWaitingRoomEvent;

        [SerializeField]
        private BoolEvent _setReadyLobbyWaitingRoomEvent;
        [SerializeField]
        private BoolEvent _setEnableReadyButtonLobbyWaitingRoomEvent;
        [SerializeField]
        private BoolEvent _showLoadingLobbyWaitingRoomEvent;

        [SerializeField]
        private Text _lobbyNameText;
        [SerializeField]
        private Transform _playerReadyTickTransform;
        [SerializeField]
        private Transform _playersParentTranform;
        [SerializeField]
        private Text _readyButtonText;
        [SerializeField]
        private AnimatedButton _playerReadyButton;
        [SerializeField]
        private AnimatedButton _backButton;
        [SerializeField]
        private GameObject _waitingRoomPlayerUIPrefab;
        [SerializeField]
        private Transform _loadingObject;

        private Dictionary<string, WaitingRoomPlayerUI> _lobbyPlayers;

        private void ShowLoading(bool isLoading)
        {
            _loadingObject.gameObject.SetActive(isLoading);
        }

        private void ClearPlayers()
        {
            foreach (Transform item in _playersParentTranform)
            {
                Destroy(item.gameObject);
            }

            _lobbyPlayers.Clear();
        }

        private void AddPlayer(string playerName, string playerId, int avatarIndex)
        {
            if (!_lobbyPlayers.ContainsKey(playerId))
            {
                GameObject waitingRoomPlayerUIGameObject = Instantiate(_waitingRoomPlayerUIPrefab, _playersParentTranform);
                if (waitingRoomPlayerUIGameObject.TryGetComponent(out WaitingRoomPlayerUI waitingRoomPlayerUI))
                {
                    waitingRoomPlayerUI.Initialize(playerName, playerId, avatarIndex);
                    _lobbyPlayers.Add(playerId, waitingRoomPlayerUI);
                }
            }
        }

        private void RemovePlayer(string playerId)
        {
            if (_lobbyPlayers.ContainsKey(playerId))
            {
                Destroy(_lobbyPlayers[playerId].gameObject);
                _lobbyPlayers.Remove(playerId);
            }
        }

        private void SetPlayerReady(string playerId, bool isReady)
        {
            _lobbyPlayers[playerId].SetReady(isReady);
        }

        private void SetReady(bool isReady)
        {
            _playerReadyTickTransform.gameObject.SetActive(isReady);
            _readyButtonText.text = isReady ? "Cancel" : "Ready";
        }

        private void SetLobbyName(string lobbyName)
        {
            _lobbyNameText.text = lobbyName;
        }

        private void SetEnableReadyButton(bool isEnable)
        {
            if (_playerReadyButton != null)
            {
                _playerReadyButton.enabled = isEnable;
            }
        }

        private void ShowRemoveButton(string playerId, bool canShowButton)
        {
            _lobbyPlayers[playerId].ShowKickButton(canShowButton);
        }

        private void OnEnable()
        {
            _lobbyPlayers = new Dictionary<string, WaitingRoomPlayerUI>();
            _playerReadyButton.onClick.AddListener( () => 
            {
                _readyButtonLobbyWaitingRoomPressed.Raise();
            });
            _backButton.onClick.AddListener(() =>
            {
                _backButtonLobbyWaitingRoomPressed.Raise();
            });

            _setReadyLobbyWaitingRoomEvent.OnRaise += SetReady;
            _setEnableReadyButtonLobbyWaitingRoomEvent.OnRaise += SetEnableReadyButton;
            _removePlayerLobbyWaitingRoomEvent.OnRaise += RemovePlayer;
            _setLobbyNameLobbyWaitingRoomEvent.OnRaise += SetLobbyName;
            _setPlayerReadyLobbyWaitingRoomEvent.OnRaise += SetPlayerReady;
            _clearPlayersLobbyWaitingRoomEvent.OnRaise += ClearPlayers;
            _showLoadingLobbyWaitingRoomEvent.OnRaise += ShowLoading;
            _addPlayerReadyLobbyWaitingRoomEvent.OnRaise += AddPlayer;
            _showRemoveButtonLobbyWaitingRoomEvent.OnRaise += ShowRemoveButton;
            _hostRemovePlayerFromLobby.OnRaise += RemovePlayer;
        }

        private void OnDisable()
        {
            _playerReadyButton.onClick.RemoveAllListeners();
            _backButton.onClick.RemoveAllListeners();

            _setReadyLobbyWaitingRoomEvent.OnRaise -= SetReady;
            _setEnableReadyButtonLobbyWaitingRoomEvent.OnRaise -= SetEnableReadyButton;
            _removePlayerLobbyWaitingRoomEvent.OnRaise -= RemovePlayer;
            _setLobbyNameLobbyWaitingRoomEvent.OnRaise -= SetLobbyName;
            _setPlayerReadyLobbyWaitingRoomEvent.OnRaise -= SetPlayerReady;
            _clearPlayersLobbyWaitingRoomEvent.OnRaise -= ClearPlayers;
            _showLoadingLobbyWaitingRoomEvent.OnRaise -= ShowLoading;
            _addPlayerReadyLobbyWaitingRoomEvent.OnRaise -= AddPlayer;
            _showRemoveButtonLobbyWaitingRoomEvent.OnRaise -= ShowRemoveButton;
            _hostRemovePlayerFromLobby.OnRaise -= RemovePlayer;
        }
    }
}