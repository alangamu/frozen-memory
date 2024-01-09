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
        private Color _defaultColor;
        [SerializeField]
        private Color _ownerColor;
        [SerializeField]
        private Image _background;
        [SerializeField]
        private IntVariable _activePlayerIdVariable;

        private int _score = 0;
        private int _playerId;

        public void AddPoint()
        {
            _score++;
            _scoreText.text = _score.ToString();
        }

        public void Setup(int playerId, string playerName)
        {
            _userNameText.text = playerName;
            _playerId = playerId;
            gameObject.SetActive(true);
            _scoreText.text = _score.ToString();

            bool isOwner = (int)NetworkManager.Singleton.LocalClientId == _playerId;

            _background.color = isOwner ? _ownerColor : _defaultColor;
            gameObject.transform.localScale = isOwner ? Vector3.one * 0.6f : Vector3.one * 0.5f;
            _turnIndicator.gameObject.SetActive(_activePlayerIdVariable.Value == _playerId);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _activePlayerIdVariable.OnValueChanged += ActivePlayerChanged;
        }

        private void OnDisable()
        {
            _activePlayerIdVariable.OnValueChanged -= ActivePlayerChanged;
        }

        private void ActivePlayerChanged(int activePlayerId)
        {
            _turnIndicator.gameObject.SetActive(activePlayerId == _playerId);
        }
    }
}