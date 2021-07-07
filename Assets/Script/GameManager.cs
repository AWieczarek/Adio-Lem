using MLAPI;
using MLAPI.Connection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private GameObject myPlayerListItem;
    private int positiveVoteCounter = 0;
    private int voteCounter = 0;

    public override void NetworkStart()
    {
        if (IsServer)
            SpawnAllPlayers();
    }

    private void SpawnAllPlayers()
    {
        foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
        {
            PlayerController pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
            myPlayerListItem = Instantiate(GameController.Instance.playerPrefab, Vector3.zero, Quaternion.identity);
            myPlayerListItem.GetComponent<NetworkObject>().SpawnWithOwnership(nc.Key);
        }
    }

    public void CheckAllPlayersTrigger()
    {
        voteCounter = 0;
        positiveVoteCounter = 0;
        foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
        {
            PlayerController pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
            if (pc.playerTrigger.Value == 2) positiveVoteCounter += 1;
            if (pc.playerTrigger.Value != 0) voteCounter += 1;
        }
        if (voteCounter == NetworkManager.Singleton.ConnectedClients.Count)
        {
            GameController.Instance.GoToNextRound();
            ResetAllTriggers();
        }
    }

    public void ResetAllTriggers()
    {
        foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
        {
            PlayerController pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
            pc.ResetTriggers();
        }
    }

}
