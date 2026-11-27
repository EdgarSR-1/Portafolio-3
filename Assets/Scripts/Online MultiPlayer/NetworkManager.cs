using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int maxPlayer = 5;

    public static NetworkManager instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("CONNECTED | Region: " + PhotonNetwork.CloudRegion + " | Version: " + PhotonNetwork.AppVersion);
        PhotonNetwork.JoinLobby();
    }

    // public override void OnJoinedLobby()
    // {
    //     Debug.Log("NETWORK MANAGER: Joined Lobby");
    // }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Creado");
    }

    public void CreateRoom(string _RoomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayer;

        PhotonNetwork.CreateRoom(_RoomName, options);
        // Debug.Log(
        // "ROOM CREATED: " +
        // PhotonNetwork.CurrentRoom.Name +
        // " | Visible: " +
        // PhotonNetwork.CurrentRoom.IsVisible +
        // " | Open: " +
        // PhotonNetwork.CurrentRoom.IsOpen);
    }

    public void JoinRoom(string _RoomName)
    {
        PhotonNetwork.JoinRoom(_RoomName);
    }

    [PunRPC]
    public void ChangeScene(string _sceneName)
    {
        PhotonNetwork.LoadLevel(_sceneName);
    }

    // public override void OnCreateRoomFailed(short code, string message)
    // {
    //     Debug.LogError("CREATE ROOM FAILED: " + code + " | " + message);
    // }
}
