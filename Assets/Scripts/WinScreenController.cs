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

        private void OnEnable()
        {
            _winScreenView.OnPlayerPressedRematch += PlayerPressedRematch;
        }

        private void OnDisable()
        {
            _winScreenView.OnPlayerPressedRematch -= PlayerPressedRematch;
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

                    Debug.Log($"playersWinUI.Length {playersWinUI.Length}");
                    Debug.Log($"NetworkManager.ConnectedClients.Count {NetworkManager.ConnectedClients.Count}");
                    if (playersWinUI != null && playersWinUI.Length == NetworkManager.ConnectedClients.Count)
                    {
                        Debug.Log("rematch");
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