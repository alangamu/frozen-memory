using Assets.Scripts.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SinglePlayerMovesView : MonoBehaviour
    {
        [SerializeField]
        private Text _movesText;
        [SerializeField]
        private IntGameEvent _showPlayerMovesEvent;
        [SerializeField]
        private Text _personalBestText;
        [SerializeField]
        private IntGameEvent _showPersonalBestEvent;

        private void OnEnable()
        {
            _showPlayerMovesEvent.OnRaise += PlayerMoved;
            _showPersonalBestEvent.OnRaise += ShowPersonalBest;
        }

        private void ShowPersonalBest(int personalBestMoves)
        {
            _personalBestText.text = personalBestMoves.ToString();
        }

        private void PlayerMoved(int moves)
        {
            _movesText.text = moves.ToString();
        }

        private void OnDisable()
        {
            _showPlayerMovesEvent.OnRaise -= PlayerMoved;
            _showPersonalBestEvent.OnRaise -= ShowPersonalBest;
        }
    }
}