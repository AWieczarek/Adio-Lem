using System.Collections.Generic;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.Spawning;
using TMPro;
using UnityEngine;

public class FirstPlayerGenerator : NetworkBehaviour
{
    public void SelectFirstPlayer() => SelectFirstPlayerServerRpc();

    [ServerRpc(RequireOwnership = false)]
    private void SelectFirstPlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong senderId = serverRpcParams.Receive.SenderClientId;

        NetworkObject playerObject = NetworkSpawnManager.GetPlayerNetworkObject(senderId);

        if (playerObject == null)
        {
            return;
        }

        var numberHolder = playerObject.GetComponent<PlayerController>();

        numberHolder.UpdateFirstPlayer((int)senderId);
    }
}