using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameSceneTest : MonoBehaviour
    {
        //[SerializeField]
        //private Button _hostButton;
        //[SerializeField]
        //private Button _clientButton;
        [SerializeField]
        private Button _spawnBoardButton;
        //[SerializeField]
        //private Transform _buttonsPanelTransform;
        [SerializeField]
        private BoardManager _boardManager;


        private void Awake()
        {
            _spawnBoardButton.gameObject.SetActive(true);

            //_hostButton.onClick.AddListener(() =>
            //{
            //    NetworkManager.Singleton.StartHost();
            //    _buttonsPanelTransform.gameObject.SetActive(false);
            //    _infoText.text = "Host";
            //    _spawnBoardButton.gameObject.SetActive(true);
            //});

            //_clientButton.onClick.AddListener(() =>
            //{
            //    NetworkManager.Singleton.StartClient();
            //    _buttonsPanelTransform.gameObject.SetActive(false);
            //    _infoText.text = "Client";
            //});

            //_spawnBoardButton.onClick.AddListener(() =>
            //{
            //    //_buttonsPanelTransform.gameObject.SetActive(false);
            //    _spawnBoardButton.gameObject.SetActive(false);

            //    _boardManager.Initialize();
            //});
        }

        //private async void Start()
        //{
        //    if (NetworkManager.Singleton.IsServer)
        //    {
        //        Debug.Log($"IsServer GameSceneTest");
        //        await Task.Delay(2000);
        //        _boardManager.Initialize();
        //    }
        //    else
        //    {
        //        Debug.Log($"Is not Server");
        //    }
        //}
    }
}