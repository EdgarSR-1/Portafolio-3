using Photon.Pun;
using Photon.Realtime;
using TMPro;

public partial class MenuController : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public void CreateNewRoom(TMP_InputField _RoomName)
    {
        NetworkManager.instance.CreateRoom(_RoomName.text);
    }
}
