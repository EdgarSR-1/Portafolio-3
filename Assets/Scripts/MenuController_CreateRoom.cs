using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public partial class MenuController : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public void CreateNewRoom(TMP_InputField _RoomName)
    {
        NetworkManager.instance.CreateRoom(_RoomName.text);
    }
}
