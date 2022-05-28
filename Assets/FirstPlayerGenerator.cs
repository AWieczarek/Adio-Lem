using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;
using UnityEngine;

public class FirstPlayerGenerator : NetworkBehaviour
{
    public void GenerateNewNumber() => GenerateNewNumberServerRpc();

    [ServerRpc(RequireOwnership = false)]
    private void GenerateNewNumberServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong senderId = serverRpcParams.Receive.SenderClientId;

        NetworkObject playerObject = NetworkSpawnManager.GetPlayerNetworkObject(senderId);

        if (playerObject == null)
        {
            return;
        }

        var numberHolder = playerObject.GetComponent<PlayerController>();

        Debug.Log((int)senderId);
        numberHolder.UpdateNumber((int)senderId);
    }
}