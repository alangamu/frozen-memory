using Assets.Scripts.ScriptableObjects.Events;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TileController : MonoBehaviour
    {
        public event Action<string, TileController> OnTileClicked;

        public int TileIndex => _tileIndex;
        public bool IsDone => _isDone;

        [SerializeField]
        private TileControllerEvent _onTileClick;

        [SerializeField]
        private Image _tileImage;
        [SerializeField]
        private Text _tileText;
        [SerializeField]
        private Transform _tileIsDone;
        
        //TODO: delete this field, just for debuging
        [SerializeField]
        private bool _isShowingIndex;

        private bool _isDone;
        private int _tileIndex;
        private Sprite _tileSprite;
        private string _playerId;

        public void Initialize(Sprite tileSprite)
        {
            _isDone = false;
            _tileIsDone.gameObject.SetActive(false);
            _tileSprite = tileSprite;
            ShowTile();
        }

        public void Initialize(int tileNumber, string playerId)
        {
            _playerId = playerId;
            _tileIndex = tileNumber;
            _tileText.text = string.Empty;
            
            _tileText.text = _isShowingIndex ? tileNumber.ToString() : string.Empty;
        }

        public void PressButton()
        {
            if (!_isDone)
            {
                OnTileClicked?.Invoke(_playerId, this);
                _onTileClick.Raise(this);
            }
        }

        public void ShowTile()
        {
            _tileImage.gameObject.SetActive(true);
            _tileImage.sprite = _tileSprite;
        }

        public void HideTile()
        {
            if(!_isDone)
            {
                _tileImage.gameObject.SetActive(false);
                _tileImage.sprite = null;
            }
        }

        public void SetIsDone(bool isDone)
        {
            _isDone = isDone;
            if (_isDone)
            {
                _tileImage.gameObject.SetActive(false);
                _tileIsDone.gameObject.SetActive(true);
            }
        }
    }
}