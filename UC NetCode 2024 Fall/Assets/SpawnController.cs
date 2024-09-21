using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class SpawnController : NetworkBehaviour
{
    [SerializeField]
    private NetworkObject _playerPrefab;
    [SerializeField]
    private Transform[] _spawnPoints;
    [SerializeField]
    private NetworkVariable<int> _playerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private TMP_Text _countTxt;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
        }
        _playerCount.OnValueChanged += PlayerCountChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
        }
        _playerCount.OnValueChanged -= PlayerCountChanged;
    }

    private void PlayerCountChanged(int previousValue, int newValue)
    {
        UpdateCountTextClientRpc(newValue);
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateCountTextClientRpc(int newValue)
    {
        Debug.Log("Message from client RPC");
        UpdateCountText(newValue);
    }

    private void UpdateCountText(int newValue) 
    {
        _countTxt.text = $"Players : {newValue}";
    }
    private void OnConnectionEvent(NetworkManager netManager, ConnectionEventData eventData) 
    {
       
        if(eventData.EventType == ConnectionEvent.ClientConnected) 
            {
            _playerCount.Value++;
            }
        if(eventData.EventType == ConnectionEvent.ClientDisconnected) 
            {
            _playerCount.Value--; 
            }

    }

    public void spawnAllPlayers()
    {
        if (!IsServer) return;

        int spawnNum = 0;
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            NetworkObject spawnedPlayerNO = NetworkManager.Instantiate(_playerPrefab, _spawnPoints[spawnNum].position, _spawnPoints[spawnNum].rotation);
            spawnedPlayerNO.SpawnAsPlayerObject(clientId);
            
            
            spawnNum++;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
