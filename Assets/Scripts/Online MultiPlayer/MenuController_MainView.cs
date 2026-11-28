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
    
    public void UpdateNickName(TMP_InputField _playerName)
    {
        PhotonNetwork.NickName = _playerName.text;
        Debug.Log("Nickname asignado: " + PhotonNetwork.NickName);
    }

    public void ShowCreateRoom()
    {
        SetScreen(CreateRoomView);
    }

    public void ShowFindRoom()
    {
        Debug.Log("Abriendo navegador de salas");
        SetScreen(GeneralLobyView);
    }
}
