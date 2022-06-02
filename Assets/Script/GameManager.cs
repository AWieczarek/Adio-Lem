using System.Collections.Generic;
using MLAPI;
using MLAPI.Connection;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private GameObject m_myPlayerListItem;

    public override void NetworkStart()
    {
        if (IsServer)
        {
            SpawnAllPlayers();
            GameController.Instance.namePlaceHolder.SetActive(false);
            GameController.Instance.raiseButton.SetActive(false);
            GameController.Instance.points.SetActive(false);
            GameController.Instance.spotifyButton.SetActive(true);
            GameController.Instance.spotifyData.SetActive(true);
            GameController.Instance.endButton.SetActive(true);
        }
        else
        {
            GameObject sps = GameObject.Find("SpotifyService");
            Destroy(sps);

            GameObject sp = GameObject.Find("Spotify");
            Destroy(sp);
        }
    }

    private void SpawnAllPlayers()
    {
        foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
        {
            PlayerController pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
            m_myPlayerListItem = Instantiate(GameController.Instance.playerPrefab, Vector3.zero, Quaternion.identity);
            m_myPlayerListItem.name = pc.playerName.Value;
            m_myPlayerListItem.GetComponent<NetworkObject>().SpawnWithOwnership(nc.Key);
        }
    }

    public void CheckAllPlayersTrigger()
    {
        if (GameController.Instance.voteCounter == GameController.Instance.players)
        {
            if (GameController.Instance.positiveVoteCounter > (GameController.Instance.voteCounter / 2))
            {
                AddPoints();
                GoToNextRound();
            }
            else
            {
                if (NetworkManager.Singleton.LocalClientId == (ulong)GameController.Instance.firstPlayerId)
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
            pc.AddPoints();
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