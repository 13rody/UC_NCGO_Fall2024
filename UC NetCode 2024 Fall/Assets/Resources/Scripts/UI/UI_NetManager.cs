using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UI_NetManager : NetworkBehaviour
{
    [SerializeField] private Button _serverBttn, _clientBttn, _hostBttn;


    void Start()
    {
        if (_serverBttn == null) Debug.LogError("Server button is not assigned!");
        if (_clientBttn == null) Debug.LogError("Client button is not assigned!");
        if (_hostBttn == null) Debug.LogError("Host button is not assigned!");

        _serverBttn.onClick.AddListener(ServerClick);
        _clientBttn.onClick.AddListener(ClientClick);
        _hostBttn.onClick.AddListener(HostClick);
    }
    private void ServerClick()
    {
        NetworkManager.Singleton.StartServer();
        this.gameObject.SetActive(false);
    }
    private void HostClick()
    {
        NetworkManager.Singleton.StartHost();
        this.gameObject.SetActive(false);
    } 
    private void ClientClick() 
    {
        NetworkManager.Singleton.StartClient();
        this.gameObject.SetActive(false);
    }

}
