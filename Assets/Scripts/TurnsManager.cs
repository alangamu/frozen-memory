using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurnsManager : NetworkBehaviour
    {
        [SerializeField]
        private GameEvent _gameStartEvent;
        [SerializeField]
        private GameEvent _turnExpiredEvent;
        [SerializeField]
        private GameEvent _endTurnEvent;
        [SerializeField]
        private GameEvent _restartTurnEvent;
        [SerializeField]
        private IntGameEvent _beginPlayerTurnEvent;
        [SerializeField]
        private IntVariable _activePlayerId;

        private int _activePlayerIndex = 0;
        private List<ulong> _clientsIdList;

        private bool _isHost = false;

        private void OnEnable()
        {
            _isHost = NetworkManager.Singleton.IsHost;
            _restartTurnEvent.OnRaise += OnRestartTurn;
            _endTurnEvent.OnRaise += OnEndTurn;
            _gameStartEvent.OnRaise += OnGameStart;
            _turnExpiredEvent.OnRaise += OnTurnExpiredServerRpc;
        }

        private void OnDisable()
        {
            _restartTurnEvent.OnRaise -= OnRestartTurn;
            _endTurnEvent.OnRaise -= OnEndTurn;
            _gameStartEvent.OnRaise -= OnGameStart;
            _turnExpiredEvent.OnRaise -= OnTurnExpiredServerRpc;
        }

        private void OnRestartTurn()
        {
            if (_isHost)
            {
                BeginPlayerTurnClientRpc((int)_clientsIdList[_activePlayerIndex]);
            }
        }

        private async void OnEndTurn()
        {
            if (_isHost)
            {
                await Task.Delay(500);
                NextTurn();
            }
        }

        private void NextTurn()
        {
            _activePlayerIndex++;

            if (_activePlayerIndex == _clientsIdList.Count)
            {
                _activePlayerIndex = 0;
            }

            BeginPlayerTurnClientRpc((int)_clientsIdList[_activePlayerIndex]);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnTurnExpiredServerRpc()
        {
            _endTurnEvent.Raise();
        }

        private void OnGameStart()
        {
            if (_isHost)
            {
                _clientsIdList = new List<ulong>();
                foreach (var key in NetworkManager.ConnectedClients.Keys)
                {
                    _clientsIdList.Add(key);
                }

                _activePlayerIndex = Random.Range(0, _clientsIdList.Count);
                BeginPlayerTurnClientRpc((int)_clientsIdList[_activePlayerIndex]);
            }
        }

        [ClientRpc]
        private void BeginPlayerTurnClientRpc(int playerId)
        {
            _activePlayerId.SetValue(playerId);
            _beginPlayerTurnEvent.Raise(playerId);
        }
    }
}