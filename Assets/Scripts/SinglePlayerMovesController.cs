using Assets.Scripts.ScriptableObjects.Events;
using Assets.Scripts.ScriptableObjects.Variables;
using UnityEngine;

namespace Assets.Scripts
{
    public class SinglePlayerMovesController : MonoBehaviour
    {
        [SerializeField]
        private IntVariable _playerMoves;
        [SerializeField]
        private IntVariable _personalBestMoves;
        [SerializeField]
        private IntGameEvent _showPlayerMovesEvent;
        [SerializeField]
        private IntGameEvent _showPersonalBestEvent;
        [SerializeField]
        private GameEvent _playerMovedEvent;
        [SerializeField]
        private GameEvent _initializeEvent;
        [SerializeField]
        private GameEvent _playerWinEvent;
        [SerializeField]
        private Transform _personalBestTrasnform;

        private void OnEnable()
        {
            _playerMovedEvent.OnRaise += PlayerMoved;
            _initializeEvent.OnRaise += Initialize;
            _playerWinEvent.OnRaise += PlayerWin;
        }

        private void OnDisable()
        {
            _playerMovedEvent.OnRaise -= PlayerMoved;
            _initializeEvent.OnRaise -= Initialize;
            _playerWinEvent.OnRaise -= PlayerWin;
        }

        private void PlayerWin()
        {
            if (_personalBestMoves.Value > _playerMoves.Value || _personalBestMoves.Value == 0)
            {
                _personalBestMoves.SetValue(_playerMoves.Value);
            }
        }

        private void PlayerMoved()
        {
            int moves = _playerMoves.Value;
            moves++;
            _playerMoves.SetValue(moves);
            _showPlayerMovesEvent.Raise(moves);
        }

        private void Initialize()
        {
            _playerMoves.SetValue(0);
            _showPersonalBestEvent.Raise(_personalBestMoves.Value);
            _showPlayerMovesEvent.Raise(_playerMoves.Value);
            _personalBestTrasnform.gameObject.SetActive(_personalBestMoves.Value > 0);
        }
    }
}