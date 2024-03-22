using Assets.Scripts.ScriptableObjects.Events;
using Assets.Scripts.ScriptableObjects.Variables;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoardSinglePlayerController : MonoBehaviour
    {
        [SerializeField]
        private TileControllerEvent _onTileClick;
        [SerializeField]
        private BoardView _boardView;
        [SerializeField]
        private IntVariable _tilesAmountVariable;
        [SerializeField]
        private GameEvent _playerMovedEvent;
        [SerializeField]
        private GameEvent _playerWinEvent;
        [SerializeField]
        private GameEvent _initializeEvent;

        private List<int> _randomNumberList = new List<int>();
        private List<TileController> _activeTiles = new List<TileController>();
        private List<TileController> _doneTiles = new List<TileController>();

        private void CreateBoard()
        {
            _randomNumberList.Clear();
            _activeTiles.Clear();
            _doneTiles.Clear();

            for (int i = 0; i < _tilesAmountVariable.Value / 2; i++)
            {
                _randomNumberList.Add(i);
                _randomNumberList.Add(i);
            }

            Shuffle();

            _boardView.CreateTiles(_randomNumberList);
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

        private void OnEnable()
        {
            _initializeEvent.OnRaise += Initialize;
            _onTileClick.OnRaise += OnTileClicked;
        }

        private void OnDisable()
        {
            _initializeEvent.OnRaise -= Initialize;
            _onTileClick.OnRaise -= OnTileClicked;
        }

        private void Initialize()
        {
            CreateBoard();
        }

        private void OnTileClicked(TileController tile)
        {
            if (_activeTiles.Count < 2)
            {
                if (_activeTiles.Count == 1)
                {
                    //double click or click the same tile
                    if (_activeTiles.Contains(tile))
                    {
                        return;
                    }
                }

                tile.ShowTile();

                _activeTiles.Add(tile);

                if (_activeTiles.Count == 2)
                {
                    Resolve(_activeTiles[0], _activeTiles[1]);
                }
            }
        }

        private async void Resolve(TileController firstTile, TileController secondTile)
        {
            _playerMovedEvent.Raise();
            
            if (firstTile.TileIndex == secondTile.TileIndex)
            {
                await Task.Delay(1000);
                firstTile.SetIsDone(true);
                secondTile.SetIsDone(true);

                _doneTiles.Add(firstTile);
                _doneTiles.Add(secondTile);

                if (_doneTiles.Count == _tilesAmountVariable.Value)
                {
                    _playerWinEvent.Raise();
                    return;
                }
            }
            else
            {
                await Task.Delay(1000);
                _activeTiles.Clear();
                firstTile.HideTile();
                secondTile.HideTile();
            }

            _activeTiles.Clear();
        }

        private void Start()
        {
            _initializeEvent.Raise();
        }
    }
}