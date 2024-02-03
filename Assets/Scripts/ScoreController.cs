using Assets.Scripts.ScriptableObjects;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class ScoreController : NetworkBehaviour
    {
        [SerializeField]
        private PlayersWiew _playersView;
        [SerializeField]
        private GameEvent _initializeEvent;
        [SerializeField]
        private StringGameEvent _playerScored;
        [SerializeField]
        private ScoreModel _scoreManager;
        [SerializeField]
        private PlayersModel _playersManager;

        private void OnEnable()
        {
            _initializeEvent.OnRaise += Initialize;
            _playerScored.OnRaise += PlayerScored;
        }

        private void OnDisable()
        {
            _initializeEvent.OnRaise -= Initialize;
            _playerScored.OnRaise -= PlayerScored;
        }

        private void Initialize()
        {
            _playersView.ClearRoot();
            _scoreManager.ClearScore();

            if (NetworkManager.Singleton.IsHost)
            {
                Dictionary<string, string> players = _playersManager.Players;

                foreach (var player in players)
                {
                    SetupPlayerScoreUIClientRpc(player.Key, player.Value);
                }
            }
        }

        private void PlayerScored(string playerId)
        {
            _scoreManager.AddPlayerPoint(playerId);

            PlayerScoreView playerScoreView = _playersView.Players[playerId];
            if (playerScoreView != null)
            {
                playerScoreView.AddPoint();
            }
        }

        [ClientRpc]
        private void SetupPlayerScoreUIClientRpc(string playerId, string playerName)
        {
            _playersView.AddPlayer(playerId, playerName);
            _scoreManager.AddScore(playerId, 0);
        }
    }
}