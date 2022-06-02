using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.SceneManagement;
using TMPro;
using UnityEngine;

public class EndController : MonoSingleton<EndController>
{
    
    [SerializeField] public GameObject endManagerPrefab;
    [SerializeField] public Transform playerListContainer;
    [SerializeField] public GameObject playerListItemPrefab;
    
    [SerializeField] public GameObject endClientText;

    public String test;

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject go = Instantiate(Instance.endManagerPrefab);
            go.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
            endClientText.SetActive(false);
        }

    }
}
