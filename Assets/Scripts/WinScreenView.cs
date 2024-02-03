using Ricimi;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class WinScreenView : MonoBehaviour
    {
        public event Action OnPlayerPressedRematch;

        public PlayerWinView[] PlayerWinViews => _playerWinViews;

        [SerializeField]
        private AnimatedButton _remachtButton;
        [SerializeField]
        private PlayerWinView[] _playerWinViews;

        private void OnEnable()
        {
            _remachtButton.onClick.AddListener(() => {
                PlayerPress();
            });
        }

        private void OnDisable()
        {
            _remachtButton.onClick.RemoveAllListeners();
        }

        private void PlayerPress()
        {
            OnPlayerPressedRematch?.Invoke();
        }
    }
}