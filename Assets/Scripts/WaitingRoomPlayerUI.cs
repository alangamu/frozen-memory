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

        public void Initialize(string playerName, string playerId)
        {
            _playerNameText.text = playerName;
            PlayerId = playerId;
            _playerReadyTickGameObject.SetActive(false);
        }

        public void SetReady(bool isReady)
        {
            _playerReadyTickGameObject.SetActive(isReady);
        }
    }
}