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
        [SerializeField]
        private GameEvent _gameOverEvent;

        private List<int> _randomNumberList = new List<int>();
        private List<int> _activeTilesIndex = new List<int>();

        //private int _testNumber = 0;

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
            ShowContentClientRpc();

            if (NetworkManager.Singleton.IsServer)
            {
                CreateBoardServerRpc();
            }
        }

        [ServerRpc]
        public void CreateBoardServerRpc()
        {
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
                PressTileClientRpc(tileControllerIndex);

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
                //_testNumber++;

                _playerScored.Raise(localClientId);
                await Task.Delay(1000);
                LockTilesClientRpc(firstIndex);

                if (Array.FindAll(_tileControllers, x => !x.IsDone).Length == 0)
                //if (_testNumber == 3)
                {
                    Debug.Log("Win");
                    OnGameOverClientRpc();
                }
            }
            else
            {
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
        private void PressTileClientRpc(int tileControllerIndex)
        {
            TileController tileController = _tileControllers[tileControllerIndex];
            tileController.ShowTile();
        }

        [ClientRpc]
        private void ShowContentClientRpc()
        {
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
        private void OnGameOverClientRpc()
        {
            _gameOverEvent.Raise();
        }
    }
}