using Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class WaitingRoomPlayerUI : MonoBehaviour
    {
        public string PlayerId { get; private set; }

        [SerializeField]
        private Text _playerNameText;
        [SerializeField]
        private GameObject _playerReadyTickGameObject;
        [SerializeField]
        private AvatarModel _avatarModel;
        [SerializeField]
        private Image _avatarImage;

        public void Initialize(string playerName, string playerId, int avatarIndex)
        {
            _playerNameText.text = playerName;
            PlayerId = playerId;
            _playerReadyTickGameObject.SetActive(false);
            _avatarImage.sprite = _avatarModel.Avatars[avatarIndex];
        }

        public void SetReady(bool isReady)
        {
            _playerReadyTickGameObject.SetActive(isReady);
        }
    }
}