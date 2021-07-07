using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TMPro;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    public NetworkVariableString playerName = new NetworkVariableString(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    public NetworkVariableInt playerPoints = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    public NetworkVariableInt playerTrigger = new NetworkVariableInt(new NetworkVariableSettings
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

    [ServerRpc]
    private void PlayServerRpc()
    {
        GameController.Instance.exe.OnPlayMedia();
    }

    public void Play()
    {
        if (!IsOwner) { return; }
        PlayServerRpc();
        GameController.Instance.exe.OnPlayMedia();
    }

    [ServerRpc]
    private void PauseServerRpc()
    {
        GameController.Instance.exe.OnPauseMedia();
    }

    public void Pause()
    {
        if (!IsOwner) { return; }
        PauseServerRpc();
        GameController.Instance.exe.OnPauseMedia();
    }


    [ServerRpc]
    private void NextServerRpc()
    {
        GameController.Instance.exe.OnNextMedia();
    }

    public void Next()
    {
        if (!IsOwner) { return; }
        NextServerRpc();
        GameController.Instance.exe.OnNextMedia();
    }

    [ServerRpc]
    private void PrevServerRpc()
    {
        GameController.Instance.exe.OnPauseMedia();
    }

    public void Prev()
    {
        if (!IsOwner) { return; }
        PrevServerRpc();
        GameController.Instance.exe.OnPreviousMedia();
    }

    public void SelectFirstPlayer()
    {
        if (!IsOwner) { return; }
        if (GameController.Instance.firstPlayerNameLabel.text != "") { return; }
        SelectFirstPlayerServerRpc();
        GameController.Instance.firstPlayerNameLabel.text = playerName.Value;
        Invoke("OnTurnOnTimer", 1f);
    }

    [ServerRpc]
    private void SelectFirstPlayerServerRpc()
    {
        SelectFirstPlayerClientRpc();
        GameController.Instance.firstPlayerNameLabel.text = playerName.Value;
        Invoke("OnTurnOnTimerServer", 1f);
    }

    [ClientRpc]
    private void SelectFirstPlayerClientRpc()
    {
        if (IsOwner) { return; }
        GameController.Instance.firstPlayerNameLabel.text = playerName.Value;
        Invoke("OnTurnOnTimer", 1f);
    }

    private void OnTurnOnTimer()
    {
        GameController.Instance.OnTurnOnTimer();
        GameController.Instance.firstPlayerNameLabel.text = "";
    }
    private void OnTurnOnTimerServer()
    {
        GameController.Instance.OnTurnOnTimerServer();
        GameController.Instance.firstPlayerNameLabel.text = "";
    }
    public void OpenRecentSong()
    {
        if (!IsOwner) { return; }
        OpenRecentSongServerRpc();
        GameController.Instance.OnRecentSongButton();
    }

    [ServerRpc]
    private void OpenRecentSongServerRpc()
    {
        OpenRecentSongClientRpc();
        GameController.Instance.OnRecentSongButton();
    }

    [ClientRpc]
    private void OpenRecentSongClientRpc()
    {
        if (IsOwner) { return; }
        GameController.Instance.OnRecentSongButton();
    }


    public void GoToNextRound()
    {
        if (!IsOwner) { return; }
        GoToNextRoundServerRpc();
        GameController.Instance.exe.OnNextMedia();
        GameController.Instance.exe.OnPlayMedia();
        GameController.Instance.maxTimeOnTimer = 5f;
        GameController.Instance.OnBackToGameButton();
    }

    [ServerRpc]
    private void GoToNextRoundServerRpc()
    {
        GoToNextRoundClientRpc();
        GameController.Instance.exe.OnNextMedia();
        GameController.Instance.exe.OnPlayMedia();
        GameController.Instance.maxTimeOnTimer = 5f;
        GameController.Instance.OnBackToGameButton();
    }

    [ClientRpc]
    private void GoToNextRoundClientRpc()
    {
        if (IsOwner) { return; }
        GameController.Instance.exe.OnNextMedia();
        GameController.Instance.exe.OnPlayMedia();
        GameController.Instance.maxTimeOnTimer = 5f;
        GameController.Instance.OnBackToGameButton();
    }


    public void ResetTriggers()
    {
        if (!IsOwner) { return; }
        ResetTriggersServerRpc();
        GameController.Instance.SetNeutralTrigger();
    }

    [ServerRpc]
    private void ResetTriggersServerRpc()
    {
        ResetTriggersClientRpc();
        GameController.Instance.SetNeutralTrigger();
    }

    [ClientRpc]
    private void ResetTriggersClientRpc()
    {
        if (IsOwner) { return; }
        GameController.Instance.SetNeutralTrigger();
    }
}
