using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.ScriptableObjects.Events;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.ScriptableObjects.Variables;

namespace Assets.Scripts
{
    public class LobbyWaitingRoomEnterPlayerController : NetworkBehaviour
    {
        [SerializeField]
        private StringBoolEvent _setPlayerReadyLobbyWaitingRoomEvent;
        [SerializeField]
        private StringStringIntEvent _addPlayerReadyLobbyWaitingRoomEvent;
        [SerializeField]
        private StringBoolEvent _showRemoveButtonLobbyWaitingRoomEvent;

        [SerializeField]
        private StringGameEvent _setLobbyNameLobbyWaitingRoomEvent;

        [SerializeField]
        private BoolEvent _setReadyLobbyWaitingRoomEvent;
        [SerializeField]
        private BoolEvent _showLoadingLobbyWaitingRoomEvent;
        [SerializeField]
        private BoolEvent _setEnableReadyButtonLobbyWaitingRoomEvent;

        [SerializeField]
        private GameEvent _clearPlayersLobbyWaitingRoomEvent;

        [SerializeField]
        private StringVariable _keyStartGameVariable;

        [SerializeField]
        private PlayersModel _playersController;

        [SerializeField]
        private LobbyManager _lobbyManager;

        private string _playerId;

        private async void Start()
        {
            _showLoadingLobbyWaitingRoomEvent.Raise(true);
            _playerId = AuthenticationService.Instance.PlayerId;

            if (_playerId.Equals(_lobbyManager.HostId))
            {
                _setEnableReadyButtonLobbyWaitingRoomEvent.Raise(false);
            }

            _setReadyLobbyWaitingRoomEvent.Raise(false);
            Lobby joinedLobby = await _lobbyManager.GetLobby(_lobbyManager.JoinedLobbyId);

            _setLobbyNameLobbyWaitingRoomEvent.Raise(joinedLobby.Name);

            var callbacks = new LobbyEventCallbacks();
            callbacks.PlayerJoined += OnPlayerJoined;
            //callbacks.PlayerLeft += OnPlayerLeft;

            try
            {
                var m_LobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(joinedLobby.Id, callbacks);
            }
            catch (LobbyServiceException ex)
            {
                switch (ex.Reason)
                {
                    case LobbyExceptionReason.AlreadySubscribedToLobby: Debug.LogWarning($"Already subscribed to lobby[{joinedLobby.Id}]. We did not need to try and subscribe again. Exception Message: {ex.Message}"); break;
                    case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); throw;
                    case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); throw;
                    default: throw;
                }
            }

            if (_playerId.Equals(_lobbyManager.HostId))
            {
                Debug.Log($"start game is host");
                await _lobbyManager.StartGame();
            }
            else
            {
                Debug.Log($"join with relay {joinedLobby.Data[_keyStartGameVariable.Value].Value}");
                bool isJoined = await _lobbyManager.StartClientWithRelay(joinedLobby.Data[_keyStartGameVariable.Value].Value);

                Debug.Log($"isJoined {isJoined}");
            }

            _playersController.CLearPlayers();
            _clearPlayersLobbyWaitingRoomEvent.Raise();

            _showLoadingLobbyWaitingRoomEvent.Raise(false);

            foreach (var player in joinedLobby.Players)
            {
                if (player.Data.TryGetValue("PlayerName", out PlayerDataObject playerName))
                {
                    if (player.Data.TryGetValue("avatarIndex", out PlayerDataObject playerAvatarIndex))
                    {
                        PlayerInfo playerInfo = new PlayerInfo(int.Parse(playerAvatarIndex.Value), playerName.Value);
                        _playersController.AddPlayer(player.Id, playerInfo);
                        _addPlayerReadyLobbyWaitingRoomEvent.Raise(playerName.Value, player.Id, int.Parse(playerAvatarIndex.Value));
                    }
                }
                if (player.Data.TryGetValue("ready", out PlayerDataObject playerReady))
                {
                    _setPlayerReadyLobbyWaitingRoomEvent.Raise(player.Id, playerReady.Value.Equals("true"));
                }
            }
        }

        private void OnPlayerJoined(List<LobbyPlayerJoined> list)
        {
            _setEnableReadyButtonLobbyWaitingRoomEvent.Raise(true);
            AddPlayerRpc(list[0].Player.Id, list[0].Player.Data["PlayerName"].Value, int.Parse(list[0].Player.Data["avatarIndex"].Value));
            _showRemoveButtonLobbyWaitingRoomEvent.Raise(list[0].Player.Id, NetworkManager.Singleton.IsHost);
        }


        [Rpc(SendTo.Everyone)]
        private void AddPlayerRpc(string playerId, string playerName, int avatarIndex)
        {
            Debug.Log($"AddPlayerRpc {playerName}");
            PlayerInfo playerInfo = new PlayerInfo(avatarIndex, playerName);
            _playersController.AddPlayer(playerId, playerInfo);
            _addPlayerReadyLobbyWaitingRoomEvent.Raise(playerName, playerId, avatarIndex);
        }
    }
}