using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayerWinController : NetworkBehaviour
    {
        [SerializeField]
        private Text _playerNameText;

        public void PlayerWin(string playerName)
        {
            _playerNameText.text = playerName;
        }
    }
}