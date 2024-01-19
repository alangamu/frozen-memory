using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayerWinUI : MonoBehaviour
    {
        [SerializeField]
        private Text _playerNameText;
        [SerializeField]
        private Text _playerScoreText;
        [SerializeField]
        private Text _rankingText;

        public void Initialize(string playerName, int score, int index)
        {
            _playerNameText.text = playerName;
            _playerScoreText.text = score.ToString();
            _rankingText.text = index.ToString();
        }
    }
}