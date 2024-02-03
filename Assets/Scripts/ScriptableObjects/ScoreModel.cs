using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu]
    public class ScoreModel : ScriptableObject
    {
        public Dictionary<string, int> Score => _score;

        // id, score
        private Dictionary<string, int> _score = new Dictionary<string, int>();

        public string GetIdPlayerHighScore(Dictionary<string, int> score)
        {
            var highScorePlayerId = score.Aggregate((max, kvp) => kvp.Value > max.Value ? kvp : max);

            return highScorePlayerId.Key;
        }

        public void AddScore(string playerId, int score)
        {
            _score.Add(playerId, score);
        }

        public void AddPlayerPoint(string playerId)
        {
            _score[playerId] = _score[playerId] + 1;
        }

        public void ClearScore()
        {
            _score.Clear();
        }
    }
}