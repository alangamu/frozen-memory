using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField]
        private GameEvent _initializeEvent;
        [SerializeField] 
        private LobbyManager _lobbyManager;

        private async void Start()
        {
            _lobbyManager.StopHeartbeat();
            //TODO: listen to loaded clients connected or something like that
            if (NetworkManager.Singleton.IsServer)
            {
                await Task.Delay(3000);
                InitializeClientRpc();
            }
        }

        [ClientRpc]
        private void InitializeClientRpc()
        {
            _initializeEvent.Raise();
        }
   }
}