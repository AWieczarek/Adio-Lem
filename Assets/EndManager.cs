using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using TMPro;
using UnityEngine;

public class EndManager : NetworkBehaviour
{
    private GameObject m_myPlayerListItem;
    private TextMeshProUGUI m_playerNameLabel;

    public override void NetworkStart()
    {
        if (IsServer)
        {
            foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
            {
                PlayerController pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
                m_myPlayerListItem =
                    Instantiate(EndController.Instance.playerListItemPrefab, Vector3.zero, Quaternion.identity);
                m_myPlayerListItem.transform.SetParent(EndController.Instance.playerListContainer, false);

                m_playerNameLabel = m_myPlayerListItem.GetComponentInChildren<TextMeshProUGUI>();
                m_playerNameLabel.text = pc.playerName.Value + ":    " + pc.playerPoints.Value;
                EndController.Instance.test = pc.playerName.Value + ":    " + pc.playerPoints.Value;
            }
        }
    }
}