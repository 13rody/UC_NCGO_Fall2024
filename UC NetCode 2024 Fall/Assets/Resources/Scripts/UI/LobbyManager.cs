using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Button _startBttn, _leaveBttn, _readyBttn;
    [SerializeField] private GameObject _pannelPrefab; // Prefab placed inside contents
    [SerializeField] private GameObject _contentGo;//where we are spawning pannel prefabs
    [SerializeField] private TMP_Text rdyTxt; //update staus to user


    //list of network players 
    [SerializeField] private NetworkedPlayerData _networkPlayers;


    private List<GameObject> _PlayerPannels = new List<GameObject>();

    private ulong _myServerID;

    private bool isReady = false;
    [ContextMenu("PopulateLabel")]
    private void Start()
    {
        _myServerID = NetworkManager.ServerClientId;

        if(IsServer) 
        {
             rdyTxt.text = "waiting for players";
            _readyBttn.gameObject.SetActive(false);
        }
        else
        {
            rdyTxt.text = "Not Ready";
            _readyBttn.gameObject.SetActive(true); 
        }
        _networkPlayers._allConnectedPlayers.OnListChanged += NetPlayersChanged;
        _leaveBttn.onClick.AddListener(LeaveBttnClick);
        _readyBttn.onClick.AddListener(ClientRdyBttnToggle);
    }

    private void ClientRdyBttnToggle()
    {
        if(IsServer) { return; }
        isReady = !isReady;
        if (isReady)
        {
            rdyTxt.text = "Ready";
        }
        else
        {
            rdyTxt.text = "Not Ready";
        }

        RdyBttnToggleServerRpc(isReady);
    }
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void RdyBttnToggleServerRpc(bool readyStatus, RpcParams rpcParams = default)
    {
        Debug.Log("From Rdy Bttn Rpc");
        _networkPlayers.UpdateReadyClient(rpcParams.Receive.SenderClientId, readyStatus);
    }

    private void LeaveBttnClick()
    {
        if (IsServer)
        {
            QuitLobbyRpc();
        }
        else
        {
            foreach(PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
            {
                if(playerData._clinentId != _myServerID)
                {
                    KickUserBttn(playerData._clinentId);
                }
            }
            NetworkManager.Shutdown();
            SceneManager.LoadScene(0);
        }
    }
    [Rpc(SendTo.Server,RequireOwnership = false)]
    private void QuitLobbyRpc(RpcParams rpcParams=default)
    {
        KickUserBttn(rpcParams.Receive.SenderClientId);
    }

    private void NetPlayersChanged(NetworkListEvent<PlayerInfoData> changeevent)
    {
        Debug.Log("NetPlayers has Changed");
        PopulateLables();
    }
    private void PopulateLables()
    {
        ClearPlayerPannel();

        bool allReady = true;

        foreach(PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
        {
            GameObject newPlayerPannel = Instantiate(_pannelPrefab, _contentGo.transform);
            PlayerLabel _playerLabel = newPlayerPannel.GetComponent<PlayerLabel>();
            _playerLabel.onKickClicked += KickUserBttn;
            if (IsServer && playerData._clinentId != _myServerID)
            {
                _playerLabel.SetKickActive(true);
                _readyBttn.GameObject().SetActive(false);
            }
            else
            {
                _playerLabel.SetKickActive(false);
                _readyBttn.GameObject().SetActive(true);
            }

            _playerLabel.SetPlayerLabelName(playerData._clinentId);
            _playerLabel.SetReady(playerData._isPlayerReady);
            _playerLabel.SetPlayerColor(playerData._colorId);
            _PlayerPannels.Add(newPlayerPannel);

            if(playerData._isPlayerReady == false)
            {
                allReady = false;
            } 
        }

        if (IsServer)
        {
            if(allReady)
            {
                if(_networkPlayers._allConnectedPlayers.Count > 1)
                {
                    rdyTxt.text = "ready to start";
                    _startBttn.gameObject.SetActive(true);
                }
                else
                {
                    rdyTxt.text = "empty loby";
                }
            }
            else
            {
                _startBttn.gameObject.SetActive(false);
                rdyTxt.text = "waiting for ready players";
            }
        }
    }

    private void KickUserBttn(ulong kickTarget)
    {
        if (!IsServer || !IsHost) return;

        foreach(PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
        {
            if(playerData._clinentId == kickTarget)
            {
                KickedClientRpc(RpcTarget.Single(kickTarget, RpcTargetUse.Temp));
                NetworkManager.Singleton.DisconnectClient(kickTarget);
            }
        }
    }
    [Rpc(SendTo.SpecifiedInParams)]
    private void KickedClientRpc(RpcParams rpcParams)
    {
        SceneManager.LoadScene(0);
    }

    private void ClearPlayerPannel()
    {
        foreach(GameObject pannel in _PlayerPannels)
        {
            Destroy(pannel);
        }
        _PlayerPannels.Clear();

    }
}
