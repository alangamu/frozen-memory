using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoardManager : NetworkBehaviour
    {
        [SerializeField]
        private int _tilesAmount = 50;
        [SerializeField] 
        private Sprite[] _images;
        [SerializeField]
        private TileController[] _tileControllers;
        [SerializeField]
        private Transform _content;
        [SerializeField]
        private GameEvent _gameStartEvent;
        [SerializeField]
        private GameEvent _nextTurnEvent;
        [SerializeField]
        private IntGameEvent _playerScored;
        //[SerializeField]
        //private PlayerWinController _playerWinController;
        //[SerializeField]
        //private ScoreManager _scoreManager;

        //[SerializeField]
        //private Button _spawnBoardButton;

        private List<int> _randomNumberList = new List<int>();
        private List<int> _activeTilesIndex = new List<int>();

        private void OnEnable()
        {
            foreach (var tileController in _tileControllers)
            {
                tileController.OnTileClicked += OnTileClicked;
            }
        }

        private void OnDisable()
        {
            foreach (var tileController in _tileControllers)
            {
                tileController.OnTileClicked -= OnTileClicked;
            }
        }

        private void OnTileClicked(int localClientId, TileController tileController)
        {
            var index = Array.FindIndex(_tileControllers, x => x == tileController);
            PressTileServerRpc(localClientId, index);
        }

        public void Initialize()
        {
            Debug.Log("Initialize");
            ShowContentClientRpc();

            if (NetworkManager.Singleton.IsServer)
            {
                CreateBoardServerRpc();
            }
        }

        [ServerRpc]
        public void CreateBoardServerRpc()
        {
            Debug.Log("CreateBoard");
            for (int i = 0; i < _tilesAmount / 2; i++)
            {
                _randomNumberList.Add(i);
                _randomNumberList.Add(i);
            }

            Shuffle();
            DisplayBoardClientRpc(_randomNumberList.ToArray());
        }

        [ClientRpc]
        private void DisplayBoardClientRpc(int[] randomNumberList)
        {
            Debug.Log("DisplayBoardClientRpc");
            for (int i = 0; i < _tilesAmount; i++)
            {
                _tileControllers[i].Initialize(randomNumberList[i]);
                _tileControllers[i].Initialize(_images[randomNumberList[i]]);
            }
        }

        private void Shuffle()
        {
            int n = _randomNumberList.Count;
            System.Random rng = new System.Random();

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = _randomNumberList[k];
                _randomNumberList[k] = _randomNumberList[n];
                _randomNumberList[n] = value;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PressTileServerRpc(int localClientId, int tileControllerIndex)
        {
            if (_activeTilesIndex.Count < 2)
            {
                PressTileClientRpc(localClientId, tileControllerIndex);

                _activeTilesIndex.Add(_tileControllers[tileControllerIndex].TileIndex);

                if (_activeTilesIndex.Count == 2)
                {
                    Resolve(localClientId, _activeTilesIndex[0], _activeTilesIndex[1]);
                }
            }
        }

        private async void Resolve(int localClientId, int firstIndex, int secondIndex)
        {
            if (firstIndex == secondIndex)
            {
                Debug.Log("Correct");
                _playerScored.Raise(localClientId);
                await Task.Delay(1000);
                LockTilesClientRpc(firstIndex);

                if (Array.FindAll(_tileControllers, x => !x.IsDone).Length == 0)
                {
                    OnWinGameClientRpc("Kim");
                    //OnWinGameClientRpc(_scoreManager.GetHighScorePlayerName());
                }
            }
            else
            {
                Debug.Log("incorrect");
                await Task.Delay(1000);
                HideContentClientRpc();
                _nextTurnEvent.Raise();
            }

            _activeTilesIndex.Clear();
        }

        [ClientRpc]
        private void LockTilesClientRpc(int Index)
        {
            TileController[] tiles = Array.FindAll(_tileControllers, x => x.TileIndex == Index);
            foreach (TileController tile in tiles)
            {
                tile.SetIsDone(true);
            }
        }

        [ClientRpc]
        private void PressTileClientRpc(int localClientId, int tileControllerIndex)
        {
            TileController tileController = _tileControllers[tileControllerIndex];
            tileController.ShowTile();
        }

        [ClientRpc]
        private void ShowContentClientRpc()
        {
            Debug.Log("ShowContentClientRpc");
            _gameStartEvent.Raise();
            _content.gameObject.SetActive(true);
        }

        [ClientRpc]
        private void HideContentClientRpc()
        {
            foreach (var tile in _tileControllers)
            {
                tile.HideTile();
            }
        }

        [ClientRpc]
        private void OnWinGameClientRpc(string winClientId)
        {
            Debug.Log("OnWinGameClientRpc");
            //_playerWinController.gameObject.SetActive(true);
            //_playerWinController.PlayerWin(winClientId);
        }

        private void Start()
        {
            Debug.Log("Start BoardManager");
            //if (_playerId == _lobbyManager.HostId)
            //{
            //    Debug.LogError("StartHost");
            //    NetworkManager.Singleton.StartHost();
            //}
            //else
            //{
            //    Debug.LogError("StartClient");
            //    NetworkManager.Singleton.StartClient();
            //}

            //_spawnBoardButton.gameObject.SetActive(false);
            //if (NetworkManager.Singleton.IsServer)
            //{
            //    _spawnBoardButton.gameObject.SetActive(true);
            //    _spawnBoardButton.onClick.AddListener(() => {
            //        _spawnBoardButton.gameObject.SetActive(false);

            //        Initialize();
            //    });
                //Initialize();
            //}
        }
    }
}