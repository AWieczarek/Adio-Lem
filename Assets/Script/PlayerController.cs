using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TMPro;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    private NetworkVariableString playerName = new NetworkVariableString(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });


    private GameObject myPlayerListItem;
    private TextMeshProUGUI playerNameLabel;

    public override void NetworkStart()
    {
        RegisterEvents();

        myPlayerListItem = Instantiate(LobbyController.Instance.playerListItemPrefab, Vector3.zero, Quaternion.identity);
        myPlayerListItem.transform.SetParent(LobbyController.Instance.playerListContainer, false);

        playerNameLabel = myPlayerListItem.GetComponentInChildren<TextMeshProUGUI>();

        if (IsOwner)
        {
            if (NetworkManager.Singleton.LocalClientId == 0) playerName.Value = "Player " + "1";
            else playerName.Value = "Player " + NetworkManager.Singleton.LocalClientId;
        }
        else
        {
            playerNameLabel.text = playerName.Value;
        }
    }

    public void OnDestroy()
    {
        Destroy(myPlayerListItem);
        UnRegisterEvents();
    }

    public void CreateGameManager()
    {
        if (IsServer)
        {
            GameObject go = Instantiate(GameController.Instance.gameManagerPrefab);
            go.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
        }
    }


    public void ChangeName(string newName)
    {
        if (IsOwner)
            playerName.Value = newName;

        playerNameLabel.text = playerName.Value;
    }

    private void RegisterEvents()
    {
        playerName.OnValueChanged += OnPlayerNameChange;
    }

    private void UnRegisterEvents()
    {
        playerName.OnValueChanged -= OnPlayerNameChange;
    }

    private void OnPlayerNameChange(string previousValue, string newValue)
    {
        playerNameLabel.text = playerName.Value;
    }

}
