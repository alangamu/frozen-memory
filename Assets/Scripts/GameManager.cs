using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField]
        private GameEvent _nextTurnEvent;
        [SerializeField]
        private GameEvent _gameStartEvent;
        [SerializeField]
        private IntVariable _activePlayerId;

        private NetworkVariable<int> _activeClientId = new NetworkVariable<int>(9);
        private NetworkVariable<int> _activePlayerIndex = new NetworkVariable<int>(9);

        private List<ulong> _clientsIdList;

        private void OnEnable()
        {
            _nextTurnEvent.OnRaise += NextTurn;
            _gameStartEvent.OnRaise += GameStart;
            _activeClientId.OnValueChanged += NextPlayerTurn;
        }

        private void OnDisable()
        {
            _nextTurnEvent.OnRaise -= NextTurn;
            _gameStartEvent.OnRaise -= GameStart;
            _activeClientId.OnValueChanged -= NextPlayerTurn;
        }

        private void NextPlayerTurn(int previousValue, int newValue)
        {
            _activePlayerId.SetValue(_activeClientId.Value);
        }

        private void GameStart()
        {
            _clientsIdList = new List<ulong>();

            if (NetworkManager.Singleton.IsHost)
            {
                foreach (var key in NetworkManager.ConnectedClients.Keys)
                {
                    _clientsIdList.Add(key);
                }

                _activePlayerIndex.Value = (int)_clientsIdList[Random.Range(0, _clientsIdList.Count)];
                _activeClientId.Value = _activePlayerIndex.Value;
            }
        }

        private void NextTurn()
        {
            _activePlayerIndex.Value++;
            
            if (_activePlayerIndex.Value == _clientsIdList.Count)
            {
                _activePlayerIndex.Value = 0;
            }

            _activeClientId.Value = (int)_clientsIdList[_activePlayerIndex.Value];
        }
    }
}