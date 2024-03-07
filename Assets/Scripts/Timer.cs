using Assets.Scripts.ScriptableObjects.Events;
using Assets.Scripts.ScriptableObjects.Variables;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        private Transform _transformToMove;
        [SerializeField]
        private Vector2 _initialPosition;
        [SerializeField]
        private Vector2 _finalPosition;
        [SerializeField]
        private IntVariable _turnTime;
        [SerializeField]
        private GameEvent _startCountdown;
        [SerializeField]
        private GameEvent _stopCountdown;
        [SerializeField]
        private GameEvent _timeExpiredEvent;

        private float _movementTime = 0f;
        private bool _isRunning = true;

        private void OnEnable()
        {
            _startCountdown.OnRaise += StartCountdown;
            _stopCountdown.OnRaise += StopCountdown;
        }

        private void OnDisable()
        {
            _startCountdown.OnRaise -= StartCountdown;
            _stopCountdown.OnRaise -= StopCountdown;
        }

        private void StopCountdown()
        {
            _isRunning = false;
        }

        private void Awake()
        {
            _transformToMove.position = _initialPosition;
            _movementTime = _turnTime.Value;
        }

        private async void StartCountdown()
        {
            _isRunning = true;

            _transformToMove.localPosition = _initialPosition;

            float elapsedTime = 0f;

            while (elapsedTime < _movementTime)
            {
                if (!_isRunning)
                {
                    return;
                }

                _transformToMove.localPosition = Vector3.Lerp(_initialPosition, _finalPosition, elapsedTime / _movementTime);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            _transformToMove.localPosition = _finalPosition;

            if (_isRunning)
            {
                _timeExpiredEvent.Raise();
            }
        }
    }
}