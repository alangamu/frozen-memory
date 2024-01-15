using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class TimerController : MonoBehaviour
    {
        [SerializeField]
        private Transform _timerController;
        [SerializeField]
        private GameEvent _startCountdown;
        [SerializeField]
        private GameEvent _gameOverEvent;
        [SerializeField]
        private GameEvent _endTurnEvent;
        [SerializeField]
        private IntGameEvent _beginPlayerTurn;

        private int _localPlayerId;

        private void OnEnable()
        {
            _gameOverEvent.OnRaise += HideTimer;
            _timerController.gameObject.SetActive(false);
            _localPlayerId = (int)NetworkManager.Singleton.LocalClientId;
            _beginPlayerTurn.OnRaise += BeginPlayerTurn;
            _endTurnEvent.OnRaise += HideTimer;
        }

        private void OnDisable()
        {
            _gameOverEvent.OnRaise -= HideTimer;
            _beginPlayerTurn.OnRaise -= BeginPlayerTurn;
            _endTurnEvent.OnRaise += HideTimer;
        }

        private void HideTimer()
        {
            _timerController.gameObject.SetActive(false);
        }

        private void BeginPlayerTurn(int localPlayerId)
        {
            if (localPlayerId == _localPlayerId)
            {
                _timerController.gameObject.SetActive(true);
                _startCountdown.Raise();
            }
        }
    }
}