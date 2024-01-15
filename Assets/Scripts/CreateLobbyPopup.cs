using Ricimi;
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
            await _lobbyManager.CreateLobby(_lobbyNameText.text);

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