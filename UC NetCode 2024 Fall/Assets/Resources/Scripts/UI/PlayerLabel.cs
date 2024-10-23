using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerText;
    [SerializeField] private Button _kickButton;
    [SerializeField] private RawImage _ReadyStatusImage, _PlayerColorImg;

    public event Action<ulong> onKickClicked;
    public ulong _clientId;

    private void OnEnable()
    {
        _kickButton.onClick.AddListener((BttnKick_Clicked));
    }

    public void SetPlayerLabelName(ulong playerName)
    {
        _clientId = playerName;
        _playerText.text = "player " +playerName.ToString();
    }

    private void BttnKick_Clicked()
    {
        onKickClicked?.Invoke(_clientId);
    }

    public void SetKickActive(bool isOn)
    {
        _kickButton.gameObject.SetActive(isOn);
    }

    public void SetReady(bool ready)
    {
        if (ready) 
        {
            _ReadyStatusImage.color = Color.green;
        }
        else
        {
            _ReadyStatusImage.color = Color.red;
        }
    }

    public void SetPlayerColor(Color color)
    {
        _PlayerColorImg.color = color;
    }
}
