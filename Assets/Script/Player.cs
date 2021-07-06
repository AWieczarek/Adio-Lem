using MLAPI;
using MLAPI.Messaging;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    private PlayerController owner;
    public TextMeshProUGUI playerNameLabel;
    public int triger;

    [SerializeField] private GameObject red;
    [SerializeField] private GameObject green;
    [SerializeField] private GameObject white;

    public override void NetworkStart()
    {
        playerNameLabel.transform.parent.SetParent(GameController.Instance.playerListContainer, false);

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var networkedClient))
        {
            owner = networkedClient.PlayerObject.GetComponent<PlayerController>();
            playerNameLabel.text = owner.playerName.Value;
            triger = owner.playerTrigger.Value;
        }
        else
        {
            triger = 0;
            ChangeTrigger(triger);
        }

        RegisterEvents();
    }

    public void OnDestroy()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        owner.playerTrigger.OnValueChanged += OnPlayerTriggerChanged;
    }

    private void UnregisterEvents()
    {
        owner.playerTrigger.OnValueChanged -= OnPlayerTriggerChanged;
    }

    private void OnPlayerTriggerChanged(int previousValue, int newValue)
    {
        triger = newValue;
        ChangeTrigger(newValue);
    }

    public void ChangeTrigger(int newValue)
    {
        if (!IsOwner) { return; }
        ChangeTriggerServerRpc(newValue);
        ChangeTriggerLabel(newValue);
    }

    [ServerRpc]
    private void ChangeTriggerServerRpc(int newValue)
    {
        ChangeTriggerClientRpc(newValue);
        ChangeTriggerLabel(newValue);
    }

    [ClientRpc]
    private void ChangeTriggerClientRpc(int newValue)
    {
        if (IsOwner) { return; }
        ChangeTriggerLabel(newValue);
    }

    public void ChangeTriggerLabel(int newValue)
    {
        if (newValue == 0)
        {
            white.SetActive(true);
            green.SetActive(false);
            red.SetActive(false);
        }
        else if (newValue == 1)
        {
            white.SetActive(false);
            green.SetActive(false);
            red.SetActive(true);
        }
        else
        {
            white.SetActive(false);
            green.SetActive(true);
            red.SetActive(false);
        }
    }
}
