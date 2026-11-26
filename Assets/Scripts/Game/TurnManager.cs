using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private int playerCount = 2;

    private int currentPlayer = 0;
    private int consecutivePasses = 0;

    public bool GameOver { get; private set; }

    public event Action<int> OnTurnChanged;
    public event Action OnDrawGame;

    public int CurrentPlayer
    {
        get { return currentPlayer; }
    }

    public void PassTurn()
    {
        if (GameOver) return;

        consecutivePasses++;

        Debug.Log("Jugador " + currentPlayer + " paso su turno");

        if (consecutivePasses >= playerCount)
        {
            Debug.Log("Empate: todos los jugadores pasaron turno");
            EndGame();
            // if (OnDrawGame != null) OnDrawGame();
            OnDrawGame?.Invoke();
            return;
        }

        AdvanceTurn();
    }

    public void RegisterMove()
    {
        if (GameOver) return;

        consecutivePasses = 0;
        AdvanceTurn();
    }

    private void AdvanceTurn()
    {
        currentPlayer++;

        if (currentPlayer >= playerCount) currentPlayer = 0;

        OnTurnChanged?.Invoke(currentPlayer);
    }

    public void EndGame()
    {
        GameOver = true;
    }
}
