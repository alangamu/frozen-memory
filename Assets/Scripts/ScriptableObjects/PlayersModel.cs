using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu]
    public class PlayersModel : ScriptableObject
    {
        public Dictionary<string, PlayerInfo> Players => _players;

        // id, name
        private Dictionary<string, PlayerInfo> _players = new Dictionary<string, PlayerInfo>();

        public void AddPlayer(string playerId, PlayerInfo playerInfo)
        {
            if (!_players.ContainsKey(playerId))
            {
                _players.Add(playerId, playerInfo);
            }
        }

        public void RemovePlayer(string playerId)
        {
            if (_players.ContainsKey(playerId))
            {
                _players.Remove(playerId); 
            }
        }

        public void CLearPlayers()
        {
            _players.Clear();
        }

        public void SetPlayerReady(string playerId, bool isReady)
        {
            if (_players.ContainsKey(playerId))
            {
                _players[playerId].SetIsReady(isReady);
            }
        }
    }
}