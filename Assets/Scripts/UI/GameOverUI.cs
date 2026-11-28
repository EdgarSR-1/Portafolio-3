using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


public class GameOverUI : MonoBehaviourPunCallbacks
{
    [Header("Referencias")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button restartButton;

    private bool leavingRoom;

    private void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SubscribeEvent();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        UnsubscribeEvent();
    }

    private void SubscribeEvent()
    {
        if (gameManager != null)
            gameManager.OnGameEnded += ShowGameOver;
    }

    private void UnsubscribeEvent()
    {
        if (gameManager != null)
            gameManager.OnGameEnded -= ShowGameOver;
    }

    private void ShowGameOver(GameResult state, int winner)
    {
        gameOverPanel.SetActive(true);
        UpdateRestartButton();

        if (state == GameResult.Won)
        {
            resultText.text = "Jugador " + (winner + 1) + " ganó";
        }
        else if (state == GameResult.Draw)
        {
            resultText.text = "Empate";
        }
    }

    public void RestartGame()
    {
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Solo el master client puede reiniciar");
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            NetworkManager.instance.RestartCurrentScene();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void UpdateRestartButton()
    {
        if (restartButton == null) return;

        restartButton.interactable = 
            !PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient;
    }

    public void EndGame()
    {
        if (PhotonNetwork.InRoom)
        {
            leavingRoom = true;
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public override void OnJoinedRoom()
    {
        UpdateRestartButton();
    }

    public override void OnLeftRoom()
    {
        if (leavingRoom) SceneManager.LoadScene("MainMenu");
        UpdateRestartButton();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // El rival salió: nos vamos al menú también
        if (leavingRoom) return;

        Debug.Log("El otro jugador salió, regresando al menú");
        leavingRoom = true;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        UpdateRestartButton();
    }
}
