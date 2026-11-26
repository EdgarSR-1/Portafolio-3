using UnityEngine;
using System;

public enum GameState
{
    Playing,
    Won,
    Draw
}

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private TurnManager turnManager;

    public GameState CurrentState { get; private set; }
    public int Winner { get; private set; } = -1;

    public event Action<GameState, int> OnGameEnded;

    private void Awake()
    {
        CurrentState = GameState.Playing;
    }

    private void OnEnable()
    {
        boardManager.OnReaction += CheckVictory;
        turnManager.OnDrawGame += EndInDraw;
    }

    private void OnDisable()
    {
        boardManager.OnReaction -= CheckVictory;
        turnManager.OnDrawGame -= EndInDraw;
    }

    private void CheckVictory(ElementType result)
    {
        if (CurrentState != GameState.Playing) return;
        if (result != ElementType.Oro) return;

        Winner = turnManager.CurrentPlayer;

        CurrentState = GameState.Won;
        turnManager.EndGame();

        Debug.Log("Jugador " + Winner + " hizo oro y ganó");

        OnGameEnded?.Invoke(CurrentState, Winner);
    }

    private void EndInDraw()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.Draw;

        Debug.Log("La partida terminó en empate");

        OnGameEnded?.Invoke(CurrentState, -1);
    }
}
