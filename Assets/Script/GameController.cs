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
    [SerializeField] private Animator animator;
    public SpotifyController exe;
    public TextMeshProUGUI playerNameLabel = null;
    public TextMeshProUGUI playerPointsLabel = null;
    public TextMeshProUGUI firstPlayerNameLabel = null;

    public Image timerBar;
    public float maxTimeOnTimer = 10f;
    float timeLeft;

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

    public void addPlayerPoints(int points)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
            {
                if (player.IsOwner)
                {
                    player.playerPoints.Value += points;
                }
            }
        }
        setPlayerPoints();
    }

    public void setPlayerPoints()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
            {
                if (player.IsOwner)
                {
                    playerPointsLabel.text = player.playerPoints.Value.ToString();
                }
            }
        }
    }

    public void OnRaiseButtonClick()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
                player.SelectFirstPlayer();
        }
        exe.OnPauseMedia();
        Invoke("OnTurnOnTimer", 2f);
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

    public void OnTurnOnTimer()
    {
        animator.SetTrigger("OpenTimer");
        timeLeft = maxTimeOnTimer;
        Invoke("OnYesNoButton", 10f);
    }

    private void Update()
    {
        if(timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerBar.fillAmount = timeLeft / maxTimeOnTimer;
        }
    }

    public void White()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
                player.playerTrigger.Value = 0;
        }
    }

    public void Red()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
                player.playerTrigger.Value = 1;
        }
    }

    public void Green()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
                player.playerTrigger.Value = 2;
        }
    }
}
