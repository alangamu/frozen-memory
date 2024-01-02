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

        public void Setup(ulong localPlayerId)
        {
            //_userNameText.text = playerName;
            _playerId = (int) localPlayerId;
            gameObject.SetActive(true);
            _scoreText.text = _score.ToString();
            _userNameText.text = localPlayerId.ToString();

            _background.color = (int)NetworkManager.Singleton.LocalClientId == _playerId ? _ownerColor : _defaultColor;
            gameObject.transform.localScale = (int)NetworkManager.Singleton.LocalClientId == _playerId ? Vector3.one * 0.6f : Vector3.one * 0.5f;
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