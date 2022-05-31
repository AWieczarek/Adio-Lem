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
    
    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void OnClientButton()
    {
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
        NetworkManager.Singleton.StartServer();
        playButton.SetActive(true);
        animator.SetTrigger("OpenLobby");
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