using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayerScoreUIController : NetworkBehaviour
    {
        public int PlayerId => _playerId;
        public int Score => _score;

        [SerializeField]
        private Text _userNameText;
        [SerializeField]
        private Text _scoreText;
        [SerializeField]
        private Transform _turnIndicator;
        [SerializeField]
        private Transform _turnIndicatorText;
        [SerializeField]
        private Color _defaultColor;
        [SerializeField]
        private Color _ownerColor;
        [SerializeField]
        private Image _background;
        [SerializeField]
        private IntGameEvent _beginPlayerTurnEvent;
        [SerializeField]
        private IntGameEvent _playerScored;

        private int _score = 0;
        private int _playerId;
        private bool _isOwner;

        public void Setup(int playerId, string playerName)
        {
            _userNameText.text = playerName;
            _playerId = playerId;
            gameObject.SetActive(true);
            _scoreText.text = _score.ToString();

            _isOwner = (int)NetworkManager.Singleton.LocalClientId == _playerId;

            _background.color = _isOwner ? _ownerColor : _defaultColor;
            gameObject.transform.localScale = _isOwner ? Vector3.one * 0.6f : Vector3.one * 0.5f;
            _turnIndicator.gameObject.SetActive(false);
            _turnIndicatorText.gameObject.SetActive(false);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _beginPlayerTurnEvent.OnRaise += ActivePlayerChanged;
            _playerScored.OnRaise += PlayerScored;
        }

        private void OnDisable()
        {
            _beginPlayerTurnEvent.OnRaise -= ActivePlayerChanged;
            _playerScored.OnRaise -= PlayerScored;
        }

        private void PlayerScored(int playerId)
        {
            if (playerId == _playerId)
            {
                _score++;
                _scoreText.text = _score.ToString();

                //TODO: add some effect
            }
        }

        private void ActivePlayerChanged(int activePlayerId)
        {
            _turnIndicatorText.gameObject.SetActive(_isOwner);
            _turnIndicator.gameObject.SetActive(activePlayerId == _playerId);
        }
    }
}