using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public partial class MenuController : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Views")]
    public GameObject MainView;
    public GameObject CreateRoomView;
    public GameObject LobbyRoomView;
    public GameObject GeneralLobyView;

    void Start()
    {
        bool ready = PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom;

        btnCreateRoom.interactable = ready;
        btnFindRoom.interactable = false;

        if (PhotonNetwork.InRoom)
        {
            // goto lobby
            PhotonNetwork.CurrentRoom.IsVisible = true; // hacer visible el room
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("MENU CONTROLLER: Joined Lobby");
    }

    // public override void OnRoomListUpdate(List<RoomInfo> rooms)
    // {
    //     Debug.Log("ROOM LIST CALLBACK: " + rooms.Count);

    //     foreach (RoomInfo room in rooms)
    //     {
    //         Debug.Log(
    //             "Room: " + room.Name +
    //             " | Players: " + room.PlayerCount +
    //             " | Open: " + room.IsOpen +
    //             " | Visible: " + room.IsVisible +
    //             " | Removed: " + room.RemovedFromList
    //         );
    //     }

    //     roomsInfo = rooms;
    // }

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
            Debug.Log("Actualizando UI de salas");
            updateLobbyBrowserUI();
        }
    }

    public void BackMainView()
    {
        SetScreen(MainView);
    }
}
