using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public partial class MenuController : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Main Screen")]
    public Button btnCreateRoom;
    public Button btnFindRoom;

    public void UpdateNickName(TMP_InputField _playeName)
    {
        PhotonNetwork.NickName = _playeName.text;
    }

    public void ShowCreateRoom()
    {
        SetScreen(CreateRoomView);
    }

    public void ShowFindRoom()
    {
        SetScreen(GeneralLobyView);
    }
}
