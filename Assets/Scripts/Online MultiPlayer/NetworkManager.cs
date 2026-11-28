using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

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
        PhotonNetwork.AutomaticallySyncScene = true;
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

    public void RequestTilePlacement(
        string tileID, int leftElement, int rightElement, int side)
    {
        photonView.RPC(
            "RecieveMoveRequest", RpcTarget.MasterClient,
            tileID, leftElement, rightElement, side);
    }

    [PunRPC]
    private void RecieveMoveRequest(
        string tileID, int leftElement, int rightElement, int side,
        PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameUIManager gameUI = FindFirstObjectByType<GameUIManager>(); 
        if (gameUI == null) return;

        int requestingPlayer = info.Sender.ActorNumber - 1;

        if (!gameUI.ValidateMoveRequest(
            requestingPlayer, (ElementType)leftElement, (ElementType)rightElement, (BoardSide)side))
        {
            Debug.LogWarning("Movimiento Multiplayer Rechazado");
            return;
        }

        photonView.RPC(
            "BroadcastTilePlacement",RpcTarget.All,tileID,
            leftElement, rightElement, side, requestingPlayer);
    }

    [PunRPC]
    private void BroadcastTilePlacement(
        string tileID, int leftElement, int rightElement, int side, int playerIndex)
    {
        GameUIManager gameUI = FindFirstObjectByType<GameUIManager>();
        if (gameUI == null) return;

        gameUI.ApplyNetworkMove(
            tileID, (ElementType)leftElement, (ElementType)rightElement, (BoardSide)side, playerIndex);
    }

    public void RequestPass()
    {
        photonView.RPC("RecievePassRequest", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RecievePassRequest(PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameUIManager gameUI = FindFirstObjectByType<GameUIManager>();
        if (gameUI == null) return;

        int player = info.Sender.ActorNumber - 1;
        if (!gameUI.ValidatePassRequest(player)) return;

        photonView.RPC("BroadcastPass", RpcTarget.All, player);
    }

    [PunRPC]
    private void BroadCastPass(int playerIndex)
    {
        GameUIManager gameUI = FindFirstObjectByType<GameUIManager>();
        if (gameUI == null) return;

        gameUI.ApplyNetworkPass(playerIndex);
    } 

    public void RestartCurrentScene()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // primero avisar a los demás, leugo cargar el master
        photonView.RPC("RPC_RestartScene", RpcTarget.Others, sceneIndex);
        PhotonNetwork.LoadLevel(sceneIndex);
    }

    [PunRPC]
    private void RPC_RestartScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void NotifyDraw()
    {
        photonView.RPC("RPC_OpponentDrew", RpcTarget.Others);
    }

    [PunRPC]
    private void RPC_OpponentDrew()
    {
        GameUIManager gameUI = FindFirstObjectByType<GameUIManager>();
        if (gameUI == null) return;

        gameUI.ApplyOpponentDraw();
    }
}
