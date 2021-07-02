using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoSingleton<LobbyController>
{
    [SerializeField] private Animator animator;

    public void OnClientButton()
    {
        animator.SetTrigger("OpenLobby");
    }

    public void OnHostButton()
    {
        animator.SetTrigger("OpenLobby");
    }

    public void OnServerButton()
    {
        animator.SetTrigger("OpenLobby");
    }

    public void OnLobbyBackButton()
    {
        animator.SetTrigger("OpenStart");
    }

    public void OnSettingsButton()
    {
        animator.SetTrigger("OpenSettings");
    }

    public void OnSettingsBackButton()
    {
        animator.SetTrigger("OpenStart");
    }
}
