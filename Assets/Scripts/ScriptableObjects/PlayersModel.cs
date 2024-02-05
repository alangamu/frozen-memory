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
            _players.Add(playerId, playerInfo);
        }

        public void CLearPlayers()
        {
            _players.Clear();
        }
    }
}