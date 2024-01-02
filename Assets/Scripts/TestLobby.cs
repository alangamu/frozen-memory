using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{
    private Lobby _hostLobby;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log($"Signed in {AuthenticationService.Instance.PlayerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
    }

    private async void CreateLobby()
    {
        string lobbyName = "Lobby one";
        int maxPlayers = 4;
        try 
        {
            _hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

            Debug.Log($"Lobby Created {_hostLobby.Name} {_hostLobby.MaxPlayers}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log($"Loobies found: {queryResponse.Results.Count}");

            foreach (var item in queryResponse.Results)
            {
                Debug.Log($"{item.Name} {item.MaxPlayers}");
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
