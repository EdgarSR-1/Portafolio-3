using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private int playerCount = 1;

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
            GameOver = true;
            Debug.Log("Empate: todos los jugadores pasaron turno");

            // if (OnDrawGame != null) OnDrawGame();

            OnDrawGame?.Invoke();

            return;
        }

        currentPlayer++;

        if (currentPlayer >= playerCount) currentPlayer = 0;

        // if (OnTurnChanged != null) OnTurnChanged(currentPlayer);

        OnTurnChanged?.Invoke(currentPlayer);
    }

    public void RegisterMove()
    {
        consecutivePasses = 0;
    }
}
