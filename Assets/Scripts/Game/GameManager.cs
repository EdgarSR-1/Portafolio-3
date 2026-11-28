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

    // patron de diseño state
    public MatchStateMachine StateMachine { get; } = new MatchStateMachine();

    // true solo mientras se puede jugar
    public bool IsPlaying => StateMachine.Current is TurnState;
    public int Winner { get; private set; } = -1;

    public event Action<GameResult, int> OnGameEnded;

    private void Awake()
    {
        StateMachine.ChangeState(new DealingState());
    }

    private void OnEnable()
    {
        boardManager.OnStartingTileSet += StartFirstTurn;
        boardManager.OnReaction += CheckVictory;
        turnManager.OnTurnChanged += HandleTurnChanged;
        turnManager.OnDrawGame += EndInDraw;
    }

    private void OnDisable()
    {
        boardManager.OnStartingTileSet -= StartFirstTurn;
        boardManager.OnReaction -= CheckVictory;
        turnManager.OnTurnChanged -= HandleTurnChanged;
        turnManager.OnDrawGame -= EndInDraw;
    }

    // primer turno, tiene que estar la ficha inicial
    private void StartFirstTurn()
    {
        if (StateMachine.Current is DealingState)
        {
            StateMachine.ChangeState(new TurnState(turnManager.CurrentPlayer));
        }
    }

    private void HandleTurnChanged(int player)
    {
        if (!IsPlaying) return;
        StateMachine.ChangeState(new TurnState(player));
    }

    private void CheckVictory(ElementType result)
    {
        if (!IsPlaying) return;
        if (result != ElementType.Oro) return;

        Winner = turnManager.CurrentPlayer;
        Debug.Log("Jugador " + Winner + " hizo oro y ganó");

        EndGame(GameResult.Won, Winner);
    }

    private void EndInDraw()
    {
        if (!IsPlaying) return;
        EndGame(GameResult.Draw, -1);
    }

    private void EndGame(GameResult result, int winner)
    {
        StateMachine.ChangeState(new GameOverState(result, winner));
        OnGameEnded?.Invoke(result, winner);
    }
}
