using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoSingleton<GameController>
{
    [SerializeField] public GameObject gameManagerPrefab;
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public Transform playerListContainer;
    [SerializeField] public Animator animator;
    public SpotifyController exe;
    public TextMeshProUGUI playerNameLabel = null;
    public TextMeshProUGUI playerPointsLabel = null;
    public TextMeshProUGUI firstPlayerNameLabel = null;

    public Image timerBar;
    public float maxTimeOnTimer = 5f;
    float timeLeft;

    public ulong firstPlayerId;
    public GameObject GoToNextRoundButton;

    public int voteCounter = 0;
    public int positiveVoteCounter = 0;
    public int players = 0;

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
        setName();
        OnPlayButton();
        if(NetworkManager.Singleton.IsServer)
        {
            GameObject go = Instantiate(GameController.Instance.gameManagerPrefab);
            go.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
        }
        IncreasePlayerCouter();
    }

    public void OnPlayButton()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
            {
                player.Play();
            }
        }
    }

    public void OnPauseButton()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
                player.Pause();
        }
    }

    public void OnNextButton()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
                player.Next();
        }
    }

    public void OnPrevButton()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
                player.Prev();
        }
    }

    public void setName()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
            {
                if (player.IsOwner)
                {
                    playerNameLabel.text = player.playerName.Value;
                }

            }
        }
    }

    public void IncreasePlayerCouter()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
            {
                player.IncreasePlayerCouter();
            }
        }
    }

    public void OnRaiseButtonClick()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
            {
                player.SelectFirstPlayer(networkClient.ClientId);
            }
        }
        exe.OnPauseMedia();
    }

    public void OnSpotifyPanelButton()
    {
        animator.SetTrigger("OpenSpotify");
    }

    public void OnBackToGameButton()
    {
        animator.SetTrigger("OpenGame");
    }

    public void OnYesNoButton()
    {
        animator.SetTrigger("OpenYesNo");
    }
    public void OnRecentSongButton()
    {
        animator.SetTrigger("OpenRecentSong");
        Invoke("OnYesNoButton", 3f);
    }

    public void OnRecentSongServer()
    {
        animator.SetTrigger("OpenRecentSong");
        Invoke("OnVoteListButton", 3f);
    }

    public void OnVoteListButton()
    {
        animator.SetTrigger("OpenVote");
    }

    public void OnTurnOnTimer()
    {
        animator.SetTrigger("OpenTimer");
        timeLeft = maxTimeOnTimer;
        Invoke("OnRecentSongButton", 5f);
    }

    public void OnTurnOnTimerServer()
    {
        animator.SetTrigger("OpenTimer");
        timeLeft = maxTimeOnTimer;
        Invoke("OnRecentSongServer", 5f);
    }

    private void Update()
    {
        if(timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerBar.fillAmount = timeLeft / maxTimeOnTimer;
        }
    }

    public void SetNeutralTrigger()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
            {
                player.playerTrigger.Value = 0;
            }
        }
    }

    public void SetRedTrigger()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
            {
                player.playerTrigger.Value = 1;
                player.IncreaseVoteCounter();
                OnVoteListButton();
            }
        }
    }

    public void SetGreenTrigger()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
            {
                player.playerTrigger.Value = 2;
                player.IncreaseVoteCounter();
                player.IncreasePositiveVoteCounter();
                OnVoteListButton();
            }
        }
    }

    public void GoToNextRound()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
            {
                player.GoToNextRound();
            }
        }
    }
}
