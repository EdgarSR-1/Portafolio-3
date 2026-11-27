using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class BootScene : MonoBehaviour
{
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        SceneManager.LoadScene("MainMenu");
    }
}
