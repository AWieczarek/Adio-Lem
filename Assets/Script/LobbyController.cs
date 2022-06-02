using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Transports.UNET;
using TMPro;
using UnityEngine;

public class LobbyController : MonoSingleton<LobbyController>
{
    [SerializeField] private Animator animator;

    [SerializeField] public Transform playerListContainer;
    [SerializeField] public GameObject playerListItemPrefab;
    [SerializeField] public TMP_InputField playerNameInput;

    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject nameChanger;

    [SerializeField] public TMP_InputField ipInputField;
    [SerializeField] public TMP_InputField messageInputField;


    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void OnClientButton()
    {
        SetIp();

        NetworkManager.Singleton.StartClient();
        nameChanger.SetActive(true);

        animator.SetTrigger("OpenLobby");
    }

    public void OnHostButton()
    {
        NetworkManager.Singleton.StartHost();
        animator.SetTrigger("OpenLobby");
    }

    public void OnServerButton()
    {
        SetIp();

        NetworkManager.Singleton.StartServer();
        playButton.SetActive(true);
        animator.SetTrigger("OpenLobby");
    }

    public void OnIpSubmit()
    {
        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ipInputField.text;
    }

    private void SetIp()
    {
        if (ipInputField.text.Length <= 0)
        {
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = "192.168.137.1";
        }
        else
        {
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ipInputField.text;
        }
    }

    public void OnLobbyBackButton()
    {
        animator.SetTrigger("OpenStart");
        playButton.SetActive(false);
        nameChanger.SetActive(false);
        NetworkManager.Singleton.Shutdown();
    }

    public void OnSettingsButton()
    {
        animator.SetTrigger("OpenSettings");
    }

    public void OnSettingsBackButton()
    {
        animator.SetTrigger("OpenStart");
    }

    public void OnStartLobbyButton()
    {
        NetworkSceneManager.SwitchScene("GamePhone");
    }

    public void OnLobbySubmitNameChange()
    {
        string newName = playerNameInput.text;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
                out var networkClient))
        {
            var player = networkClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
                player.ChangeName(newName);
        }
    }
}