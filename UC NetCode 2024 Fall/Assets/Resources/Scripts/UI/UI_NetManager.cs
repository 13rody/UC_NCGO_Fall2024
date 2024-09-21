using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UI_NetManager : NetworkBehaviour
{
    [SerializeField] private Button _serverBttn, _clientBttn, _hostBttn, _startBttn;
    [SerializeField] private GameObject _connectionBttnGroup;
    [SerializeField] private SpawnController _mySpawnControler;

    void Start()
    {
        _startBttn.gameObject.SetActive(false);
        if (_serverBttn == null) Debug.LogError("Server button is not assigned!");
        if (_clientBttn == null) Debug.LogError("Client button is not assigned!");
        if (_hostBttn == null) Debug.LogError("Host button is not assigned!");

        _serverBttn.onClick.AddListener(ServerClick);
        _clientBttn.onClick.AddListener(ClientClick);
        _hostBttn.onClick.AddListener(HostClick);
        _startBttn.onClick.AddListener(StartClick);
    }
    private void StartClick()
    {
        if (IsServer)
        {
            _mySpawnControler.spawnAllPlayers();
            _startBttn.gameObject.SetActive(false);
        }
    }
    private void ServerClick()
    {
        _startBttn.gameObject.gameObject.SetActive(true);
        NetworkManager.Singleton.StartServer();
        _connectionBttnGroup.SetActive(false);
    }
    private void HostClick()
    {
        NetworkManager.Singleton.StartHost();
        _connectionBttnGroup.SetActive(false);
        _startBttn.gameObject.gameObject.SetActive(true);
    } 
    private void ClientClick() 
    {
        NetworkManager.Singleton.StartClient();
        _connectionBttnGroup.SetActive(false);
    }

}
