using System;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
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
        private LobbyManager _lobbyManager;

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

        private async void GameStart()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                Lobby lobby = await _lobbyManager.GetLobby(_lobbyManager.JoinedLobbyId);

                //int index = 0;
                //foreach (var player in lobby.Players)
                //{
                //    string playerName = string.Empty;
                //    //TODO: change for player's name
                //    ulong clientId = NetworkManager.Singleton.ConnectedClientsList[index].ClientId;
                //    SetupPlayerScoreUIClientRpc(index, clientId, playerName);
                //}

                //for (int i = 0; i < lobby.Players.Count; i++)
                //{
                //    string playerName = lobby.Players[i].Data["PlayerName"].Value;
                //    string clientId = lobby.Players[i].Id;
                //    SetupPlayerScoreUIClientRpc(i, clientId, playerName);
                //}

                for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++)
                {
                    NetworkClient networkClient = NetworkManager.Singleton.ConnectedClientsList[i];
                    //int index = NetworkManager.Singleton.ConnectedClientsList, x => x. == networkClient);
                    int index = Array.FindIndex(NetworkManager.Singleton.ConnectedClientsList.ToArray(), x => x == networkClient);
                    string playerName = lobby.Players[index].Data["PlayerName"].Value;
                    //string playerName = lobby.Players[i].Data["PlayerName"].Value;
                    //TODO: change for player's name
                    ulong clientId = networkClient.ClientId;
                    SetupPlayerScoreUIClientRpc(i, (int)clientId, playerName);
                }
            }
        }

        [ClientRpc]
        private void PlayerScoredClientRpc(int playerId)
        {
            Array.Find(_playerScoreUIControllers, x => x.PlayerId.Equals(playerId)).AddPoint();
        }

        [ClientRpc]
        private void SetupPlayerScoreUIClientRpc(int index, int clientId, string playerName)
        {
            _playerScoreUIControllers[index].Setup(clientId, playerName);
            //_infoText.text = $"index {index}, clientId {clientId}";
            //Debug.LogError($"index {index}, clientId {clientId}");
        }
    }
}