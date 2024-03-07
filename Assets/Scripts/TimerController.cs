using Assets.Scripts.ScriptableObjects.Events;
using Unity.Services.Authentication;
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
        private StringGameEvent _beginPlayerTurn;

        private string _playerId;

        private void OnEnable()
        {
            _gameOverEvent.OnRaise += HideTimer;
            _timerController.gameObject.SetActive(false);
            _playerId = AuthenticationService.Instance.PlayerId;
            _beginPlayerTurn.OnRaise += BeginPlayerTurn;
            _endTurnEvent.OnRaise += HideTimer;
        }

        private void OnDisable()
        {
            _gameOverEvent.OnRaise -= HideTimer;
            _beginPlayerTurn.OnRaise -= BeginPlayerTurn;
            _endTurnEvent.OnRaise -= HideTimer;
        }

        private void HideTimer()
        {
            if (_timerController.gameObject != null)
            {
                _timerController.gameObject.SetActive(false);
            }
        }

        private void BeginPlayerTurn(string localPlayerId)
        {
            if (localPlayerId.Equals(_playerId))
            {
                _timerController.gameObject.SetActive(true);
                _startCountdown.Raise();
            }
        }
    }
}