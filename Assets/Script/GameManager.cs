using MLAPI;
using MLAPI.Connection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private GameObject myPlayerListItem;
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
        foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
        {
            PlayerController pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
            if (pc.playerTrigger.Value == 2) voteCounter += 1;
        }
        Debug.Log(voteCounter);
        voteCounter = 0;
    }

}
