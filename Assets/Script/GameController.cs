using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoSingleton<GameController>
{
    public GameObject gameManagerPrefab;
    [SerializeField] public GameObject piecePrefab;
    public TextMeshProUGUI Label;

    private void Start()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.ServerClientId, out var networkedClient))
        {
            var player = networkedClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
            {
                player.CreateGameManager();
            }
        }
    }
}
