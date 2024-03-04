using Ricimi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class QuickJoinController : MonoBehaviour
{
    [SerializeField]
    private Transform _findingMatchPanelTransform;
    [SerializeField]
    private AnimatedButton _findingMatchButton;

    private void OnEnable()
    {
        _findingMatchPanelTransform.gameObject.SetActive(false);
        _findingMatchButton.onClick.AddListener(FindMatch);
    }

    private void OnDisable()
    {
        _findingMatchButton.onClick.RemoveAllListeners();
    }

    private async void FindMatch()
    {
        _findingMatchPanelTransform.gameObject.SetActive(true);

        var players = new List<Player>
        {
            new Player("Player1", new Dictionary<string, object>())
        };


        // Set options for matchmaking
        var options = new CreateTicketOptions(
          "quick-match", // The name of the queue defined in the previous step,
          new Dictionary<string, object>());

        // Create ticket
        var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);

        // Print the created ticket id
        Debug.Log(ticketResponse.Id);
        _findingMatchPanelTransform.gameObject.SetActive(false);

        MultiplayAssignment assignment = null;
        bool gotAssignment = false;
        do
        {
            //Rate limit delay
            await Task.Delay(TimeSpan.FromSeconds(1f));

            // Poll ticket
            var ticketStatus = await MatchmakerService.Instance.GetTicketAsync(ticketResponse.Id);
            if (ticketStatus == null)
            {
                continue;
            }

            //Convert to platform assignment data (IOneOf conversion)
            if (ticketStatus.Type == typeof(MultiplayAssignment))
            {
                assignment = ticketStatus.Value as MultiplayAssignment;
            }

            switch (assignment?.Status)
            {
                case MultiplayAssignment.StatusOptions.Found:
                    gotAssignment = true;
                    Debug.Log("match found");
                    break;
                case MultiplayAssignment.StatusOptions.InProgress:
                    //...
                    break;
                case MultiplayAssignment.StatusOptions.Failed:
                    gotAssignment = true;
                    Debug.LogError("Failed to get ticket status. Error: " + assignment.Message);
                    break;
                case MultiplayAssignment.StatusOptions.Timeout:
                    gotAssignment = true;
                    Debug.LogError("Failed to get ticket status. Ticket timed out.");
                    break;
                default:
                    throw new InvalidOperationException();
            }

        } while (!gotAssignment);
    }
}
