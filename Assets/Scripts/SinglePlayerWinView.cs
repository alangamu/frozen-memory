using Assets.Scripts.ScriptableObjects.Events;
using Assets.Scripts.ScriptableObjects.Variables;
using Ricimi;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SinglePlayerWinView : MonoBehaviour
    {
        [SerializeField]
        private GameEvent _restartSinglePlayerEvent;
        [SerializeField]
        private GameEvent _loadMainMenuEvent;
        [SerializeField] 
        private Text _movesCountText;
        [SerializeField]
        private Text _personalBestMovesText;
        [SerializeField]
        private IntVariable _playerMoves;
        [SerializeField]
        private AnimatedButton _mainMenuButton;
        [SerializeField]
        private AnimatedButton _restartButton;
        [SerializeField]
        private IntVariable _personalBestMoves;

        private void OnEnable()
        {
            _mainMenuButton.onClick.AddListener( ()=> 
            {
                _loadMainMenuEvent.Raise();
            });
            _restartButton.onClick.AddListener(() =>
            {
                _restartSinglePlayerEvent.Raise();
            });

            _movesCountText.text = _playerMoves.Value.ToString();
            Debug.Log($"Win View Start _personalBestMoves.Value {_personalBestMoves.Value}");
            _personalBestMovesText.text = _personalBestMoves.Value.ToString();
        }

        private void OnDisable()
        {
            _mainMenuButton.onClick.RemoveAllListeners();
            _restartButton.onClick.RemoveAllListeners();
        }
    }
}