using Assets.Scripts.ScriptableObjects;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayerScoreView : MonoBehaviour
    {
        public string PlayerId => _playerId;

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
        private Image _avatarImage;
        [SerializeField]
        private AvatarModel _avatarModel;

        private int _score = 0;
        private string _playerId;
        private bool _isOwner;

        public void Initialize(string playerId, string playerName, int avatarIndex)
        {
            _avatarImage.sprite = _avatarModel.Avatars[avatarIndex];
            _userNameText.text = playerName;
            _playerId = playerId;
            gameObject.SetActive(true);
            _scoreText.text = _score.ToString();

            _isOwner = AuthenticationService.Instance.PlayerId.Equals(_playerId);

            _background.color = _isOwner ? _ownerColor : _defaultColor;
            gameObject.transform.localScale = _isOwner ? Vector3.one * 0.6f : Vector3.one * 0.5f;
            _turnIndicator.gameObject.SetActive(false);
            _turnIndicatorText.gameObject.SetActive(false);
        }

        public void AddPoint()
        {
            _score++;
            _scoreText.text = _score.ToString();
        }

        public void SetActivePlayer(bool isActive)
        {
            _turnIndicatorText.gameObject.SetActive(isActive && _isOwner);
            _turnIndicator.gameObject.SetActive(isActive);
        }
    }
}