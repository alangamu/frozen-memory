using Assets.Scripts.ScriptableObjects.Variables;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField]
        private Transform _tilesRootTransform;

        [SerializeField]
        private TileController _tilePrefab;

        [SerializeField]
        private TileSpritesVariable _tileSprites;


        public void CreateTiles(List<int> tilesList)
        {
            foreach (Transform tileTransform in _tilesRootTransform)
            {
                Destroy(tileTransform.gameObject);
            }

            int index = 0;
            foreach (var tileIndex in tilesList)
            {
                TileController tileController =  Instantiate(_tilePrefab, _tilesRootTransform);
                if (tileController != null )
                {
                    tileController.Initialize(tileIndex, string.Empty);
                    tileController.Initialize(_tileSprites.Value[tileIndex]);
                    tileController.HideTile();
                    tileController.name = $"Tile {index}";
                    index++;
                }
            }
        }
    }
}