using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class WaitingRoomPlayerUI : MonoBehaviour
    {
        [SerializeField]
        private Text _playerNameText;
        [SerializeField]
        private GameObject _playerReadyTickGameObject;

        public void Initialize(string playerName)
        {
            _playerNameText.text = playerName;
            _playerReadyTickGameObject.SetActive(false);
        }

        public void SetReady(bool isReady)
        {
            _playerReadyTickGameObject.SetActive(isReady);
        }
    }
}