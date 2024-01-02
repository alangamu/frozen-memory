using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TileController : MonoBehaviour
    {
        public event Action<int, TileController> OnTileClicked;
        public int TileIndex => _tileIndex;
        public bool IsDone => _isDone;

        [SerializeField]
        private Image _tileImage;
        [SerializeField]
        private IntVariable _activePlayerId;
        [SerializeField]
        private Text _tileText;
        [SerializeField]
        private Transform _tileIsDone;

        private bool _isDone;
        private int _tileIndex;
        private Sprite _tileSprite;

        public async void Initialize(Sprite tileSprite)
        {
            _isDone = false;
            _tileIsDone.gameObject.SetActive(false);
            _tileSprite = tileSprite;
            ShowTile();
            await Task.Delay(2000);
            HideTile();
        }

        public void Initialize(int tileNumber)
        {
            _tileIndex = tileNumber;
            //_tileText.text = tileNumber.ToString();
            _tileText.text = string.Empty;
        }

        public void PressButton()
        {
            if (!_isDone)
            {
                if (_activePlayerId.Value == (int)NetworkManager.Singleton.LocalClientId)
                {
                    OnTileClicked?.Invoke(_activePlayerId.Value, this); 
                }
            }
        }

        public void ShowTile()
        {
            _tileImage.sprite = _tileSprite;
        }

        public void HideTile()
        {
            if(!_isDone)
            {
                _tileImage.sprite = null;
            }
        }

        public void SetIsDone(bool isDone)
        {
            _isDone = isDone;
            if (_isDone)
            {
                _tileIsDone.gameObject.SetActive(true);
            }
        }
    }
}