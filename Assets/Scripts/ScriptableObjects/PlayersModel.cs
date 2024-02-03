using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu]
    public class PlayersModel : ScriptableObject
    {
        public Dictionary<string, string> Players => _players;

        // id, name
        private Dictionary<string, string> _players = new Dictionary<string, string>();

        public void AddPlayer(string playerId, string playerName)
        {
            _players.Add(playerId, playerName);
        }

        public void CLearPlayers()
        {
            _players.Clear();
        }
    }
}