using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using TMPro;
using UnityEngine;

public class EndManager : NetworkBehaviour
{
    private GameObject myPlayerListItem;
    private TextMeshProUGUI playerNameLabel;

    public override void NetworkStart()
    {
        if (IsServer)
        {
            foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
            {
                PlayerController pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
                Debug.Log(pc.playerName.Value + ": " + pc.playerPoints.Value);
                myPlayerListItem =
                    Instantiate(EndController.Instance.playerListItemPrefab, Vector3.zero, Quaternion.identity);
                myPlayerListItem.transform.SetParent(EndController.Instance.playerListContainer, false);

                playerNameLabel = myPlayerListItem.GetComponentInChildren<TextMeshProUGUI>();
                playerNameLabel.text = pc.playerName.Value + ": " + pc.playerPoints.Value;
                EndController.Instance.test = pc.playerName.Value + ": " + pc.playerPoints.Value;
                TestClientRpc();
            }
        }
    }

    [ClientRpc]
    private void TestClientRpc()
    {
        Debug.Log(EndController.Instance.test);
    }
    
}