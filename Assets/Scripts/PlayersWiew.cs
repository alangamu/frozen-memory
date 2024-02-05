using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayersWiew : MonoBehaviour
    {
        public Dictionary<string, PlayerScoreView> Players => _players;

        [SerializeField]
        private Transform _playersRoot;
        [SerializeField]
        private PlayerScoreView _playerUIPrefab;

        private Dictionary<string, PlayerScoreView> _players = new Dictionary<string, PlayerScoreView>();

        public void AddPlayer(string playerId, string playerName, int avatarIndex)
        {
            PlayerScoreView playerScoreView = Instantiate(_playerUIPrefab);
            playerScoreView.transform.SetParent(_playersRoot);
            playerScoreView.Initialize(playerId, playerName, avatarIndex);
            _players.Add(playerId, playerScoreView);
        }

        public void ClearRoot()
        {
            _players.Clear();
            foreach (Transform playerTransform in _playersRoot)
            {
                Destroy(playerTransform.gameObject);
            }
        }
    }
}