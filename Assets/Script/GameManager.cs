using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private GameObject myPlayerListItem;
    int x;

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
            myPlayerListItem.name = pc.playerName.Value;
            myPlayerListItem.GetComponent<NetworkObject>().SpawnWithOwnership(nc.Key);
        }
    }

    public void CheckAllPlayersTrigger()
    {
        if (GameController.Instance.voteCounter == GameController.Instance.players)
        {
            if (GameController.Instance.positiveVoteCounter >= Math.Round((double)(GameController.Instance.voteCounter / 2)))
            {
                AddPoints();
                GoToNextRound();
            }
            else
            {
                if (NetworkManager.Singleton.LocalClientId == GameController.Instance.firstPlayerId)
                    GameController.Instance.animator.SetTrigger("OpenDebil");
                Invoke("GoToNextRound", 1f);
            }
            ResetAllTriggers();
        }
    }

    private void AddPoints()
    {
        foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
        {
            PlayerController pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
            pc.AddPosints();
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

    public void GoToNextRound()
    {
        foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
        {
            PlayerController pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
            pc.GoToNextRound();
        }
    }

}
