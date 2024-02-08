using Ricimi;
using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

namespace Assets.Scripts
{
    public class WinScreenController : NetworkBehaviour
    {
        [SerializeField]
        private WinScreenView _winScreenView;
        [SerializeField]
        private GameEvent _rematchEvent;
        [SerializeField]
        private SceneTransition _sceneTransition;

        private void OnEnable()
        {
            _winScreenView.OnPlayerPressedRematch += PlayerPressedRematch;
            _winScreenView.OnPlayerPressedRestart += PlayerPressedRestart;
        }

        private void OnDisable()
        {
            _winScreenView.OnPlayerPressedRematch -= PlayerPressedRematch;
            _winScreenView.OnPlayerPressedRestart -= PlayerPressedRestart;
        }

        private void PlayerPressedRestart()
        {
            NetworkManager.Singleton.Shutdown();
            _sceneTransition.PerformTransition();
        }

        private void PlayerPressedRematch()
        {
            PlayerPressReadyServerRpc(AuthenticationService.Instance.PlayerId);
        }

        private void Start()
        {
            foreach (var playerWinView in _winScreenView.PlayerWinViews)
            {
                playerWinView.gameObject.SetActive(false);
            }

            _winScreenView.gameObject.SetActive(false);
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayerPressReadyServerRpc(string playerId)
        {
            PlayerPressReadyClientRpc(playerId);
        }

        [ClientRpc]
        private void PlayerPressReadyClientRpc(string playerId)
        {
            PlayerWinView playerWinUI = Array.Find(_winScreenView.PlayerWinViews, x => x.PlayerId != null && x.PlayerId.Equals(playerId));
            if (playerWinUI != null)
            {
                playerWinUI.SetReady(true);

                if (NetworkManager.Singleton.IsHost)
                {
                    PlayerWinView[] playersWinUI = Array.FindAll(_winScreenView.PlayerWinViews, x => x.PlayerId != null && x.IsReady);

                    if (playersWinUI != null && playersWinUI.Length == NetworkManager.ConnectedClients.Count)
                    {
                        RematchGameClientRpc();
                    }
                }
            }
        }

        [ClientRpc]
        private void RematchGameClientRpc()
        {
            _rematchEvent.Raise();
        }
    }
}