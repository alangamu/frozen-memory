using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ScoreManager : NetworkBehaviour
    {
        [SerializeField]
        private GameEvent _gameStartEvent;
        [SerializeField]
        private PlayerScoreUIController[] _playerScoreUIControllers;
        [SerializeField]
        private IntGameEvent _playerScored;
        [SerializeField]
        private PlayerWinController _playerWinController;

        [SerializeField]
        private Text _infoText;

        public string GetHighScorePlayerName()
        {
            PlayerScoreUIController playerScoreUIController = _playerScoreUIControllers[0];

            foreach (var controller in _playerScoreUIControllers)
            {
                if (controller.Score > playerScoreUIController.Score)
                {
                    playerScoreUIController = controller;
                }
            }

            return playerScoreUIController.PlayerId.ToString();
        }

        private void OnEnable()
        {
            _gameStartEvent.OnRaise += GameStart;
            _playerScored.OnRaise += PlayerScoredClientRpc;
            _playerWinController.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _gameStartEvent.OnRaise -= GameStart;
            _playerScored.OnRaise -= PlayerScoredClientRpc;
        }

        private void GameStart()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++)
                {
                    //TODO: change for player's name
                    ulong clientId = NetworkManager.Singleton.ConnectedClientsList[i].ClientId;
                    SetupPlayerScoreUIClientRpc(i, clientId);
                }
            }
        }

        [ClientRpc]
        private void PlayerScoredClientRpc(int playerId)
        {
            Array.Find(_playerScoreUIControllers, x => x.PlayerId == playerId).AddPoint();
        }

        [ClientRpc]
        private void SetupPlayerScoreUIClientRpc(int index, ulong clientId)
        {
            _playerScoreUIControllers[index].Setup(clientId);
            //_infoText.text = $"index {index}, clientId {clientId}";
            //Debug.LogError($"index {index}, clientId {clientId}");
        }
    }
}