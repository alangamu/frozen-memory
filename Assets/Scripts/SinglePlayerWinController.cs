using Assets.Scripts.ScriptableObjects.Events;
using Ricimi;
using UnityEngine;

namespace Assets.Scripts
{
    public class SinglePlayerWinController : MonoBehaviour
    {
        [SerializeField]
        private GameEvent _playerWinEvent;
        [SerializeField]
        private GameEvent _restartSinglePlayerEvent;
        [SerializeField]
        private GameEvent _loadMainMenuEvent;
        [SerializeField]
        private SceneTransition _sceneTransition;
        [SerializeField]
        private Transform _winWindowTransform;
        [SerializeField]
        private GameEvent _restartMovesEvent;
        [SerializeField]
        private GameEvent _initializeEvent;

        private void OnEnable()
        {
            _winWindowTransform.gameObject.SetActive(false);
            _playerWinEvent.OnRaise += PlayerWin;
            _restartSinglePlayerEvent.OnRaise += RestartSinglePlayer;
            _loadMainMenuEvent.OnRaise += LoadMainMenu;
        }

        private void OnDisable()
        {
            _playerWinEvent.OnRaise -= PlayerWin;
            _restartSinglePlayerEvent.OnRaise -= RestartSinglePlayer;
            _loadMainMenuEvent.OnRaise -= LoadMainMenu;
        }

        private void LoadMainMenu()
        {
            _sceneTransition.PerformTransition();
        }

        private void RestartSinglePlayer()
        {
            _winWindowTransform.gameObject.SetActive(false);
            _initializeEvent.Raise();
        }

        private void PlayerWin()
        {
            _winWindowTransform.gameObject.SetActive(true);
        }
    }
}