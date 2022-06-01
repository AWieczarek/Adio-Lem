using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.SceneManagement;
using UnityEngine;

public class EndController : MonoSingleton<EndController>
{
    
    [SerializeField] public GameObject endManagerPrefab;
    [SerializeField] public Transform playerListContainer;
    [SerializeField] public GameObject playerListItemPrefab;
    
    [SerializeField] public Transform canvas;

    public String test;

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject go = Instantiate(EndController.Instance.endManagerPrefab);
            go.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
        }

    }
}