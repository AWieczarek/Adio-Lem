using MLAPI;
using System;
using System.Collections;
using MLAPI.Connection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI.NetworkVariable;
using TMPro;

public class GameManager : NetworkBehaviour
{
    private GameObject go;

    public override void NetworkStart()
    {
/*        if (IsServer)
            SpawnAllPlayers();*/
    }

/*    private void SpawnAllPlayers()
    {
        foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
        {
            PlayerController pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
            go = GameObject.Instantiate(GameController.Instance.piecePrefab, Vector3.zero, Quaternion.identity);
            //go.transform.SetParent(GameController.Instance.gameListContainer, false);
            go.GetComponent<MeshRenderer>().material.color = pc.playerColor.Value;
            go.GetComponent<NetworkObject>().SpawnWithOwnership(nc.Key);
        }
    }*/
}
