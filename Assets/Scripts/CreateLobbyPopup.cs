using Ricimi;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CreateLobbyPopup : MonoBehaviour
    {
        [SerializeField]
        private Text _lobbyNameText;
        [SerializeField]
        private LobbyManager _lobbyManager;

        public async void CreateLobby()
        {
            CreateLobbyOptions options = new CreateLobbyOptions();

            options.Player = _lobbyManager.CreatePlayer();

            await _lobbyManager.CreateLobby(_lobbyNameText.text, options);
            
            if (TryGetComponent(out Popup popup))
            {
                popup.Close();
            }

            if (TryGetComponent(out SceneTransition sceneTransition))
            {
                sceneTransition.PerformTransition();
            }
        }
    }
}