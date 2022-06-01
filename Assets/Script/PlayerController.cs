using System;
using System.Linq;
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
        WritePermission = NetworkVariablePermission.Everyone,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    public NetworkVariableInt firstSelected = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    public NetworkVariableInt playerTrigger = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    
    public NetworkVariableString playerLoseMessage = new NetworkVariableString(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    private GameObject myPlayerListItem;
    private TextMeshProUGUI playerNameLabel;

    public override void NetworkStart()
    {
        RegisterEvents();

        myPlayerListItem =
            Instantiate(LobbyController.Instance.playerListItemPrefab, Vector3.zero, Quaternion.identity);
        myPlayerListItem.transform.SetParent(LobbyController.Instance.playerListContainer, false);

        playerNameLabel = myPlayerListItem.GetComponentInChildren<TextMeshProUGUI>();

        if (IsOwner)
        {
            playerName.Value = "Player " + (NetworkManager.Singleton.LocalClientId - 1);
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

    public void AddPosints()
    {
        if (NetworkManager.Singleton.LocalClientId == (ulong)GameController.Instance.firstPlayerId)
            playerPoints.Value += 300;
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
        firstSelected.OnValueChanged += OnFirstPlayerChange;
    }

    private void UnRegisterEvents()
    {
        playerName.OnValueChanged -= OnPlayerNameChange;
        firstSelected.OnValueChanged -= OnFirstPlayerChange;
    }

    private void OnPlayerNameChange(string previousValue, string newValue)
    {
        playerNameLabel.text = playerName.Value;
    }

    private void OnFirstPlayerChange(int previousValue, int newValue)
    {
        if (!IsClient)
        {
            return;
        }

        Debug.Log("Zmieniam wartość.");
    }

    public void UpdateNumber(int newNumber)
    {
        firstSelected.Value = newNumber;
        if (GameController.Instance.firstPlayerId == 0)
        {
            GameController.Instance.exe.OnPauseMedia();

            GameController.Instance.firstPlayerNameLabel.text = newNumber.ToString();
            GameController.Instance.firstPlayerId = newNumber;
            GameController.Instance.firstPlayerNameLabel.text = playerName.Value;
            if (NetworkManager.Singleton.LocalClientId != (ulong)newNumber)
                GameController.Instance.GoToNextRoundButton.SetActive(false);
            TestClientRPC(newNumber);
            Invoke("OnTurnOnTimerServer", 1f);
        }
        else
        {
            FailClientRPC();
        }
    }

    [ClientRpc]
    private void FailClientRPC()
    {
        Debug.Log("Niestety wygrał: gracz nr: " + GameController.Instance.firstPlayerId.ToString());
    }

    [ClientRpc]
    private void TestClientRPC(int newNumber)
    {
        GameController.Instance.exe.OnPauseMedia();

        GameController.Instance.firstPlayerNameLabel.text = newNumber.ToString();
        GameController.Instance.firstPlayerId = newNumber;
        GameController.Instance.firstPlayerNameLabel.text = playerName.Value;
        if (NetworkManager.Singleton.LocalClientId != (ulong)newNumber)
            GameController.Instance.GoToNextRoundButton.SetActive(false);
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
        if (!IsOwner)
        {
            return;
        }

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
        if (IsOwner)
        {
            return;
        }

        GameController.Instance.OnRecentSongButton();
    }


    public void GoToNextRound()
    {
        if (!IsOwner)
        {
            return;
        }

        GoToNextRoundServerRpc();
        GameController.Instance.exe.OnNextMedia();
        GameController.Instance.exe.OnPlayMedia();
        GameController.Instance.maxTimeOnTimer = 5f;
        GameController.Instance.OnBackToGameButton();
        if (IsOwner)
            GameController.Instance.playerPointsLabel.text = playerPoints.Value.ToString();
        GameController.Instance.GoToNextRoundButton.SetActive(true);
        GameController.Instance.voteCounter = 0;
        GameController.Instance.positiveVoteCounter = 0;
        GameController.Instance.firstPlayerId = 0;
    }

    [ServerRpc(RequireOwnership = false)]
    private void GoToNextRoundServerRpc()
    {
        GoToNextRoundClientRpc();
        GameController.Instance.exe.OnNextMedia();
        GameController.Instance.exe.OnPlayMedia();
        GameController.Instance.maxTimeOnTimer = 5f;
        GameController.Instance.OnBackToGameButton();
        if (IsOwner)
            GameController.Instance.playerPointsLabel.text = playerPoints.Value.ToString();
        GameController.Instance.GoToNextRoundButton.SetActive(true);
        GameController.Instance.voteCounter = 0;
        GameController.Instance.positiveVoteCounter = 0;
        GameController.Instance.firstPlayerId = 0;
    }

    [ClientRpc]
    private void GoToNextRoundClientRpc()
    {
        if (IsOwner)
        {
            return;
        }

        GameController.Instance.exe.OnNextMedia();
        GameController.Instance.exe.OnPlayMedia();
        GameController.Instance.maxTimeOnTimer = 5f;
        GameController.Instance.OnBackToGameButton();
        if (IsOwner)
            GameController.Instance.playerPointsLabel.text = playerPoints.Value.ToString();
        GameController.Instance.GoToNextRoundButton.SetActive(true);
        GameController.Instance.voteCounter = 0;
        GameController.Instance.positiveVoteCounter = 0;
        GameController.Instance.firstPlayerId = 0;
    }

    public void ResetTriggers()
    {
        if (!IsOwner)
        {
            return;
        }

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
        if (IsOwner)
        {
            return;
        }

        GameController.Instance.SetNeutralTrigger();
    }


    public void IncreaseVoteCounter()
    {
        if (!IsOwner)
        {
            return;
        }

        IncreaseVoteCounterServerRpc();
        GameController.Instance.voteCounter += 1;
    }

    [ServerRpc]
    private void IncreaseVoteCounterServerRpc()
    {
        IncreaseVoteCounterClientRpc();
        GameController.Instance.voteCounter += 1;
    }

    [ClientRpc]
    private void IncreaseVoteCounterClientRpc()
    {
        if (IsOwner)
        {
            return;
        }

        GameController.Instance.voteCounter += 1;
    }

    public void IncreasePositiveVoteCounter()
    {
        if (!IsOwner)
        {
            return;
        }

        IncreasePositiveVoteCounterServerRpc();
        GameController.Instance.positiveVoteCounter += 1;
    }

    [ServerRpc]
    private void IncreasePositiveVoteCounterServerRpc()
    {
        IncreasePositiveVoteCounterClientRpc();
        GameController.Instance.positiveVoteCounter += 1;
    }

    [ClientRpc]
    private void IncreasePositiveVoteCounterClientRpc()
    {
        if (IsOwner)
        {
            return;
        }

        GameController.Instance.positiveVoteCounter += 1;
    }


    public void IncreasePlayerCounter()
    {
        if (!IsOwner)
        {
            return;
        }

        IncreasePlayerCounterServerRpc();
        GameController.Instance.players += 1;
    }

    [ServerRpc]
    private void IncreasePlayerCounterServerRpc()
    {
        IncreasePlayerCounterClientRpc();
        GameController.Instance.players += 1;
    }

    [ClientRpc]
    private void IncreasePlayerCounterClientRpc()
    {
        if (IsOwner)
        {
            return;
        }

        GameController.Instance.players += 1;
    }
}