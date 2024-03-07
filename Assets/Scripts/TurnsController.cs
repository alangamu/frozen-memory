using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.ScriptableObjects.Events;
using Assets.Scripts.ScriptableObjects.Variables;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurnsController : NetworkBehaviour
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
        private StringGameEvent _beginPlayerTurnEvent;
        [SerializeField]
        private StringVariable _activePlayerId;
        [SerializeField]
        private PlayersModel _playersManager;
        [SerializeField]
        private PlayersWiew _playersView;

        private int _activePlayerIndex = 0;
        private List<string> _clientsIdList = new List<string>();

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
                BeginPlayerTurnClientRpc(_clientsIdList[_activePlayerIndex]);
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

            BeginPlayerTurnClientRpc(_clientsIdList[_activePlayerIndex]);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnTurnExpiredServerRpc()
        {
            EndTurnClientRpc();
        }

        private void OnGameStart()
        {
            if (_isHost)
            {
                _clientsIdList.Clear();
                foreach (var player in _playersManager.Players)
                {
                    _clientsIdList.Add(player.Key);
                }

                _activePlayerIndex = Random.Range(0, _clientsIdList.Count);

                BeginPlayerTurnClientRpc(_clientsIdList[_activePlayerIndex]);
            }
        }

        [ClientRpc]
        private void EndTurnClientRpc() 
        {
            _activePlayerId.SetValue(string.Empty);
            _endTurnEvent.Raise();
        }

        [ClientRpc]
        private void BeginPlayerTurnClientRpc(string playerId)
        {
            _activePlayerId.SetValue(playerId);
            _beginPlayerTurnEvent.Raise(playerId);

            foreach (var playerScoreView in _playersView.Players)
            {
                playerScoreView.Value.SetActivePlayer(playerScoreView.Key.Equals(playerId));
            }
        }
    }
}