using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public partial class MenuController : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Lobby Room")]
    public TextMeshProUGUI playerNames;
    public TextMeshProUGUI roomName;
    public Button startButton;

    public override void OnJoinedRoom()
    {
        Debug.Log("Se unio a room");

        SetScreen(LobbyRoomView);

        photonView.RPC("UpdatePlayerList", RpcTarget.All);
        //UpdateLobbyUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    [PunRPC]
    public void UpdatePlayerList()
    {
        startButton.interactable = PhotonNetwork.IsMasterClient;

        playerNames.text = "";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerNames.text += player.NickName + "\n";
        }
        roomName.text = PhotonNetwork.CurrentRoom.Name.ToString();
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        // dile a todos que carguen la escena
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "GamePlay");
    }

    public void LeaveCurrentLoby()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(MainView);
    }
}
