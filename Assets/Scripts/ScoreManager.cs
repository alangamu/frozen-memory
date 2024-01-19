using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Assets.Scripts
{
    public class ScoreManager : NetworkBehaviour
    {
        [SerializeField]
        private GameEvent _initializeEvent;
        [SerializeField]
        private PlayerScoreUIController[] _playerScoreUIControllers;
        [SerializeField]
        private IntGameEvent _playerScored;
        [SerializeField]
        private GameEvent _gameOverEvent;
        [SerializeField]
        private LobbyManager _lobbyManager;
        [SerializeField]
        private Transform _winPanelTransform;

        [SerializeField]
        private PlayerWinUI[] _playerWinUIControllers;

        private Dictionary<int, string> _players = new Dictionary<int, string>();
        private Dictionary<int, int> _score = new Dictionary<int, int>();

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
            _initializeEvent.OnRaise += Initialize;
            _playerScored.OnRaise += PlayerScoredClientRpc;
            _winPanelTransform.gameObject.SetActive(false);
            _gameOverEvent.OnRaise += OnGameOver;
        }

        private void OnDisable()
        {
            _initializeEvent.OnRaise -= Initialize;
            _playerScored.OnRaise -= PlayerScoredClientRpc;
            _gameOverEvent.OnRaise -= OnGameOver;
        }

        private void OnGameOver()
        {
            _winPanelTransform.gameObject.SetActive(true);

            if (NetworkManager.Singleton.IsHost)
            {
                int index = 1;
                while (_score.Count > 0)
                {
                    int idPlayerHighScore = GetIdPlayerHighScore();
                    ShowWinPanelClientRpc(_players[idPlayerHighScore], _score[idPlayerHighScore], index++);
                    _score.Remove(idPlayerHighScore);
                }
            }
        }

        private int GetIdPlayerHighScore()
        {
            var highScorePlayerId = _score.Aggregate((max, kvp) => kvp.Value > max.Value ? kvp : max);

            return highScorePlayerId.Key;
        }

        private async void Initialize()
        {
            foreach (var playerUIController in _playerWinUIControllers)
            {
                playerUIController.gameObject.SetActive(false);
            }

            if (NetworkManager.Singleton.IsHost)
            {
                Lobby lobby = await _lobbyManager.GetLobby(_lobbyManager.JoinedLobbyId);

                for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++)
                {
                    NetworkClient networkClient = NetworkManager.Singleton.ConnectedClientsList[i];
                    int index = Array.FindIndex(NetworkManager.Singleton.ConnectedClientsList.ToArray(), x => x == networkClient);
                    
                    //TODO: make a stringVariable for PlayerName
                    string playerName = lobby.Players[index].Data["PlayerName"].Value;
                    ulong clientId = networkClient.ClientId;

                    SetupPlayerScoreUIClientRpc(i, (int)clientId, playerName);
                    _players.Add((int)clientId, playerName);
                    _score.Add((int)clientId, 0);
                }
            }
        }

        [ClientRpc]
        private void PlayerScoredClientRpc(int playerId)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                _score[playerId] = _score[playerId] + 1;
            }
        }

        [ClientRpc]
        private void SetupPlayerScoreUIClientRpc(int index, int playerId, string playerName)
        {
            _playerScoreUIControllers[index].Setup(playerId, playerName);
        }

        [ClientRpc]
        private void ShowWinPanelClientRpc(string playerName, int playerScore, int index)
        {
            _playerWinUIControllers[index].gameObject.SetActive(true);
            _playerWinUIControllers[index].Initialize(playerName, playerScore, index);
            //_playerWinController.gameObject.SetActive(true);
            //_playerWinController.PlayerWin(playerName);
        }

        //[ClientRpc]
        //private void ShowWinPanelClientRpc(string playerName)
        //{
        //    _playerWinController.gameObject.SetActive(true);
        //    _playerWinController.PlayerWin(playerName);
        //}
    }
}