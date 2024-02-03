using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayerWinView : MonoBehaviour
    {
        public string PlayerId => _playerId;
        public bool IsReady => _isReady;

        [SerializeField]
        private Text _playerNameText;
        [SerializeField]
        private Text _playerScoreText;
        [SerializeField]
        private Text _rankingText;
        [SerializeField]
        private Transform _readyTickTransform;

        private string _playerId;
        private bool _isReady;

        public void Initialize(string playerName, int score, int index, string playerId)
        {
            _playerId = playerId;
            _readyTickTransform.gameObject.SetActive(false);
            _playerNameText.text = playerName;
            _playerScoreText.text = score.ToString();
            _rankingText.text = index.ToString();
            _isReady = false;
        }

        public void SetReady(bool ready)
        {
            _readyTickTransform.gameObject.SetActive(ready);
            _isReady = ready;
        }
    }
}