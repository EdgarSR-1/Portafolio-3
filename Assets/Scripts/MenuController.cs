using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public partial class MenuController : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Views")]
    public GameObject MainView;
    public GameObject CreateRoomView;
    public GameObject LobbyRoomView;
    public GameObject GeneralLobyView;

    void Start()
    {
        btnCreateRoom.interactable = false;
        btnFindRoom.interactable = false;

        if (PhotonNetwork.InRoom)
        {
            // goto lobby
            PhotonNetwork.CurrentRoom.IsVisible = true; // hacer visible el room
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }

    public override void OnConnectedToMaster()
    {
        btnCreateRoom.interactable = true;
        btnFindRoom.interactable = true;
    }

    void SetScreen(GameObject _screen)
    {
        LobbyRoomView.SetActive(false);
        CreateRoomView.SetActive(false);
        GeneralLobyView.SetActive(false);
        MainView.SetActive(false);

        _screen.SetActive(true);
        
        if(_screen == GeneralLobyView)
        {
            updateLobbyBrowserUI();
        }
    }

    public void BackMainView()
    {
        SetScreen(MainView);
    }
}
