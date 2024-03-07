using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class WaitingRoomPlayerUI : MonoBehaviour
    {
        public string PlayerId { get; private set; }

        [SerializeField]
        private StringGameEvent _hostRemovePlayerFromLobby;
        [SerializeField]
        private Text _playerNameText;
        [SerializeField]
        private Transform _playerReadyTickTransform;
        [SerializeField]
        private Button _removePlayerButton;
        [SerializeField]
        private AvatarModel _avatarModel;
        [SerializeField]
        private Image _avatarImage;

        public void Initialize(string playerName, string playerId, int avatarIndex)
        {
            _removePlayerButton.gameObject.SetActive(false);
            _playerNameText.text = playerName;
            PlayerId = playerId;
            _playerReadyTickTransform.gameObject.SetActive(false);
            _avatarImage.sprite = _avatarModel.Avatars[avatarIndex];
        }

        public void SetReady(bool isReady)
        {
            _playerReadyTickTransform.gameObject.SetActive(isReady);
        }

        public void ShowKickButton(bool isVisible)
        {
            _removePlayerButton.gameObject.SetActive(isVisible);
        }

        private void OnEnable()
        {
            _removePlayerButton.onClick.AddListener( ()=> { _hostRemovePlayerFromLobby.Raise(PlayerId); });
        }

        private void OnDisable()
        {
            _removePlayerButton.onClick.RemoveAllListeners();
        }
    }
}