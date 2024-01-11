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
        private GameEvent _endTurn;

        private float _movementTime = 0f;

        private void OnEnable()
        {
            _transformToMove.gameObject.SetActive(false);
            _startCountdown.OnRaise += StartCountdown;
        }

        private void OnDisable()
        {
            _startCountdown.OnRaise -= StartCountdown;
        }

        private void Awake()
        {
            _movementTime = _turnTime.Value;
        }

        private async void StartCountdown()
        {
            _transformToMove.gameObject.SetActive(true);
            _transformToMove.position = _initialPosition;

            float elapsedTime = 0f;

            while (elapsedTime < _movementTime)
            {
                _transformToMove.position = Vector3.Lerp(_initialPosition, _finalPosition, elapsedTime / _movementTime);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            _transformToMove.position = _finalPosition;

            Debug.Log("Movimiento completado");
            _endTurn.Raise();
        }
    }
}