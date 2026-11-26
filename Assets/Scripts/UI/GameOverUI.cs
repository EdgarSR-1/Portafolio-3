using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameOverUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultText;

    private void Start()
    {
        gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        gameManager.OnGameEnded += ShowGameOver;
    }

    private void OnDisable()
    {
        gameManager.OnGameEnded -= ShowGameOver;
    }

    private void ShowGameOver(GameState state, int winner)
    {
        gameOverPanel.SetActive(true);

        if (state == GameState.Won)
        {
            resultText.text = "Jugador " + (winner + 1) + " ganó";
        }
        else if (state == GameState.Draw)
        {
            resultText.text = "Empate";
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EndGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
