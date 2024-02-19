using Ricimi;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LobbyWaitingRoomView : MonoBehaviour
    {
        public event Action OnReadyButtonPressed;
        public event Action OnBackButtonPressed;

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

        private Dictionary<string, WaitingRoomPlayerUI> _lobbyPlayers;

        public void ClearPlayers()
        {
            foreach (Transform item in _playersParentTranform)
            {
                Destroy(item.gameObject);
            }

            _lobbyPlayers.Clear();
        }

        public void AddPlayer(string playerName, string playerId, int avatarIndex)
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

        public void RemovePlayer(string playerId)
        {
            if (_lobbyPlayers.ContainsKey(playerId))
            {
                Destroy(_lobbyPlayers[playerId].gameObject);
                _lobbyPlayers.Remove(playerId);
            }
        }

        public void SetPlayerReady(string playerId, bool isReady)
        {
            _lobbyPlayers[playerId].SetReady(isReady);
        }

        public void SetReady(bool isReady)
        {
            _playerReadyTickTransform.gameObject.SetActive(isReady);
            _readyButtonText.text = isReady ? "Cancel" : "Ready";
        }

        public void SetLobbyName(string lobbyName)
        {
            _lobbyNameText.text = lobbyName;
        }

        public void SetEnebleReadyButton(bool isEnable)
        {
            _playerReadyButton.enabled = isEnable;
        }

        private void OnEnable()
        {
            _lobbyPlayers = new Dictionary<string, WaitingRoomPlayerUI>();
            _playerReadyButton.onClick.AddListener( () => 
            {
                OnReadyButtonPressed?.Invoke();    
            });
            _backButton.onClick.AddListener(() =>
            {
                OnBackButtonPressed?.Invoke();
            });
        }

        private void OnDisable()
        {
            _playerReadyButton.onClick.RemoveAllListeners();
            _backButton.onClick.RemoveAllListeners();
        }
    }
}