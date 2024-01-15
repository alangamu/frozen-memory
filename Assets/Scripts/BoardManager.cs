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
        private IntVariable _activePlayerId;
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
        private GameEvent _initializeEvent;
        [SerializeField]
        private GameEvent _endTurnEvent;
        [SerializeField]
        private IntGameEvent _playerScored;
        [SerializeField]
        private GameEvent _gameOverEvent;
        [SerializeField]
        private GameEvent _stopCountdown;
        [SerializeField]
        private GameEvent _restartTurnEvent;

        private List<int> _randomNumberList = new List<int>();
        private List<int> _activeTilesIndex = new List<int>();

        private void OnEnable()
        {
            foreach (var tileController in _tileControllers)
            {
                tileController.OnTileClicked += OnTileClicked;
            }

            _endTurnEvent.OnRaise += EndTurn;
            _initializeEvent.OnRaise += Initialize;
        }

        private void OnDisable()
        {
            foreach (var tileController in _tileControllers)
            {
                tileController.OnTileClicked -= OnTileClicked;
            }

            _endTurnEvent.OnRaise -= EndTurn;
            _initializeEvent.OnRaise -= Initialize;
        }

        private void EndTurn()
        {
            HideContentClientRpc();
            _activeTilesIndex.Clear();
        }

        private void OnTileClicked(int localClientId, TileController tileController)
        {
            if (_activePlayerId.Value == localClientId)
            {
                var index = Array.FindIndex(_tileControllers, x => x == tileController);
                PressTileServerRpc(localClientId, index);
            }
        }

        private void Initialize()
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
            int localPlayerId = (int)NetworkManager.Singleton.LocalClientId;

            for (int i = 0; i < _tilesAmount; i++)
            {
                _tileControllers[i].Initialize(randomNumberList[i], localPlayerId);
                _tileControllers[i].Initialize(_images[randomNumberList[i]]);
            }

            StartGame();
        }

        private async void StartGame()
        {
            await Task.Delay(2000);

            HideContentClientRpc();

            StartGameClientRpc();
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
            //TODO: Fix double click

            if (_activeTilesIndex.Count < 2)
            {
                PressTileClientRpc(tileControllerIndex);

                _activeTilesIndex.Add(_tileControllers[tileControllerIndex].TileIndex);

                if (_activeTilesIndex.Count == 2)
                {
                    CancelCountdownClientRpc();

                    //TODO: resolve the double clicking
                    Resolve(localClientId, _activeTilesIndex[0], _activeTilesIndex[1]);
                    _activeTilesIndex.Clear();
                }
            }
        }

        private async void Resolve(int localClientId, int firstIndex, int secondIndex)
        {
            if (firstIndex == secondIndex)
            {
                PlayerScoredClientRpc(localClientId);

                await Task.Delay(1000);
                LockTilesClientRpc(firstIndex);

                if (Array.FindAll(_tileControllers, x => !x.IsDone).Length == 0)
                {
                    EndGameClientRpc();
                    return;
                }

                RestartTurnClientRpc();
            }
            else
            {
                await Task.Delay(1000);
                EndTurnClientRpc();
            }
        }

        [ClientRpc]
        private void PlayerScoredClientRpc(int localPlayerId)
        {
            _playerScored.Raise(localPlayerId);
        }

        [ClientRpc]
        private void CancelCountdownClientRpc()
        {
            _stopCountdown.Raise();
        }

        [ClientRpc]
        private void RestartTurnClientRpc()
        {
            _restartTurnEvent.Raise();
        }

        [ClientRpc]
        private void EndTurnClientRpc()
        {
            _endTurnEvent.Raise();
        }

        [ClientRpc]
        private void StartGameClientRpc()
        {
            _gameStartEvent.Raise();
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
        private void EndGameClientRpc()
        {
            _gameOverEvent.Raise();
        }
    }
}